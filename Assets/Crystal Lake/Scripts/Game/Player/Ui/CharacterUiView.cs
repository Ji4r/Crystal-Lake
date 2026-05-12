using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    public class CharacterUiView : MonoBehaviour, ILocalOnly
    {
        [SerializeField] private GameObject canvas;
        [Header("StaminaBar")]
        [SerializeField] private StaminaController staminaController;
        [SerializeField] private Image leftStaminaImg;
        [SerializeField] private Image rightStaminaImg;
        [SerializeField] private float durationHideStaminaBar = 1f;

        private void OnEnable()
        {
            staminaController.StaminaChanged += OnStaminaChanged;
        }


        private void OnDisable()
        {
            staminaController.StaminaChanged -= OnStaminaChanged;
        }

        private void OnStaminaChanged(float maxStamina, float currentStamina)
        {
            leftStaminaImg.fillAmount = currentStamina / maxStamina;
            rightStaminaImg.fillAmount = currentStamina / maxStamina;
        }

        public void LocalDissable()
        {
            canvas.SetActive(false);
            this.enabled = false;
        }
    }
}
