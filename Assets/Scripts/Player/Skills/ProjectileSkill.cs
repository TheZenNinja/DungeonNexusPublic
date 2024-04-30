using Skills;
using Skills.Warlock;
using UnityEngine;

namespace Skills
{
    public class ProjectileSkill : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        public int damage = 60;
        public LayerMask targetLayer;
        public FireballProjectile projectile;
        public Vector3 handOffset = Vector3.forward;


        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            Vector3 originPos = caster.GetHandTransform().TransformPoint(handOffset);

            Vector3 point;
            if (Physics.Raycast(caster.GetSightRay(), out RaycastHit hit, 100, targetLayer))
                point = hit.point;
            else
                point = caster.GetSightRay().GetPoint(100);

            Vector3 dirToTarget = (point - originPos).normalized;

            var proj = Instantiate(projectile, originPos, Quaternion.identity);
            proj.gameObject.SetActive(true);
            proj.Init(caster, caster.GetSpellDamage(damage, 3), dirToTarget);

            audioSource.Play();
            StartCooldown();
        }
    }
}
