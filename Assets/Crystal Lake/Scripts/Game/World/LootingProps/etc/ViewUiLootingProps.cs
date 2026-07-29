using Mirror;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MyProj
{
    [System.Serializable]
    public class SlotLooting
    {
        public InventorySlot inventorySlot;
        public Vector2 position;
        public bool isFree;

        public SlotLooting(InventorySlot inventorySlot, Vector2 pos, bool isFree = true)
        {
            this.inventorySlot = inventorySlot;
            this.position = pos;
            this.isFree = isFree;
        }
    }

    public class ViewUiLootingProps : MonoBehaviour, ILocalOnly, IExitHandler
    {
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameObject uiLooting;
        [SerializeField] private AllPartPlayer partPlayer;
        [SerializeField] private Button exitButton;

        [Title("Настройка сетки")]
        [SerializeField] private Transform parentSlots;
        [SerializeField] private InventorySlot inventorySlotPrefab;
        [SerializeField] private Vector2Int maxSizeGrid;

        private SlotLooting[] grid;
        private CharacterManager characterManager;
        private MouseLook mouseLook;
        private IInputReader inputReader;
        private Container container;
        private QuickSlotInventory quickSlotInventory;


        private void Awake()
        {
            itemDatabase.Initialize();
        }

        private void Start()
        {
            InitializeGrid();

            characterManager = partPlayer.Get<CharacterManager>();
            mouseLook = partPlayer.Get<MouseLook>();
            quickSlotInventory = partPlayer.Get<QuickSlotInventory>();
            inputReader = characterManager.GetComponent<IInputReader>();
        }

        private void OnEnable()
        {
            exitButton.onClick.AddListener(HideWindow);
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(HideWindow);
        }

        private void InitializeGrid()
        {
            grid = new SlotLooting[maxSizeGrid.x * maxSizeGrid.y];
            int index = 0;

            for (int y = 0; y < maxSizeGrid.y; y++)
            {
                for (int x = 0; x < maxSizeGrid.x; x++)
                {
                    var slot = Instantiate(inventorySlotPrefab, parentSlots);
                    grid[index++] = new SlotLooting(slot, new Vector2(x, y));
                }
            }
        }

        public void Exit()
        {
            HideWindow();
        }

        public void ShowGrid(byte countSlot, Container container, SyncList<SlotContainer> listItem)
        {
            this.container = container;
            SubscrabeActionClickSlot(container, quickSlotInventory.slots, true);

            foreach (var item in grid)
            {
                item.inventorySlot.ClearSlot();
            }

            uiLooting.SetActive(true);

            inputReader.SetActiveMap(MapInputSystem.UI);
            mouseLook.SetStateCursor(true);
            characterManager.SetExitHandler(this);

            for (int i = grid.Length - 1; i > 0; i--) 
            {
                grid[i].inventorySlot.gameObject.SetActive(false);
            }

            for (int i = 0; i < countSlot; i++) 
            {
                grid[i].inventorySlot.gameObject.SetActive(true);
            }

            for (int i = 0; i < listItem.Count; i++)
            {
                ItemScriptebleObject item = itemDatabase.Get(listItem[i].IdProp);

                grid[i].inventorySlot.SetItem(item);
            }
        }

        public void UpdateGrid()
        {
            foreach (var item in grid)
            {
                item.inventorySlot.ClearSlot();
            }
            List<SlotContainer> listItem = container.GetListItem();
            for (int i = 0; i < listItem.Count; i++)
            {
                ItemScriptebleObject item = itemDatabase.Get(listItem[i].IdProp);
                grid[i].inventorySlot.SetItem(item);
            }
        }

        public void LocalDissable()
        {
            this.enabled = false;
        }

        private void HideWindow()
        {
            if (container != null)
            {
                container.CmdSetIsOpen(false);
            }

            SubscrabeActionClickSlot(container, quickSlotInventory.slots, false);

            uiLooting.SetActive(false);

            inputReader.SetActiveMap(MapInputSystem.GAMEPLAY);
            mouseLook.SetStateCursor(false);
            characterManager.SetExitHandler(null);
        }

        private void SubscrabeActionClickSlot(Container cont, List<InventorySlot> slots, bool isSub)
        {
            foreach (var slot in slots)
            {
                if (isSub)
                    slot.OnClickSlot += TryAddPropInContainer;
                else
                    slot.OnClickSlot -= TryAddPropInContainer;
            }
        }

        private bool TryAddPropInContainer(ushort id)
        {
            if (!container.TryAddPropInContainer(id))
            {
                Debug.LogWarning("Не удолось добавить");
                return false;
            }

            UpdateGrid();
            Debug.LogWarning("Добавление прошло успешно");
            return true;
        }
    }
}
