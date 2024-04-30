using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class SkillCooldownUI : MonoBehaviour
    {
        [SerializeField] Image background;
        [SerializeField] Image icon;
        [SerializeField] Image overlay;

        public void SetCooldown(float cooldownPercent)
        {
            if (cooldownPercent < 0 || cooldownPercent > 1)
            {
                Debug.LogWarning("Cooldown must be between 0 and 1");
                cooldownPercent = Mathf.Clamp01(cooldownPercent);
            }

            overlay.enabled = cooldownPercent > 0;
            overlay.transform.localScale = new Vector3(1, cooldownPercent, 1);
        }

        public void SetIcon(Sprite sprite)
        {
            if (icon != null)
            {
                icon.enabled = true;
                icon.sprite = sprite;
            }
        }

        public void SetEmpty()
        {
            icon.enabled = false;
            SetCooldown(0);
        }

        private void OnValidate() => SetCooldown(1f / 3);
    }
}
