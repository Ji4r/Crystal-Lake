using System.Collections.Generic;
using UnityEngine;

namespace MyProj
{
    public class FootstepAudioSystem : MonoBehaviour, IPartPlayer
    {
        [Header("References")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private CharacterGravity gravity;
        [SerializeField] private SimpleSurfaceDetector surfaceDetector;

        [Header("Timing")]
        [SerializeField] private float walkStepDelay = 0.5f;
        [SerializeField] private float sprintStepDelay = 0.35f;
        [SerializeField] private float crouchStepDelay = 0.7f;

        [Header("Sounds")]
        [SerializeField] private List<SurfaceSound> surfaces;

        private float stepTimer;

        private void Update()
        {
            if (!gravity.IsGrounded)
                return;

            if (controller.velocity.magnitude < 0.1f)
                return;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayStep();
                stepTimer = walkStepDelay;
            }
        }

        private void PlayStep()
        {
            string surface = surfaceDetector.DetectSurface();

            foreach (var s in surfaces)
            {
                if (s.surfaceName == surface)
                {
                    var clip = s.GetRandomClip();
                    SoundManager.PlayOneShot(clip);
                    return;
                }
            }
        }
    }
}
