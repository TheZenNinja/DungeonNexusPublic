using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;

namespace Player
{
    public class PlayerEntity : Entity
    {
        [TabGroup("Callbacks")]
        public UnityEvent<bool> OnLookLockChanged;
        [TabGroup("Callbacks")]
        public UnityEvent<PlayerEntity> OnLevelUp;
        [TabGroup("Callbacks")]
        public UnityEvent<int> OnFloorClear;

        [Space]

        //[SerializeField] int[] expThresholds;
        //public int EXP { get; protected set; }

        [SerializeField] int maxHPOnLvlUp = 10;

        PlayerSkillController skillController;

        [Space]

        [SerializeField] Animation uiNotifAnim;
        [SerializeField] TMPro.TextMeshProUGUI uiNotifTxt;

        [SerializeField] AudioSource getHitSFX;

        public void Start()
        {
            var pm = GetComponent<PlayerMovement>();
            OnMoveLockChanged.AddListener(x => pm.LockControls(x));

            var cam = GetComponent<PlayerCamera>();
            OnLookLockChanged.AddListener(x => cam.LockControls(x));

            skillController = GetComponentInChildren<PlayerSkillController>();
            OnMoveLockChanged.AddListener(x => skillController.SetControlsLocked(x));

            //SetExp(0);

            //OnLevelUp.AddListener((_) => Debug.Log("Player Leveled up to: " + level));
            OnFloorClear.AddListener((_) =>
            {
                if (Health.GetMaxHP < Health.GetMaxHP + maxHPOnLvlUp * GetLevel())
                {
                    Health.SetMaxHP(Health.GetMaxHP + maxHPOnLvlUp);
                    Health.AddHealth(maxHPOnLvlUp);
                    uiNotifTxt.text = $"Level up: {GetLevel()}\nDmg+ HP+";
                    uiNotifAnim.Play();
                }
            });
            //OnLevelUp.AddListener((p) =>
            //{
            //    levelUpTxt.text = $"Level Up: {p.level}";
            //    levelUpUI_anim.Play();
            //});
            OnFloorClear.AddListener((f) =>
            {
                uiNotifTxt.text = $"Cleared Floor {f}";
                uiNotifAnim.Play();
            });

            Health.onDie.AddListener((_) => SessionDataManager.instance.PlayerDeath());

            Health.onTakeDamageToHealth.AddListener((_) => getHitSFX.Play());
        }

        public bool IsLookLocked { get; protected set; }
        public void SetLookLock(bool isLocked)
        {
            OnLookLockChanged?.Invoke(isLocked);
            IsLookLocked = isLocked;
        }
        public void SetLockAllInput(bool lockInput)
        {
            SetLookLock(lockInput);
            SetMoveLock(lockInput);
        }
        //[Button]
        //private void AddPlayerExp() => AddExp(100);

        //public void SetExp(int exp) 
        //{ 
        //    this.EXP = exp;
        //    level = GetLevel();
        //    OnLevelUp?.Invoke(this);
        //}
        //public void AddExp(int exp)
        //{
        //    this.EXP += exp;
        //    var newLvl = GetLevel();
        //    if (newLvl > level)
        //    {
        //        level = newLvl;
        //        OnLevelUp?.Invoke(this);
        //    }
        //}

        protected int GetLevel()
        {
            int currentFloor = SessionDataManager.instance.floor;

            if (currentFloor < 3)
                return 1;
            if (currentFloor < 9)
                return 2;
            if (currentFloor < 21)
                return 3;
            if (currentFloor < 33)
                return 4;

            return 5;
        }

        public override int GetSpellLevel()
        {
            var lvl = this.level;
            if (lvl >= 10)
                return 3;
            else if (lvl >= 5)
                return 2;

            return 1;
        }

        public int GetCantripLevel()
        {
            var lvl = this.level;
            if (lvl >= 10)
                return 3;
            else if (lvl  >= 5)
                return 2;

            return 1;
        }

        public PlayerSkillController GetSkillController() => skillController;


        public void TriggerUI_Notif(string text)
        {
            uiNotifTxt.text = text;
            uiNotifAnim.Play();
        }
    }
}