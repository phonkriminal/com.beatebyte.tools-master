using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public static class AudioPreviewUtility
{
    private static readonly MethodInfo playPreviewClipMethod;
    private static readonly MethodInfo stopAllPreviewClipsMethod;
    private static readonly MethodInfo playClipMethod;


    static AudioPreviewUtility()
    {
        var audioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
        if (audioUtilType != null)
        {
            playPreviewClipMethod = audioUtilType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.NonPublic, null, new System.Type[] { typeof(AudioClip), typeof(Int32), typeof(Boolean) }, null);
            playClipMethod = audioUtilType.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(AudioClip) }, null);
            stopAllPreviewClipsMethod = audioUtilType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.NonPublic);
        }
    }

    public static void PlayPreviewClip(AudioClip clip)
    {
        Stop();
        Assembly assembly = typeof(AudioImporter).Assembly;
        Type audioUtilType = assembly.GetType("UnityEditor.AudioUtil");
        MethodInfo methodInfo = audioUtilType.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {typeof(AudioClip), typeof(Int32), typeof(Boolean) },
            null);
        methodInfo.Invoke(null, new object[] { clip, 0, false });
    }
   
    public static void Play(AudioClip clip)
    {
        if (clip == null) return;

        Stop();

        if (playPreviewClipMethod != null)
            playPreviewClipMethod.Invoke(null, new object[] { clip, 0, false });
        else if (playClipMethod != null)
            playClipMethod.Invoke(null, new object[] { clip });
        else
            Debug.LogWarning("❌ Nessun metodo di riproduzione valido trovato.");
    }

    public static void Stop()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

        if (audioUtilType == null)
            return;

        MethodInfo stopMethod = audioUtilType.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public
        );

        stopMethod?.Invoke(null, null);
    }
}
