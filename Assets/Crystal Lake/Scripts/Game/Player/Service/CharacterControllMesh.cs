using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyProj
{
    public class CharacterControllMesh : NetworkBehaviour, IPartPlayer
    {
        [Header("Dissable")]
        public SkinnedMeshRenderer[] meshRendererForDissables;
        [SerializeField] private GameObject parentObjectForDisable;
        [SerializeField] private bool dissableChildObjects;

        [Header("Enables")]
        public GameObject parentLocalHands;
        public SkinnedMeshRenderer[] meshRendererForEnables;

        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) return;

            LocalDisableMesh();
        }

        public void EnabledMesh()
        {
            if (!isLocalPlayer) return;

            foreach (var item in meshRendererForDissables)
            {
                item.shadowCastingMode = ShadowCastingMode.On;
                if (dissableChildObjects)
                {
                    parentObjectForDisable.SetActive(true);
                }
            }


            foreach (var item in meshRendererForEnables)
            {
                if (isLocalPlayer)
                {
                    item.shadowCastingMode = ShadowCastingMode.On;
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.shadowCastingMode = ShadowCastingMode.Off;
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void LocalDisableMesh()
        {
            foreach (var item in meshRendererForDissables)
            {
                item.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                if (dissableChildObjects)
                {
                    parentObjectForDisable.SetActive(false);
                }
            }


            foreach (var item in meshRendererForEnables)
            {
                if (isLocalPlayer)
                {
                    item.shadowCastingMode = ShadowCastingMode.On;
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.shadowCastingMode = ShadowCastingMode.Off;
                    item.gameObject.SetActive(false);
                }
            }
        }


        public void ShowHandsPlayer(AllPartPlayer allPartPlayer)
        {
            var controllMesh = allPartPlayer.Get<CharacterControllMesh>();
            controllMesh.parentLocalHands.SetActive(true);
            foreach (var item in controllMesh.meshRendererForDissables)
            {
                item.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                if (dissableChildObjects)
                {
                    parentObjectForDisable.SetActive(false);
                }
            }


            foreach (var item in controllMesh.meshRendererForEnables)
            {
                item.shadowCastingMode = ShadowCastingMode.On;
                item.gameObject.SetActive(true);
            }
        }

        public void HideHandsPlayer(AllPartPlayer allPartPlayer)
        {
            var controllMesh = allPartPlayer.Get<CharacterControllMesh>();
            controllMesh.parentLocalHands.SetActive(false);
            foreach (var item in controllMesh.meshRendererForDissables)
            {
                item.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                if (dissableChildObjects)
                {
                    parentObjectForDisable.SetActive(false);
                }
            }


            foreach (var item in controllMesh.meshRendererForEnables)
            {
                item.shadowCastingMode = ShadowCastingMode.Off;
                item.gameObject.SetActive(false);
            }
        }
    }
}
