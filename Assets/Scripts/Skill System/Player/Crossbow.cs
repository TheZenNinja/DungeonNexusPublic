using System;
using System.Collections;
using UnityEngine;
using Player;

namespace Skills
{
    public class Crossbow : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.channel;

        [SerializeField] float maxDistance = 100;
        [SerializeField] int damage = 10;
        [SerializeField] LayerMask ignoreMask;
        [SerializeField] GameObject crossbowPrefab;

        [Space]

        [SerializeField] GameObject crossbowInstance;
        [SerializeField] ParticleSystem vfx_crossbowShot;

        [Space]

        [SerializeField] Vector3 recoilDirection;
        [SerializeField] AnimationCurve recoilPositionCurve;
        [SerializeField] float recoilPositionMulti = 1;
        [SerializeField] float recoilAngle;
        [SerializeField] AnimationCurve recoilAngleCurve;

        public override void Initialize(Entity caster)
        {
            var hand = ((PlayerEntity)caster).GetHandTransform();

            crossbowInstance = Instantiate(crossbowPrefab, hand);
            crossbowInstance.SetActive(true);

            vfx_crossbowShot = crossbowInstance.GetComponentInChildren<ParticleSystem>();
        }
        public override void Deinitialize(Entity caster)
        {
            Destroy(crossbowInstance);
            crossbowInstance = null;
            vfx_crossbowShot = null;
        }

        public override void HoldSkill(Entity caster, float deltaTime)
        {
            if (!IsOnCooldown)
                Shoot(caster.GetSightRay(), caster);
        }
        private void Update()
        {
            crossbowInstance.transform.localPosition = recoilDirection.normalized * recoilPositionMulti * recoilPositionCurve.Evaluate(GetCooldownPercent);
            crossbowInstance.transform.localEulerAngles = Vector3.right * recoilAngle * recoilAngleCurve.Evaluate(GetCooldownPercent);
        }

        private void Shoot(Ray ray, Entity caster)
        {
            cooldownCurrent = cooldownMax;
            
            //Debug.Log("Shot crossbow");

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~ignoreMask))
            {
                //Debug.Log(hit.collider.gameObject.name);

                if (hit.collider.TryGetComponent(out Entity target))
                {
                    target.Health.TakeDamage(damage);
                    caster.OnDealDamage?.Invoke(target);
                }

            }
            audioSource.Play();
            vfx_crossbowShot.Play();
        }
    }
}