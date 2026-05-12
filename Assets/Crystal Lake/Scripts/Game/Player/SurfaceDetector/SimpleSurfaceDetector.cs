using UnityEngine;

namespace MyProj
{
    public class SimpleSurfaceDetector : MonoBehaviour, IPartPlayer
    {
        [Header("Настройки детекции")]
        [SerializeField] private float rayDistance = 1.5f;
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private Transform feetPosition;

        [Header("Текущая поверхность")]
        [SerializeField] private string currentSurface = "Concrete";

        private void Start()
        {
            if (feetPosition == null)
                feetPosition = transform;
        }

        public string DetectSurface()
        {
            RaycastHit hit;
            Vector3 rayStart = feetPosition.position;

            Debug.DrawRay(rayStart, Vector3.down * rayDistance, Color.yellow); 

            if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance, groundLayers))
            {
                currentSurface = hit.collider.tag;

                if (hit.collider.TryGetComponent<SurfaceType>(out var surfaceType))
                {
                    currentSurface = surfaceType.surfaceName;
                }

                return currentSurface;
            }

            return "Air";
        }

        public string GetCurrentSurface()
        {
            return currentSurface;
        }
    }
}
