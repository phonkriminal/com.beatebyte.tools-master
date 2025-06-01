using System;
using UnityEngine;

namespace BeatebyteToolsEditor.Shared
{

    public static partial class Utils
    {
        public static void Add(this Texture2D tex1, Texture2D tex2)
        {
            if (tex1.width != tex2.width) throw new ArgumentException("Textures must have the same width!");
            if (tex1.height != tex2.height) throw new ArgumentException("Textures must have the same height!");

            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) + tex2.GetPixel(i, j));
        }

        public static void Subtract(this Texture2D tex1, Texture2D tex2)
        {
            if (tex1.width != tex2.width) throw new ArgumentException("Textures must have the same width!");
            if (tex1.height != tex2.height) throw new ArgumentException("Textures must have the same height!");

            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) - tex2.GetPixel(i, j));
        }

        public static void Multiply(this Texture2D tex1, Texture2D tex2)
        {
            if (tex1.width != tex2.width) throw new ArgumentException("Textures must have the same width!");
            if (tex1.height != tex2.height) throw new ArgumentException("Textures must have the same height!");

            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) * tex2.GetPixel(i, j));
        }

        public static void Add(this Texture2D tex1, Color color)
        {
            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) + color);
        }

        public static void Subtract(this Texture2D tex1, Color color)
        {
            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) - color);
        }

        public static void Multiply(this Texture2D tex1, Color color)
        {
            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, tex1.GetPixel(i, j) * color);
        }


        public static Texture2D Copy(this Texture2D tex1)
        {
            Texture2D tex2 = new Texture2D(tex1.width, tex1.height);
            tex2.SetPixels(tex1.GetPixels());
            return tex2;
        }

        public static void Replace(this Texture2D tex1, Color color)
        {
            for (int i = 0; i < tex1.width; i++)
                for (int j = 0; j < tex1.height; j++)
                    tex1.SetPixel(i, j, color);
        }
        public static void BlendTop(this Texture2D Bottom, Texture2D Top)
        {
            var bData = Bottom.GetPixels();
            var tData = Top.GetPixels();
            int count = bData.Length;
            var final = new Color[count];
            int i = 0;
            int iT = 0;
            int startPos = (Bottom.width / 2) - (Top.width / 2) - 1;
            int endPos = Bottom.width - startPos - 1;

            for (int y = 0; y < Bottom.height; y++)
            {
                for (int x = 0; x < Bottom.width; x++)
                {
                    if (y > startPos && y < endPos && x > startPos && x < endPos)
                    {
                        Color B = bData[i];
                        Color T = tData[iT];
                        Color R;

                        R = new Color((T.a * T.r) + ((1 - T.a) * B.r),
                            (T.a * T.g) + ((1 - T.a) * B.g),
                            (T.a * T.b) + ((1 - T.a) * B.b), 1.0f);
                        final[i] = R;
                        i++;
                        iT++;
                    }
                    else
                    {
                        final[i] = bData[i];
                        i++;
                    }
                }
            }
            /*var res = new Texture2D(Bottom.width, Bottom.height);
            res.SetPixels(final);
            res.Apply();
            Bottom = res.Copy();
            Bottom.Apply();*/
            Bottom.SetPixels(final);
            Bottom.Apply();
            //return res;
        }
        public static void BlendBottom(this Texture2D Top, Texture2D Bottom)
        {
            var bData = Bottom.GetPixels();
            var tData = Top.GetPixels();
            int count = bData.Length;
            var final = new Color[count];
            int i = 0;
            int iT = 0;
            int startPos = (Bottom.width / 2) - (Top.width / 2) - 1;
            int endPos = Bottom.width - startPos - 1;

            for (int y = 0; y < Bottom.height; y++)
            {
                for (int x = 0; x < Bottom.width; x++)
                {
                    if (y > startPos && y < endPos && x > startPos && x < endPos)
                    {
                        Color B = bData[i];
                        Color T = tData[iT];
                        Color R;

                        R = new Color((T.a * T.r) + ((1 - T.a) * B.r),
                            (T.a * T.g) + ((1 - T.a) * B.g),
                            (T.a * T.b) + ((1 - T.a) * B.b), 1.0f);
                        final[i] = R;
                        i++;
                        iT++;
                    }
                    else
                    {
                        final[i] = bData[i];
                        i++;
                    }
                }
            }
            /*        var res = new Texture2D(Bottom.width, Bottom.height);
                    res.SetPixels(final);
                    res.Apply();
            */
            Top.SetPixels(final);
            Top.Apply();
        }
    } 
}