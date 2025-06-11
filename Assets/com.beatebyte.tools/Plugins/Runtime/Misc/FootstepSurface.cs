using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFootstepSurface", menuName = "Beatebyte/Footstep Surface", order = 0)]
public class FootstepSurface : ScriptableObject
{    
    public FootstepSurfaceData gameData = new();
}

[Serializable]
public class FootstepSurfaceData
{
    [SerializeField] private string surfaceName;
    [SerializeField] private Material surfaceMaterial;
    [SerializeField] private Texture2D surfaceTexture;
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private AudioClip[] runSounds;
    [SerializeField] private AudioClip[] landSounds;
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private AudioClip[] slideSounds;

    public string SurfaceName => surfaceName;
    public Material SurfaceMaterial => surfaceMaterial;
    public Texture2D SurfaceTexture => surfaceTexture;
    public AudioClip[] WalkSounds => walkSounds;
    public AudioClip[] RunSounds => runSounds;
    public AudioClip[] LandSounds => landSounds;
    public AudioClip[] JumpSounds => jumpSounds;
    public AudioClip[] SlideSounds => slideSounds;

    public FootstepSurfaceData(string name = "Default Surface", Material material = null, Texture2D texture = null)
    {
        surfaceName = name;
        surfaceMaterial = material;
        surfaceTexture = texture;
        walkSounds = new AudioClip[0];
        runSounds = new AudioClip[0];
        landSounds = new AudioClip[0];
        jumpSounds = new AudioClip[0];
        slideSounds = new AudioClip[0];
    }
}