using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Skills.Player
{
    public class SkillSelectionSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI nameTxt;
        [SerializeField] TextMeshProUGUI descTxt;
        [SerializeField] TextMeshProUGUI slotTxt;
        [SerializeField] bool isClickable = true;
        [SerializeField] bool canChangeSlotType = false;
        [SerializeField] SkillSlotType slotType;
        public SkillSlotType GetSlotType() => slotType;
        [SerializeField] int slotIndex;
        public int GetSlotIndex() => slotIndex;
        [SerializeField] GameObject highlightObj;

        [Header("Dynamic")]
        public SkillScriptableObject skillSO;

        public Action<SkillSelectionSlot> onClick;

        public void LoadSkill(SkillScriptableObject skill)
        {
            if (skill == null)
            {
                icon.enabled = false;
                nameTxt.text = "No Skill";
                return;
            }

            skillSO = skill;
            icon.enabled = true;
            icon.sprite = skill.GetIcon();
            nameTxt.text = skill.name;
            //descTxt.text = skill.GetDescription();

            if (canChangeSlotType)
            {
                slotType = skill.GetSlotType();
                slotTxt.text = GetSlotTypeName(skill.GetSlotType());
            }
        }

        private string GetSlotTypeName(SkillSlotType slotType)
        {
            switch (slotType)
            {
                case SkillSlotType.primary:
                    return "Primary Skill";
                case SkillSlotType.secondary:
                    return "Secondary Skill";
                case SkillSlotType.melee:
                    return "Melee Skill";
                case SkillSlotType.util:
                    return "Utility";
                default:
                case SkillSlotType.other:
                    return "Ability";
            }
        }

        public void HighlightSlot(SkillSlotType slotTypeToHighlight) => highlightObj.SetActive(slotType == slotTypeToHighlight);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isClickable)
                return;

            onClick?.Invoke(this);
            Debug.Log("Clicked " + gameObject.name);
        }
    }
}