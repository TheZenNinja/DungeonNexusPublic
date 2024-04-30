using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills.Warlock
{
    public class EldritchBlastMulti : MultiTargetSkillBase
    {
        [SerializeField] float delayBetweenShots = .1f;
        [SerializeField] int damage = 10;
        [SerializeField] LayerMask terrainLayer;

        [Header("VFX")]
        [SerializeField] Vector3 lineStartOffset;
        [SerializeField] VisualEffect beamVFX;

        public override void Initialize(Entity caster)
        {
            base.Initialize(caster);
            var player = caster as PlayerEntity;
            maxTargets = player.GetCantripLevel();
            player.OnLevelUp.AddListener((p) => maxTargets = p.GetCantripLevel());
        }

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;
            base.TapSkill(caster);
            PlayerSkillController.instance.SetTargetReticleVisible(true);
        }

        public override void ReleaseSkill(Entity caster)
        {
            PlayerSkillController.instance.SetTargetReticleVisible(false);
            if (!hasTargets)
                return;

            StartCoroutine(AttackTargets(caster, targets.ToArray(), ((PlayerEntity)caster).GetHandTransform().TransformPoint(lineStartOffset)));
            StartCooldown();
            ClearLists();
        }

        public IEnumerator AttackTargets(Entity caster, Entity[] targets, Vector3 handPos)
        {
            foreach (Entity target in targets)
            {
                bool isInLightOfSight = true;
                Vector3 endPos = target.centerPosition;
                //check line of sight
                if (Physics.Raycast(handPos, target.centerPosition - handPos, out RaycastHit hit, Vector3.Distance(target.centerPosition, handPos), terrainLayer))
                {
                    isInLightOfSight = false;
                    endPos = hit.point;
                }

                audioSource.Play();

                if (isInLightOfSight)
                {
                    target.Health.TakeDamage(damage);
                    caster.OnDealDamage?.Invoke(target);
                }

                var fx = Instantiate(beamVFX, null);
                fx.SetVector3("Start", handPos);
                fx.SetVector3("End", endPos);
                fx.Play();
                Destroy(fx.gameObject, 3);

                //StartCoroutine(CreateTestRay(handPos, target.centerPosition));
                yield return new WaitForSeconds(delayBetweenShots);
            }
        }
    }
}
