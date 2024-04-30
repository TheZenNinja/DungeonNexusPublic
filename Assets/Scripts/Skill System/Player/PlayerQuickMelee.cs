using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Skills.Player
{
    public class PlayerQuickMelee : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        [SerializeField] LayerMask ignoreMask;
        [SerializeField] int damage;
        public new Transform camera;
        [SerializeField] Vector3 hitboxOffset;
        public Vector3 GetHitboxOffset => hitboxOffset;
        [SerializeField] Vector3 hitboxSize;
        public Vector3 GetHitboxSize => hitboxSize;
        // add damage type later

        [SerializeField] Animator prefab;
        [SerializeField] Animator animObj;


        public override void Initialize(Entity caster)
        {
            var player = caster as PlayerEntity;
            camera = player.GetHandTransform();
            animObj = Instantiate(prefab, camera);
            animObj.gameObject.SetActive(true);
            animObj.transform.SetLocalPositionAndRotation(new Vector3(-.4f, .4f, 0), Quaternion.Euler(Vector3.zero));
        }

        public override void Deinitialize(Entity caster)
        {
            Destroy(animObj.gameObject);
        }

        static int animHash = Animator.StringToHash("Stab");

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            cooldownCurrent = cooldownMax;
            animObj.Play(animHash);
            audioSource.Play();

            Collider[] cols = Physics.OverlapBox(camera.TransformPoint(hitboxOffset), hitboxSize, camera.rotation, ~ignoreMask);
            var objs = new List<Entity>();
            
            foreach (var c in cols)
            {
                var e = c.GetComponentInParent<Entity>();
                
                if (e == null)
                    continue;
                if (objs.Contains(e))
                    continue;

                objs.Add(e);
                e.Health.TakeDamage(damage);
                caster.OnDealDamage?.Invoke(e);
            }
        }
    }
}