using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills.Warlock
{
    public class BurningHands : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        public LayerMask ignoreLayer;
        public LayerMask terrainLayer;
        public float range;
        [Range(0,180)]
        public float angle = 30;

        public float attackRange;
        public float attackRadius;


        public int damage = 20;

        public ParticleSystem vfx;

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            //var targets = GetDamageables2(caster.position + Vector3.up, caster.GetLookDirection().ZeroY());
            var targets = GetTargets(caster.GetSightRay());

            if (targets.Length > 0)
                caster.OnDealDamage?.Invoke(null);

            foreach (var target in targets)
            {
                Debug.Log(target.name);
                target.TakeDamage(caster.GetSpellDamage(damage, 1));
                caster.OnDealDamage?.Invoke(null);
            }
            StartCooldown();
            audioSource.Play();
            vfx.transform.position = caster.GetHandTransform().TransformPoint(Vector3.left);
            vfx.transform.forward = caster.GetHandTransform().forward;
            vfx.Play();
        }

        Health[] GetTargets(Ray ray)
        {
            List<Health> targets = new List<Health>();

            var cols = Physics.OverlapSphere(ray.GetPoint(attackRange), attackRadius, ~ignoreLayer);

            Debug.DrawLine(ray.GetPoint(attackRange), ray.GetPoint(attackRange) + Vector3.up * attackRadius, Color.red, 1f);

            foreach (var c in cols)
                if (c.gameObject.TryGetComponent<Health>(out Health hp))
                    if (!targets.Contains(hp))
                        targets.Add(hp);

            return targets.ToArray();
        }

        Health[] GetDamageables(Vector3 originPos, Vector3 lookDir)
        {
            List<Health> result = new List<Health>();

            
            
            var targets = Physics.OverlapSphere(originPos, range, ~ignoreLayer);
            foreach (var target in targets)
            {
                // dont think normalizing it is needed
                var dirToTarget = target.transform.position - originPos;
                dirToTarget.y = 0;
                
                Debug.DrawRay(target.transform.position, dirToTarget, Color.red, 1f);

                if (target.TryGetComponent(out Health hp))
                    if (!result.Contains(hp))
                        if (IsInCone(originPos, dirToTarget, lookDir))
                            if (IsInLineOfSight(originPos, dirToTarget))
                                result.Add(hp);
            }
                


            return result.ToArray();
        }
        Health[] GetDamageables2(Vector3 origin, Vector3 lookDir)
        {
            List<Health> result = new List<Health>();

            Collider[] targets = Physics.OverlapSphere(origin, range, ~ignoreLayer);

            foreach (var target in targets)
            {
                Vector3 dirToTarget = (target.transform.position - origin).ZeroY().normalized;

                if (target.TryGetComponent(out Health hp))
                    if (!result.Contains(hp))
                        if (Vector3.Angle(lookDir, dirToTarget) < angle)
                        {
                            float dstToTarget = Vector3.Distance(origin, target.transform.position);
                            if (!Physics.Raycast(origin, dirToTarget, dstToTarget, terrainLayer))
                                result.Add(hp);
                        }
            }

            return result.ToArray();
        }

        private bool IsInCone(Vector3 originPos, Vector3 dirToTarget, Vector3 lookDir)
        {
            float dotProduct = Vector3.Dot(lookDir, dirToTarget);
            float angleFromDot = (dotProduct - 1) * -90;

            return angleFromDot <= angle;
        }
        private bool IsInLineOfSight(Vector3 originPos, Vector3 dirToTarget)
        {
            return Physics.Raycast(originPos, dirToTarget, range, ~ignoreLayer);
        }
    }
}