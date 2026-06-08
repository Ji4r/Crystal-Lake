using Adrenak.UniVoice.Outputs;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MyProj
{
    public class VoiceAudioConfigurator : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("VoiceAudioConfigurator enabled");
            StreamedAudioSourceOutput.OnCreated += HandleVoiceCreated;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            StreamedAudioSourceOutput.OnCreated -= HandleVoiceCreated;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void HandleVoiceCreated(StreamedAudioSourceOutput voice)
        {
            Debug.Log("VOICE CREATED EVENT");
            var audio = voice.Stream.UnityAudioSource;

            if (SceneManager.GetActiveScene().name == SceneName.MENU)
            {
                audio.spatialBlend = 0f;
            }
            else
            {
                audio.spatialBlend = 1f;
                audio.rolloffMode = AudioRolloffMode.Linear;
                audio.minDistance = 3f;
                audio.maxDistance = 20f;
                audio.dopplerLevel = 0f;
            }

            var player = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.PeerId == voice.PeerId);
            if (player == null)
            {
                Debug.Log("PLAYER NOT FOUND, RETRYING...");
                StartCoroutine(FindPlayerLater(voice));
            }

            Debug.Log($"SpatialBlend = {audio.spatialBlend}");
            Debug.Log($"Scene = {SceneManager.GetActiveScene().name}");
            Debug.Log(voice.name);
            Debug.Log(voice.transform.position);
            Debug.Log($"Voice for peer {voice.PeerId}");
            Debug.Log($"FOUND PLAYER = {player != null}");

        }

        private IEnumerator FindPlayerLater(StreamedAudioSourceOutput voice)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.5f);

                var player = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.PeerId == voice.PeerId);

                var players = FindObjectsByType<NetworkVoiceIdentity>(
    FindObjectsSortMode.None);

                foreach (var p in players)
                {
                    Debug.Log($"PLAYER: {p.name} PEER={p.PeerId}");
                }

                Debug.Log($"RETRY FOUND = {player != null}");

                if (player != null)
                {
                    Debug.Log($"FOUND PEER {player.PeerId}");
                    break;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != SceneName.GAME)
                return;

            StartCoroutine(AttachAllVoices());
        }

        private IEnumerator AttachAllVoices()
        {
            yield return new WaitForSeconds(1f);

            var voices = FindObjectsByType<StreamedAudioSourceOutput>(
                FindObjectsSortMode.None);

            foreach (var voice in voices)
            {
                AttachVoiceToPlayer(voice);
            }
        }

        private void AttachVoiceToPlayer(StreamedAudioSourceOutput voice)
        {
            var player = FindObjectsByType<NetworkVoiceIdentity>(
                FindObjectsSortMode.None)
                .FirstOrDefault(x => x.PeerId == voice.PeerId);

            if (player == null)
            {
                Debug.Log($"PLAYER NOT FOUND FOR PEER {voice.PeerId}");
                return;
            }

            var allPartPlayer = player.GetComponent<AllPartPlayer>();

            if (allPartPlayer == null)
            {
                Debug.LogWarning($"AllPartPlayer not found on {player.name}");
                return;
            }

            var voiceChat = allPartPlayer.VoiceChatParent;

            if (voiceChat == null)
            {
                Debug.LogWarning($"VoiceChatParent not found on {player.name}");
                return;
            }

            voice.transform.SetParent(voiceChat);
            voice.transform.localPosition = Vector3.zero;
            voice.transform.localRotation = Quaternion.identity;

            var audio = voice.Stream.UnityAudioSource;

            audio.spatialBlend = 1f;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.minDistance = 3f;
            audio.maxDistance = 20f;
            audio.dopplerLevel = 0f;

            Debug.Log($"VOICE ATTACHED TO PLAYER {player.PeerId}");
        }
    }
}
