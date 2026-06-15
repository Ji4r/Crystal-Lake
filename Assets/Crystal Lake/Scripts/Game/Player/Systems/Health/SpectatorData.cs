using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyProj
{
    public class SpectatorData : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private AllPartPlayer allPartPlayer;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform spectatorCamera;
        [SerializeField] private GameObject mainUi;
        [SerializeField] private GameObject spectatorUi;
        [SerializeField] private GameObject voiceChat;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;

        private Transform target;
        private AllPartPlayer targetPlayer;

        public void EnableSpectatorMode()
        {
            spectatorCamera.gameObject.SetActive(true);
            spectatorUi.SetActive(true);
            mainCamera.enabled = false;
            mainUi.SetActive(false); 
            //voiceChat.SetActive(false);
        }

        public void DisableSpectatorMode()
        {
            mainCamera.enabled = true;
            mainUi.SetActive(true);
            spectatorCamera.gameObject.SetActive(false);
            spectatorUi.SetActive(false);
            //voiceChat.SetActive(true);
        }

        public void BtnNextPlayer(UnityAction onClick)
        {
            Debug.Log("BtnNextPlayer add action");
            nextButton.onClick.AddListener(onClick);
        }

        public void BtnBackPlayer(UnityAction onClick)
        {
            Debug.Log("BtnBackPlayer add action");
            backButton.onClick.AddListener(onClick);
        }

        public void SetTarget(AllPartPlayer allPart)
        {
            if (targetPlayer != null)
            {
                targetPlayer.Get<CharacterControllMesh>().EnabledMesh();
            }

            targetPlayer = allPart;
            target = allPart.CharacterCamera.transform;
            targetPlayer.Get<CharacterControllMesh>().DisableMesh();
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            spectatorCamera.SetPositionAndRotation(target.position, target.rotation);
        }

        private void OnDisable()
        {

            Debug.Log("SpectatorData disabled");
            nextButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
