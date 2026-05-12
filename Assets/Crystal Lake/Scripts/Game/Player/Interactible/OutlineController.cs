using UnityEngine;

namespace MyProj
{
    public class OutlineController : MonoBehaviour
    {
        [SerializeField] private float outlineWidth = 0.005f;
        [SerializeField] private GameObject referenceMesh;

        private Renderer rend;
        private MaterialPropertyBlock propertyBlock;

        private static readonly int OutlineID = Shader.PropertyToID("_Outline");

        private void Awake()
        {
            rend = referenceMesh.GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            DisableOutline();
        }

        public void EnableOutline()
        {
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(OutlineID, outlineWidth);
            rend.SetPropertyBlock(propertyBlock);
        }

        public void DisableOutline()
        {
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(OutlineID, 0f);
            rend.SetPropertyBlock(propertyBlock);
        }
    }
}
