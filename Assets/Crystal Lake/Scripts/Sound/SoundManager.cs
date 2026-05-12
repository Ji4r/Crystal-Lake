using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public enum Sound
    {
        Walk,
        CrackDoor,
        Cr,
        Sprint,
        Crouch,
        Jump,
        FlickerOfLight
    }
    public static class SoundManager
    {
        private static Dictionary<Sound, float> soundTimeDictionary;
        private static GameObject oneShotGameObject;
        private static AudioSource oneShotAudioSource;

        public static void Initialize()
        {
            soundTimeDictionary = new Dictionary<Sound, float>();
            soundTimeDictionary[Sound.Walk] = 0f;
            soundTimeDictionary[Sound.Sprint] = 0f;
            soundTimeDictionary[Sound.Crouch] = 0f;
        }

        public static void PlaySound(Sound sound, Vector3 position) // 3d volue
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(sound);
                audioSource.Play();

                Object.Destroy(soundGameObject, audioSource.clip.length);
            }
        }

        public static void PlaySound(Sound sound) // 2d volue
        {
            if (CanPlaySound(sound))
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("Sound");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                }
                oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
            }
        }

        public static void PlayOneShot(AudioClip clip)
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(clip);
        }

        private static bool CanPlaySound(Sound sound)
        {
            switch (sound)
            {
                default:
                    return true;
                case Sound.Walk:
                case Sound.Sprint:
                case Sound.Crouch:
                    if (soundTimeDictionary.ContainsKey(sound))
                    {
                        float lastTimePlayed = soundTimeDictionary[sound];
                        float playerMoveTimerMax = 0.7f;
                        if (lastTimePlayed + playerMoveTimerMax < Time.time)
                        {
                            soundTimeDictionary[sound] = Time.time;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        soundTimeDictionary[sound] = Time.time;
                        return true;
                    }
            }
        }

        private static AudioClip GetAudioClip(Sound sound)
        {
            foreach (var soundAudioClip in SoundList.instance.soundAudioSource)
            {
                if (soundAudioClip.sound == sound)
                    return soundAudioClip.audioClip;
            }
            Debug.Log("Sound " + sound + "not found!");
            return null;
        }
    }
}