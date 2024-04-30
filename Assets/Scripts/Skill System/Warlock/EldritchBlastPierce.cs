using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills.Warlock
{
    public class EldritchBlastPierce : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.hold_release;

        [SerializeField] LayerMask ignoreLayer;
        [SerializeField] LayerMask terrainLayer;

        [SerializeField] float maxRange = 50;

        [SerializeField] float chargeTime;
        [SerializeField] bool isCharging;
        [SerializeField] bool readyToFire;

        [SerializeField] int damage = 10;
        [SerializeField] int spellLevel = 1;

        [Header("VFX")]
        [SerializeField] Vector3 lineStartOffset;
        [SerializeField] VisualEffect beamVFX;
        [SerializeField] VisualEffect chargeVFX;
        [SerializeField] AudioSource chargeSFX;
        [SerializeField] Transform handPos;

        public override void Initialize(Entity caster)
        {
            var player = caster as PlayerEntity;
            spellLevel = player.GetCantripLevel();
            player.OnLevelUp.AddListener((p) => spellLevel = p.GetCantripLevel());
            handPos = player.GetHandTransform();
        }

        public override void TapSkill(Entity caster)
        {
            if (isCharging || IsOnCooldown)
                return;
            StartCoroutine(SkillTrigger((PlayerEntity)caster));
            chargeVFX.Play();
            chargeSFX.Play();
            readyToFire = false;
        }

        public override void ReleaseSkill(Entity caster)
        {
            readyToFire = true;
        }
       
        IEnumerator UpdateParticlePos()
        { 
            while (isCharging)
            {
                chargeVFX.SetVector3("Position", handPos.TransformPoint(lineStartOffset));
                yield return null;
            }
        }
        IEnumerator SkillTrigger(PlayerEntity caster)
        {
            isCharging = true;
            StartCoroutine(UpdateParticlePos());

            yield return new WaitForSeconds(chargeTime);

            yield return new WaitUntil(() => readyToFire);

            chargeVFX.Stop();
            chargeSFX.Stop();

            isCharging = false;
            readyToFire = false;
            StartCooldown();
            Shoot(caster);
        }

        void Shoot(PlayerEntity caster)
        {
            audioSource.Play();
            beamVFX.Play();
            var hits = Physics.RaycastAll(caster.GetSightRay(), maxRange, ~ignoreLayer);

            Vector3 endpoint = caster.GetSightRay().GetPoint(maxRange);

            bool hasHitEntity = false;

            foreach (var hit in hits)
            {
                endpoint = hit.point;
                if (MyUtils.LayerIsInLayermask(terrainLayer, hit.collider.gameObject.layer))
                    break;

                if (TryGetComponent(out Entity entity))
                {
                    entity.Health.TakeDamage(damage * spellLevel);
                    hasHitEntity = true;
                }
            }

            if (hasHitEntity)
                caster.OnDealDamage?.Invoke(null);

            var fx = Instantiate(beamVFX, null);
            fx.SetVector3("Start", handPos.TransformPoint(lineStartOffset));
            fx.SetVector3("End", endpoint);
            fx.Play();
            Destroy(fx.gameObject, 3);
        }
    }
}
