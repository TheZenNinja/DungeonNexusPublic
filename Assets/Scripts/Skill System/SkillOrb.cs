using Player;
using System.Collections;
using UnityEngine;
using Utils;

namespace Skills
{
    public class SkillOrb : MonoBehaviour, IInteractable
    {
        public SkillScriptableObject skill;
        public bool isInfinite = false;

        public System.Action onConsume;

        public string GetDescription()
        {
            return $"{skill.name}\n{skill.GetDescription()}";
        }

        public bool Interact(PlayerEntity player)
        {
            player.GetSkillController().ShowSkillSelectionMenu(skill, this);

            return true;
        }
        public void FinishSelection()
        {
            if (!isInfinite)
                Destroy(gameObject);
            onConsume?.Invoke();
        }
        public void SetSkill(SkillScriptableObject skillSO)
        {
            this.skill = skillSO;
        }
    }
}