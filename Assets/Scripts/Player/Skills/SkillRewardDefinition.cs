
using UnityEngine;

namespace Skills
{
    [CreateAssetMenu(menuName = "Skill Rewards File")]
    public class SkillRewardDefinition : ScriptableObject
    {
        [SerializeField] SkillScriptableObject[] commonRewards;
        [SerializeField] SkillScriptableObject[] uncommonRewards;
        [SerializeField] SkillScriptableObject[] rareRewards;

        public SkillScriptableObject GetCommonReward() =>  commonRewards.GetRandomItem();
        public SkillScriptableObject GetUncommonReward() =>  uncommonRewards.GetRandomItem();
        public SkillScriptableObject GetRareReward() => rareRewards.GetRandomItem();
    }
}
