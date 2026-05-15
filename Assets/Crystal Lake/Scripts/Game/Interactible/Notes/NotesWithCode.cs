using UnityEngine;

namespace MyProj
{
    public class NotesWithCode : MonoBehaviour, IInteractible
    {
        [SerializeField, Tooltip("Символ, который вместо цифр будет")]
        private char charNotNumber;

        public string Code => code;

        private string code;


        public void Interact(RaycastHit hit, AllPartPlayer allPartPlayer)
        {
            //allPartPlayer.Get<NotesPartPlayer>().OpenNote(this);
        }

        public void SetCode(string code, byte startPosition, byte fullCodeLength)
        {
            if (string.IsNullOrEmpty(code) || code.Length > fullCodeLength)
            {
                Debug.LogError("Недопустимый код или длина кода превышает полную длину кода.");
                return;
            }

            if (startPosition > fullCodeLength || startPosition + code.Length > fullCodeLength)
            {
                Debug.LogError("Недопустимая стартовая позиция.");
                return;
            }

            char[] codeChars = new char[fullCodeLength];

            for (int i = 0; i < fullCodeLength; i++) 
            {
                if (i >= startPosition && i < startPosition + code.Length)
                {
                    codeChars[i] = code[i - startPosition];
                }
                else
                {
                    codeChars[i] = charNotNumber; 
                }
            }

            this.code = new string(codeChars);
        }
    }
}
