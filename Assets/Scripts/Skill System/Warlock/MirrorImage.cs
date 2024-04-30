using DG.Tweening;
using Skills;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Skills.Warlock
{
    public class MirrorImage : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        [SerializeField] int mirrorsLeft = 0;
        [SerializeField] AudioSource onHitSound;
        [SerializeField] Volume screenEffect;

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            caster.Health.onTakeDamage.AddListener(OnCasterHit);
            caster.Health.SetInvulnerable(true);
            mirrorsLeft = 3;
            StartCooldown();
            audioSource.Play();

            TriggerScreenEffect(.5f);
        }
        private void TriggerScreenEffect(float endWeight)
        {
            screenEffect.weight = 1;
            DOTween.To(() => screenEffect.weight, x => screenEffect.weight = x, 1, .4f);
        }

        public void OnCasterHit(Health health)
        {
            mirrorsLeft--;
            onHitSound.Play();

            if (mirrorsLeft <= 0)
            {
                health.SetInvulnerable(false);
                health.onTakeDamage.RemoveListener(OnCasterHit);
                TriggerScreenEffect(0);

            }
            else
                TriggerScreenEffect(.5f);
        }

        protected override void TickCooldown(float deltaTime)
        {
            if (mirrorsLeft > 0)
                return;
            base.TickCooldown(deltaTime);
        }
    }
}
