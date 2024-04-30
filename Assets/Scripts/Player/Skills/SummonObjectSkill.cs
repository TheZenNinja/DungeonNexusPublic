using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Skills
{
    public class SummonObjectSkill : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.hold_release;

        public int damage;
        public float damageInterval;
        public LayerMask raycastLayer;
        public LayerMask targetLayer;
        public float maxRange = 10;

        public bool objectIsOut => summonedObject != null;
        public float summonDuration;
        public float currentSummonLifetime;

        public SummonedObject summonPrefab;
        public GameObject extraSummon;

        public GameObject targetingVFX;
        
        [Space]

        public SummonedObject summonedObject;

        public override void Initialize(Entity caster)
        {
            targetingVFX.SetActive(false);
            summonPrefab.gameObject.SetActive(false);
        }

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;
            targetingVFX.SetActive(true);
        }

        public override void HoldSkill(Entity caster, float deltaTime)
        {
            if (IsOnCooldown)
                return;
            Ray ray = caster.GetSightRay();
            if (Physics.Raycast(ray, out RaycastHit hit, maxRange, raycastLayer))
            {
                targetingVFX.transform.position = hit.point;
                targetingVFX.transform.up = hit.normal;
            }
            else
            {
                targetingVFX.transform.position = ray.GetPoint(maxRange);
                targetingVFX.transform.up = Vector3.up;
            }

        }

        public override void ReleaseSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            Vector3 point = caster.GetSightRay().GetPoint(maxRange);

            if (Physics.Raycast(caster.GetSightRay(), out RaycastHit hit, maxRange, raycastLayer))
                point = hit.point;

            CreateObject(caster, point, hit.normal);

            audioSource.Play();

            targetingVFX.SetActive(false);
        }

        public void CreateObject(Entity caster, Vector3 position, Vector3 normal)
        {
            currentSummonLifetime = summonDuration;

            if (summonPrefab != null)
            {
                summonedObject = Instantiate(summonPrefab, position, Quaternion.identity);
                summonedObject.transform.up = normal;
                summonedObject.gameObject.SetActive(true);

                summonedObject.Intialize(caster, damage, damageInterval, targetLayer);
            }
            if (extraSummon != null)
            {
                var obj = Instantiate(extraSummon, position, Quaternion.identity);
                obj.SetActive(true);
            }

            StartCooldown();
        }

        protected override void TickCooldown(float deltaTime)
        {
            if (objectIsOut)
            {
                if (currentSummonLifetime <= 0)
                {
                    if (summonedObject != null)
                        Destroy(summonedObject.gameObject);
                }
                else
                    currentSummonLifetime -= deltaTime;
            }
            else
                base.TickCooldown(deltaTime);
        }
    }
}
