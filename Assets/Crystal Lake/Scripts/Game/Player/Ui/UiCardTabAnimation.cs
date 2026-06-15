using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyProj
{
    public class UiCardTabAnimation : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        public Button BtnPlayAnim => button;

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
        
        public void AddActionOnButton(Action<string> action, string parameter)
        {
            button.onClick.AddListener(() => action.Invoke(parameter));
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
