using System;
using UnityEngine;

namespace MyProj
{
    public class CharacterInteracter : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float distanceRay = 2f;
        [SerializeField] private LayerMask ignoreLayer;

        public event Action AimedAtObjectAction;
        public event Action NotAimedAtObjectAction;
        private OutlineController currentOutline;

        public (IInteractible, RaycastHit) CheckInteract()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);

            if (!Physics.Raycast(ray, out var hitInfo, distanceRay, ~ignoreLayer)) {
                ClearOutline();
                NotAimedAtObjectAction?.Invoke();
                return (null, default);
            }

            if (hitInfo.collider.TryGetComponent<IInteractible>(out var interactObject))
            {
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 0.1f);
                var outline = hitInfo.collider.GetComponent<OutlineController>();
                if (outline != currentOutline)
                {
                    ClearOutline();

                    currentOutline = outline;
                    currentOutline?.EnableOutline();
                }
                AimedAtObjectAction?.Invoke();
                return (interactObject, hitInfo);
            }

            NotAimedAtObjectAction?.Invoke();
            return (null, default);
        }

        private void ClearOutline()
        {
            if (currentOutline != null)
            {
                currentOutline.DisableOutline();
                currentOutline = null;
            }
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }
    }
}
