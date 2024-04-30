using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills.Warlock
{
    public class EldritchBlast : SkillBase
    {
        //this one is for power shot and rapid fire
        public override SkillActivationType activationType => SkillActivationType.tap;

        [Range(1,5)]
        [SerializeField] int bolts = 1;
        [SerializeField] int damage = 25;
        [SerializeField] float maxDistance = 100;

        [SerializeField] float timeBetweenBolts = .1f;
        [SerializeField] LayerMask ignoreLayer;

        [Header("VFX")]
        [SerializeField] Vector3 lineStartOffset;
        [SerializeField] VisualEffect beamVFX;

        public override void Initialize(Entity caster)
        {
            var player = caster as PlayerEntity;
            bolts = player.GetCantripLevel();
            player.OnLevelUp.AddListener((p) => bolts = p.GetCantripLevel());
        }

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            StartCooldown();

            //cooldownCurrent = cooldownMax;

            if (bolts > 1)
                StartCoroutine(FireRoutine(caster));
            else
                FireBolt(caster, caster.GetSightRay());

            IEnumerator FireRoutine(Entity caster)
            {
                for (int i = 1; i <= bolts; i++)
                {
                    FireBolt(caster, caster.GetSightRay());
                    yield return new WaitForSeconds(timeBetweenBolts);
                }
            }
            void FireBolt(Entity caster, Ray ray)
            {
                Vector3 point = ray.GetPoint(maxDistance);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~ignoreLayer))
                {
                    if (hit.collider.TryGetComponent(out Health target))
                    {
                        target.TakeDamage(damage);
                        caster.OnDealDamage?.Invoke(null);
                    }

                    point = hit.point;
                }
                audioSource.Play();

                var fx = Instantiate(beamVFX, null);
                fx.SetVector3("Start", caster.GetHandTransform().TransformPoint(lineStartOffset));
                fx.SetVector3("End", point);
                fx.Play();
                Destroy(fx.gameObject, 3);
            }
        }
    }
}