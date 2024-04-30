using Player;

namespace Utils
{
    public interface IInteractable
    {
        public bool Interact(PlayerEntity player);
        public string GetDescription();
    }
}
