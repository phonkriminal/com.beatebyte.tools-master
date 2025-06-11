using System.Threading;
using UnityEngine;

namespace BeatebyteToolsEditor.Shared
{
    public static partial class Util
    {
        #region To Verify
        private static Color[] texColors;
        private static Color[] newColors;
        private static int w;
        private static float ratioX;
        private static float ratioY;
        private static int w2;
        private static int finishCount;
        private static Mutex mutex;

        public static void Point(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, false);
        }

        public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, true);
        }

        private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = (float)tex.width / newWidth;
                ratioY = (float)tex.height / newHeight;
            }

            w = tex.width;
            w2 = newWidth;
            int cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            int slice = newHeight / cores;

            finishCount = 0;
            mutex ??= new Mutex(false);

            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart
                        ts = useBilinear ? BilinearScale : new ParameterizedThreadStart(PointScale);
                    Thread thread = new(ts);
                    thread.Start(threadData);
                }

                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }

                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Reinitialize(newWidth, newHeight);
#pragma warning disable UNT0017 // SetPixels invocation is slow
            tex.SetPixels(newColors);
#pragma warning restore UNT0017 // SetPixels invocation is slow
            tex.Apply();

            texColors = null;
            newColors = null;
        }

        public static void BilinearScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (int y = threadData.start; y < threadData.end; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                int y1 = yFloor * w;
                int y2 = (yFloor + 1) * w;
                int yw = y * w2;

                for (int x = 0; x < w2; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    float xLerp = (x * ratioX) - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(
                        ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                        ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                        (y * ratioY) - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        public static void PointScale(object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (int y = threadData.start; y < threadData.end; y++)
            {
                int thisY = (int)(ratioY * y) * w;
                int yw = y * w2;
                for (int x = 0; x < w2; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + (ratioX * x))];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + ((c2.r - c1.r) * value),
                c1.g + ((c2.g - c1.g) * value),
                c1.b + ((c2.b - c1.b) * value),
                c1.a + ((c2.a - c1.a) * value));
        }

        public class ThreadData
        {
            public int end;
            public int start;

            public ThreadData(int s, int e)
            {
                this.start = s;
                this.end = e;
            }
        }
        #endregion

        /// A unility class with functions to scale Texture2D Data.
        ///
        /// Scale is performed on the GPU using RTT, so it's blazing fast.
        /// Setting up and Getting back the texture data is the bottleneck.
        /// But Scaling itself costs only 1 draw call and 1 RTT State setup!
        /// WARNING: This script override the RTT Setup! (It sets a RTT!)   
        ///
        /// Note: This scaler does NOT support aspect ratio based scaling. You will have to do it yourself!
        /// It supports Alpha, but you will have to divide by alpha in your shaders,
        /// because of premultiplied alpha effect. Or you should use blend modes.
        public class GPUTextureScaler
        {
            /// <summary>
            ///     Returns a scaled copy of given texture.
            /// </summary>
            /// <param name="tex">Source texure to scale</param>
            /// <param name="width">Destination texture width</param>
            /// <param name="height">Destination texture height</param>
            /// <param name="mode">Filtering mode</param>
            public static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
            {
                Rect texR = new(0, 0, width, height);
                _gpu_scale(src, width, height, mode);

                //Get rendered data back to a new texture
                Texture2D result = new(width, height, TextureFormat.ARGB32, true);
                result.Reinitialize(width, height);
                result.ReadPixels(texR, 0, 0, true);
                return result;
            }

            /// <summary>
            ///     Scales the texture data of the given texture.
            /// </summary>
            /// <param name="tex">Texure to scale</param>
            /// <param name="width">New width</param>
            /// <param name="height">New height</param>
            /// <param name="mode">Filtering mode</param>
            public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
            {
                Rect texR = new(0, 0, width, height);
                _gpu_scale(tex, width, height, mode);

                // Update new texture
                tex.Reinitialize(width, height);
                tex.ReadPixels(texR, 0, 0, true);
                tex.Apply(true); //Remove this if you hate us applying textures for you :)
            }

            // Internal unility that renders the source texture into the RTT - the scaling method itself.
            private static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
            {
                //We need the source texture in VRAM because we render with it
                src.filterMode = fmode;
                src.Apply(true);

                //Using RTT for best quality and performance. Thanks, Unity 5
                RenderTexture rtt = new(width, height, 32);

                //Set the RTT in order to render to it
                Graphics.SetRenderTarget(rtt);

                //Setup 2D matrix in range 0..1, so nobody needs to care about sized
                GL.LoadPixelMatrix(0, 1, 1, 0);

                //Then clear & draw the texture to fill the entire RTT.
                GL.Clear(true, true, new Color(0, 0, 0, 0));
                Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
            }
        }
        public static Texture2D SimpleTexture(this Texture2D texture, BackColor backColor, int width, int height)
        {

            texture.SetPixels(BackgroundColorsArray(backColor, width, height));
            texture.Apply();
            return texture;
        }
        private static Color[] BackgroundColorsArray(BackColor backColor, int width, int height)
        {
            Color[] _colors = new Color[width * height];

            Color _color = new Color();

            switch (backColor)
            {
                case BackColor.Red:
                    _color = new Color(0.67f, 0, 0, 1);
                    break;
                case BackColor.Green:
                    _color = new Color(0, 0.4f, 0, 1);
                    break;
                case BackColor.Blue:
                    _color = new Color(0, 0, 0.4f, 1);
                    break;
                case BackColor.Alpha:
                    _color = new Color(0.35f, 0.35f, 0.35f, 1);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = _color;

                _colors[i].r = _color.r;
                _colors[i].g = _color.g;
                _colors[i].b = _color.b;
                _colors[i].a = _color.a;
            }

            return _colors;
        }

    }

    [System.Serializable]
    public enum BackColor
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Alpha = 3
    }
}
