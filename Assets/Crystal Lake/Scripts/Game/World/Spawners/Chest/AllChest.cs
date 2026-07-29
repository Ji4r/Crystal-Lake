using TriInspector;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MyProj
{
    public class AllChest : MonoBehaviour
    {
        [SerializeField] List<Chest> ChestList = new();


        [Button("Get all chest")]
        private void GetAllChestWithScene()
        {
            if (ChestList == null)
                ChestList = new List<Chest>();

            ChestList.Clear();

            ChestList = FindObjectsByType<Chest>(FindObjectsSortMode.None).ToList();
        }

        public List<Chest> GetChests()
        {
            return ChestList;
        }
    }
}
