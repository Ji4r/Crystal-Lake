//using Adrenak.UniVoice.Outputs;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Zenject;

namespace MyProj
{
    public class VoiceAudioConfigurator : MonoBehaviour
    {
        //[SerializeField] private float attachDelay = 1f;
        //[SerializeField] private float playerSearchRetryDelay = 0.5f;
        //[SerializeField] private int playerSearchRetryCount = 5;

        private VoiceSettings settings;

        [Inject]
        private void Construct(VoiceSettings settings)
        {
            this.settings = settings;
        }

        private void OnEnable()
        {
            //StreamedAudioSourceOutput.OnCreated += HandleVoiceCreated;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            //StreamedAudioSourceOutput.OnCreated -= HandleVoiceCreated;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void ApplyGameAudioSettings(AudioSource audio)
        {
            audio.spatialBlend = settings.SpatialBlendInGame;
            audio.rolloffMode = settings.RolloffMode;
            audio.minDistance = settings.MinDistance;
            audio.maxDistance = settings.MaxDistance;
            audio.dopplerLevel = settings.DopplerLevel;
        }

        //private void HandleVoiceCreated(StreamedAudioSourceOutput voice)
        //{
            //var audio = voice.Stream.UnityAudioSource;

            //if (SceneManager.GetActiveScene().name == SceneName.MENU)
            //{
            //    audio.spatialBlend = settings.SpatialBlendInMenu;
            //}
            //else
            //{
            //    ApplyGameAudioSettings(audio);
            //}

            //var player = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.PeerId == voice.PeerId);
            //if (player == null)
            //{
            //    Debug.Log("PLAYER NOT FOUND, RETRYING...");
            //    StartCoroutine(FindPlayerLater(voice));
            //}
        //}

        //private IEnumerator FindPlayerLater(StreamedAudioSourceOutput voice)
        //{
        //    for (int i = 0; i < playerSearchRetryCount; i++)
        //    {
        //        yield return new WaitForSeconds(playerSearchRetryDelay);

        //        //var player = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.PeerId == voice.PeerId);

        //        var players = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None);

        //        foreach (var p in players)
        //        {
        //            Debug.Log($"PLAYER: {p.name} PEER={p.PeerId}");
        //        }

        //        //Debug.Log($"RETRY FOUND = {player != null}");

        //        //if (player != null)
        //        //{
        //        //    Debug.Log($"FOUND PEER {player.PeerId}");
        //        //    break;
        //        //}
        //    }
        //}

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //if (scene.name != SceneName.GAME)
            //    return;

            //StartCoroutine(AttachAllVoices());
        }

        //private IEnumerator AttachAllVoices()
        //{
        //    //yield return new WaitForSeconds(attachDelay);

        //    //var voices = FindObjectsByType<StreamedAudioSourceOutput>(FindObjectsSortMode.None);

        //    //foreach (var voice in voices)
        //    //{
        //    //    AttachVoiceToPlayer(voice);
        //    //}
        //}

        //private void AttachVoiceToPlayer(StreamedAudioSourceOutput voice)
        //{
        //    //var player = FindObjectsByType<NetworkVoiceIdentity>(FindObjectsSortMode.None).FirstOrDefault(x => x.PeerId == voice.PeerId);

        //    //if (player == null)
        //    //{
        //    //    Debug.Log($"PLAYER NOT FOUND FOR PEER {voice.PeerId}");
        //    //    return;
        //    //}

        //    //var allPartPlayer = player.GetComponent<AllPartPlayer>();

        //    //if (allPartPlayer == null)
        //    //{
        //    //    Debug.LogWarning($"AllPartPlayer not found on {player.name}");
        //    //    return;
        //    //}

        //    //var voiceChat = allPartPlayer.VoiceChatParent;

        //    //if (voiceChat == null)
        //    //{
        //    //    Debug.LogWarning($"VoiceChatParent not found on {player.name}");
        //    //    return;
        //    //}

        //    //voice.transform.SetParent(voiceChat);
        //    //voice.transform.localPosition = Vector3.zero;
        //    //voice.transform.localRotation = Quaternion.identity;
        //    //if (voice.GetComponent<VoiceOcclusion>() == null)
        //    //{
        //    //    var occlusion = voice.gameObject.AddComponent<VoiceOcclusion>();

        //    //    ProjectContext.Instance.Container.Inject(occlusion);
        //    //}

        //    //var audio = voice.Stream.UnityAudioSource;
        //    //ApplyGameAudioSettings(audio);

        //    //Debug.Log($"VOICE ATTACHED TO PLAYER {player.PeerId}");
        //}
    }
}