using System.Collections;
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(menuName = "New Skill", fileName = "New Skill")]
    public class SkillScriptableObject : ScriptableObject
    {
        [SerializeField] SkillBase prefab;
        [SerializeField] Sprite icon;
        [SerializeField] SkillSlotType slotType = SkillSlotType.other;
        [TextArea(3, 10)]
        [SerializeField] string description;
        
        public SkillBase GetPrefab() => prefab;
        public Sprite GetIcon() => icon;
        public string GetDescription() => description;
        public SkillSlotType GetSlotType() => slotType;
    }
}