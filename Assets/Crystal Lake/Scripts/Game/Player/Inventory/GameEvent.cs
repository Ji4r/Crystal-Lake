using UnityEngine;
using UnityEngine.Events;

namespace MyProj
{
    [CreateAssetMenu(fileName = "New GameEvent", menuName = "SO/Events/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        public UnityEvent<ItemScriptebleObject, GameObject> OnEventRaised = new UnityEvent<ItemScriptebleObject, GameObject>();

        public void Raise(ItemScriptebleObject item, GameObject obj)
        {
            OnEventRaised?.Invoke(item, obj);
        }
    }
}
