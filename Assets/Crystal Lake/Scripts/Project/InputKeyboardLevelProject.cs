//using DavidFDev.DevConsole;
using ConsoleShell;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyProj
{
    public class InputKeyboardLevelProject : MonoBehaviour
    {
        [SerializeField] private ConsoleService consoleService;
        public static InputKeyboardLevelProject Instance { get; private set; }

        private ProjectInput inputSystem;
        private bool isOpenConsole;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            inputSystem = new ProjectInput();
            inputSystem.Enable();
        }

        private void OnEnable()
        {
            inputSystem.MainMap.Console.performed += Console_performed;
        }


        private void OnDisable()
        {
            inputSystem.MainMap.Console.performed -= Console_performed;
        }

        private void Console_performed(InputAction.CallbackContext obj)
        {
            consoleService.OpenCloseToggle();
        }
    }
}
