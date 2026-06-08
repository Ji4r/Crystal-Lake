using Adrenak.UniVoice;
using Adrenak.UniVoice.Outputs;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProj
{
    public class NetworkVoiceIdentity : NetworkBehaviour
    {
        [SyncVar]
        public int PeerId;

        public override void OnStartLocalPlayer()
        {
            if (VoiceRuntimeData.LocalPeerId != 0)
            {
                CmdSetPeerId(VoiceRuntimeData.LocalPeerId);
            }
        }

        private void OnEnable()
        {
            VoiceRuntimeData.OnPeerIdReceived += HandlePeerIdReceived;
        }

        private void OnDisable()
        {
            VoiceRuntimeData.OnPeerIdReceived -= HandlePeerIdReceived;
        }

        private void HandlePeerIdReceived(int peerId)
        {
            if (!isLocalPlayer)
                return;

            Debug.Log($"PEER ID RECEIVED = {peerId}");

            CmdSetPeerId(peerId);
        }

        [Command]
        private void CmdSetPeerId(int peerId)
        {
            Debug.Log($"SERVER RECEIVED PEER ID = {peerId}");

            PeerId = peerId;
        }
    }

    public static class VoiceIdentity
    {
        public static int LocalPeerId;
    }
}
