using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Skills;
using Utils;
using Skills.Player;
using TMPro;

namespace Player
{
    public class PlayerSkillController : MonoBehaviour
    {
        public static PlayerSkillController instance;
        public static bool ArePlayerControlsLocked
        {
            get
            {
                if (instance == null)
                    return false;
                return instance.areControlsLocked;
            }
        }
        private void Awake() => instance = this;
        
        [Serializable]
        public class AbilityDrawer
        {
            public SkillScriptableObject skillSO;
            public InputActionReference input;
            public SkillCooldownUI cooldownUI;
            public SkillBase skill;

            //add unbinding
            public void BindControls(Entity entity)
            {
                if (skill == null || entity == null || input == null)
                {
                    Debug.LogError("Couldnt bind controls");
                    /*Debug.LogError("Couldnt bind controls:" +
                        $"\n\tSkill: " + skill == null ? "null" : "available" +
                        $"\n\tEntity: " + entity == null ? "null" : "available" +
                        $"\n\tInput: " + input == null ? "null" : "available");*/

                    return;
                }

                input.action.started += _ => CheckIfControlsAreLocked(ArePlayerControlsLocked, () => skill.TapSkill(entity));
                input.action.canceled += _ => CheckIfControlsAreLocked(ArePlayerControlsLocked, () => skill.ReleaseSkill(entity));
            }

            public void CheckIfControlsAreLocked(bool isLocked, Action action)
            {
                if (!isLocked)
                    action.Invoke();
            }

            public void Update(Entity entity, float deltaTime)
            {
                if (skillSO == null)
                    return;

                if (input.action.IsPressed() && !ArePlayerControlsLocked)
                    skill.HoldSkill(entity, deltaTime);

                if (cooldownUI != null)
                    cooldownUI.SetCooldown(skill.GetCooldownPercent);
            }

            public void SetSkill(SkillScriptableObject skill)
            {
                skillSO = skill;
                //cooldownIU.SetIcon(skill);
            }


            public void Initialize(Entity caster, Transform parent)
            {
                if (skillSO == null)
                {
                    if (cooldownUI != null)
                        cooldownUI.SetEmpty();
                    return;
                }

                skill = Instantiate(skillSO.GetPrefab(), parent);
                //skill.transform.SetLocalPositionAndRotation(Vector3.zero, Vector3.zero);

                if (cooldownUI != null)
                    cooldownUI.SetIcon(skillSO.GetIcon());

                BindControls(caster);
                skill.Initialize(caster);
            }
            public void Deinitialize(Entity caster)
            {
                if (skill == null)
                    return;

                skill.Deinitialize(caster);

                Destroy(skill.gameObject);
                skill = null;
            }
        }

        [Header("Vars")]
        [SerializeField] bool areControlsLocked;
        [Header("References")]
        [SerializeField] PlayerEntity player;
        [SerializeField] SkillSelectionMenu skillSelectionMenu;
        [SerializeField] GameObject targetReticle;
        [Space]
        [Header("Abilities")]
        
        [SerializeField] AbilityDrawer primaryAttack;
        [Space]
        [SerializeField] AbilityDrawer classAction;
        [Space]
        [SerializeField] AbilityDrawer melee;
        [Space]
        [SerializeField] AbilityDrawer defensive;
        [Space]
        [SerializeField] AbilityDrawer[] abilities;

        [Space]
        [Header("Interact")]
        [SerializeField] InputActionReference input_interact;
        [SerializeField] float interactDistance;
        [SerializeField] LayerMask interactLayer;
        [SerializeField] TextMeshProUGUI interactTxt;


        void Start()
        {
            areControlsLocked = false;

            SetTargetReticleVisible(false);
            LoadAbilities();
        }
        
        private void Update() => TryGetInteractable();
        void TryGetInteractable()
        {
            if (Physics.Raycast(player.GetSightRay(), out RaycastHit hit, interactDistance, interactLayer))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable i))
                {
                    interactTxt.enabled = true;
                    interactTxt.text = i.GetDescription();
                    if (input_interact.action.triggered)
                        i.Interact(player);
                }
                else
                    interactTxt.enabled = false;
            }
            else
                interactTxt.enabled = false;

        }

        private void FixedUpdate()
        {
            float time = Time.deltaTime;

            primaryAttack.Update(player, time);
            classAction.Update(player, time);
            melee.Update(player, time);
            defensive.Update(player, time);

            foreach (var a in abilities)
                a.Update(player, time);
        }

        public void ReloadAbilities()
        { 
            ResetAbilities();
            LoadAbilities();
        }
        public void ResetAbilities()
        {
            primaryAttack.Deinitialize(player);
            classAction.Deinitialize(player);
            melee.Deinitialize(player);
            defensive.Deinitialize(player);

            foreach (var a in abilities)
                a.Deinitialize(player);
        }
        public void LoadAbilities()
        {
            primaryAttack.Initialize(player, transform);
            classAction.Initialize(player, transform);
            melee.Initialize(player, transform);
            defensive.Initialize(player, transform);

            foreach (var a in abilities)
                a.Initialize(player, transform);
        }
            
        public void RemoveSkill(AbilityDrawer drawer)
        {
            drawer.skill.Deinitialize(player);

            Destroy(drawer.skill.gameObject);
            drawer.skill = null;
        }
        public void InstantiateSkill(AbilityDrawer drawer)
        {

        }
        public void RequipSkills()
        {

        }
        public void ReplaceSkill(SkillScriptableObject newSkill, SkillSlotType slotType, int slotIndex = 0) => ReplaceSkill(newSkill, GetSkill(slotType, slotIndex));
        public void ReplaceSkill(SkillScriptableObject newSkill, AbilityDrawer drawer)
        {
            drawer.Deinitialize(player);
            drawer.SetSkill(newSkill);
            drawer.Initialize(player, transform);
        }

        public void SetControlsLocked(bool areLocked) => areControlsLocked = areLocked;

        public void ShowSkillSelectionMenu(SkillScriptableObject newSkill, SkillOrb skillOrb)
        {
            skillSelectionMenu.ShowMenu(newSkill, skillOrb);

            player.SetLockAllInput(true);
            FindObjectOfType<GameSettingsController>().OpenSkillMenu();
        }
        public AbilityDrawer GetSkill(SkillSlotType slot, int index = 0)
        {
            switch (slot)
            {
                case SkillSlotType.primary:
                    return primaryAttack;
                case SkillSlotType.secondary:
                    return classAction;
                case SkillSlotType.melee:
                    return melee;
                case SkillSlotType.util:
                    return defensive;
                default:
                case SkillSlotType.other:
                    return abilities[index];
            }
        }

        public void SetTargetReticleVisible(bool isVisible) => targetReticle.SetActive(isVisible);
    }
}