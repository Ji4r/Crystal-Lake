using UnityEngine;

namespace MyProj
{
    public enum ItemType { 
        dafault, weapon 
    }

    [CreateAssetMenu(fileName = "Item", menuName = "SO/CreateItem/Item")]
    public class ItemScriptebleObject : ScriptableObject
    {
        [SerializeField] private string nameItem;
        [SerializeField] private string description;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
        [Header("Настройка предмета в руке")]
        [SerializeField] private int defaultLayer = 0; // default
        [SerializeField] private int setLayerOnHandle = 8; // Hand
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private Vector3 scale;

        public string NameItem { get => nameItem; }
        public string Description { get => description; }
        public GameObject Prefab { get => prefab; }
        public Sprite Icon { get => icon; }
        public int SetLayerOnHandle { get => setLayerOnHandle; }
        public int DefaultLayer { get => defaultLayer; }
        public Vector3 Position { get => position; }
        public Vector3 Rotation { get => rotation; }
        public Vector3 Scale { get => scale; }
    }
}
