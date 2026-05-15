using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace MyProj
{
    public class ButtonKeypad : MonoBehaviour, IPointerClickHandler
    {
        [Header("Value")]
        [SerializeField] private string value;

        [Header("Button Animation Settings")]
        [SerializeField] private float bttnspeed = 0.1f;
        [SerializeField] private float moveDist = 0.0025f;
        [SerializeField] private float buttonPressedTime = 0.1f;

        [Header("Component References")]
        [SerializeField] private Keypad keypad;

        private bool moving;


        public void OnPointerClick(PointerEventData eventData) =>
            PressButton();

        private void PressButton()
        {
            if (!moving)
            {
                keypad.AddValue(value);
                //SoundManager.PlaySound(Sound.Walk);
                StartCoroutine(MoveSmooth());
            }
        }

        private IEnumerator MoveSmooth()
        {

            moving = true;
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = transform.localPosition + new Vector3(0, 0, moveDist);

            float elapsedTime = 0;
            while (elapsedTime < bttnspeed)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / bttnspeed);

                transform.localPosition = Vector3.Lerp(startPos, endPos, t);

                yield return null;
            }
            transform.localPosition = endPos;
            yield return new WaitForSeconds(buttonPressedTime);
            startPos = transform.localPosition;
            endPos = transform.localPosition - new Vector3(0, 0, moveDist);

            elapsedTime = 0;
            while (elapsedTime < bttnspeed)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / bttnspeed);

                transform.localPosition = Vector3.Lerp(startPos, endPos, t);

                yield return null;
            }
            transform.localPosition = endPos;

            moving = false;
        }
    }
}
