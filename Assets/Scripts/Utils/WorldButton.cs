using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class WorldButton : MonoBehaviour, IInteractable
    {
        [SerializeField] string description;
        public UnityEvent<PlayerEntity> OnButtonPress;

        public string GetDescription() => description;

        public bool Interact(PlayerEntity player)
        {
            OnButtonPress?.Invoke(player);
            return true;
        }
    }
}