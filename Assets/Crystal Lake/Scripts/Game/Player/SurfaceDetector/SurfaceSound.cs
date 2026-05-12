using UnityEngine;

namespace MyProj
{
    [System.Serializable]
    public class SurfaceSound
    {
        public string surfaceName;
        public AudioClip[] clips;

        public AudioClip GetRandomClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
