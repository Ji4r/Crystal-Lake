using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyProj
{
    public class PlayerControllMesh : NetworkBehaviour, IPartPlayer
    {
        [SerializeField] private SkinnedMeshRenderer[] meshRenderer;

        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) return;

            foreach (var item in meshRenderer)
            {
                item.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
