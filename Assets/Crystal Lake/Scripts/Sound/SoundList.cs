using UnityEngine;

namespace MyProj
{
    public class SoundList : MonoBehaviour
    {
        public static SoundList instance;

        public SoundAudioClip[] soundAudioSource;

        [System.Serializable]
        public class SoundAudioClip
        {
            public Sound sound;
            public AudioClip audioClip;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
                Destroy(this.gameObject);


            SoundManager.Initialize();
        }
    }
}

