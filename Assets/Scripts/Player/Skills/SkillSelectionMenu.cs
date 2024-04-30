using Player;
using System.Collections;
using UnityEngine;

namespace Skills.Player
{
    public class SkillSelectionMenu : MonoBehaviour
    {
        [SerializeField] GameObject UI;
        PlayerSkillController skillController;
        [SerializeField] SkillOrb currentSkillOrb;

        [SerializeField] SkillSelectionSlot newSkill;
        [SerializeField] SkillSelectionSlot primary;
        [SerializeField] SkillSelectionSlot secondary;
        [SerializeField] SkillSelectionSlot melee;
        [SerializeField] SkillSelectionSlot util;
        [SerializeField] SkillSelectionSlot[] others;


        private void Start()
        {
            skillController = GetComponent<PlayerSkillController>();

            primary.onClick += ClickSlot;
            secondary.onClick += ClickSlot;
            melee.onClick += ClickSlot;
            util.onClick += ClickSlot;

            for (int i = 0; i < others.Length; i++)
                others[i].onClick += ClickSlot;

            CloseUI();
        }


        public void ClickSlot(SkillSelectionSlot slot)
        {
            if (newSkill.GetSlotType() == slot.GetSlotType()) {
                skillController.ReplaceSkill(newSkill.skillSO, slot.GetSlotType(), slot.GetSlotIndex());

                if (currentSkillOrb != null)
                { 
                    currentSkillOrb.FinishSelection();
                    currentSkillOrb = null;
                }

                FindObjectOfType<GameSettingsController>().CloseSkillMenu();
                CloseUI();
            }
        }

        public void ShowMenu(SkillScriptableObject newSkillSO, SkillOrb orb)
        {
            this.currentSkillOrb = orb;
            UI.SetActive(true);
            newSkill.LoadSkill(newSkillSO);

            SkillSlotType hightlightType = newSkillSO.GetSlotType();

            primary.LoadSkill(skillController.GetSkill(SkillSlotType.primary).skillSO);
            primary.HighlightSlot(hightlightType);

            secondary.LoadSkill(skillController.GetSkill(SkillSlotType.secondary).skillSO);
            secondary.HighlightSlot(hightlightType);

            melee.LoadSkill(skillController.GetSkill(SkillSlotType.melee).skillSO);
            melee.HighlightSlot(hightlightType);

            util.LoadSkill(skillController.GetSkill(SkillSlotType.util).skillSO);
            util.HighlightSlot(hightlightType);

            for (int i = 0; i < others.Length; i++)
            {
                others[i].LoadSkill(skillController.GetSkill(SkillSlotType.other, i).skillSO);
                others[i].HighlightSlot(hightlightType);
            }
        }

        public void CloseUI()
        {
            UI.SetActive(false);
            currentSkillOrb = null;
        }
    }
}