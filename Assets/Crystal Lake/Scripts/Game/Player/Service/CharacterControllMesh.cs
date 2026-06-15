using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyProj
{
    public class CharacterControllMesh : NetworkBehaviour, IPartPlayer
    {
        [Header("Dissable")]
        [SerializeField] private SkinnedMeshRenderer[] meshRendererForDissables;
        [SerializeField] private GameObject parentObjectForDisable;
        [SerializeField] private bool dissableChildObjects;

        [Header("Enables")]
        [SerializeField] private SkinnedMeshRenderer[] meshRendererForEnables;

        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) return;

            DisableMesh();
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

        public void DisableMesh()
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
    }
}
