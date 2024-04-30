using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Skills.Warlock
{
    public class EldritchArmor : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        public bool armorIsActive;

        public int damage = 10;
        public float radius = 3;

        public LayerMask ignoreMask;

        [SerializeField] AudioSource onHitSound;
        [SerializeField] ParticleSystem onHitVFX;
        [SerializeField] Volume screenEffect;
        System.Action onHit;

        public override void Initialize(Entity caster)
        {
            onHit += () => caster.OnDealDamage?.Invoke(null);
        }

        protected override void TickCooldown(float deltaTime)
        {
            if (armorIsActive)
                return;
            base.TickCooldown(deltaTime);
        }

        public override void TapSkill(Entity caster)
        {
            if (armorIsActive)
                return;
            if (IsOnCooldown)
                return;
            

            audioSource.Play();

            caster.Health.SetInvulnerable(true);

            armorIsActive = true;
            caster.Health.onHit.AddListener((h) => OnCasterHit(caster, h));
            DOTween.To(() => screenEffect.weight, x => screenEffect.weight = x, 1, .4f);
            StartCooldown();
        }

        private void OnCasterHit(Entity caster, Health health)
        {
            onHitSound.Play();
            armorIsActive = false;
            health.SetInvulnerable(false);
            health.onHit.RemoveListener((h) => OnCasterHit(caster, h));
            DamageTargets(caster, health.transform.position, radius);
            DOTween.To(() => screenEffect.weight, x => screenEffect.weight = x, 0, .2f);

        }

        void DamageTargets(Entity caster, Vector3 center, float range)
        {
            var targets = new List<Entity>();

            var cols = Physics.OverlapSphere(center, range, ~ignoreMask);

            foreach (var col in cols)
            {
                var e = col.GetComponentInParent<Entity>();
                if (e && !targets.Contains(e))
                    targets.Add(e);
            }

            if (targets.Count > 0)
            {
                foreach (var t in targets)
                    t.Health.TakeDamage(caster.GetSpellDamage(damage));

                onHit.Invoke();
            }
        }
    }
}
