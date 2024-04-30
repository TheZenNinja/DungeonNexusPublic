using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Skills
{
    public enum SkillActivationType
    {
        tap, channel, hold_release
    }
    // Wish it could be an interface, but it doesnt show in the inspector for testing
    // public interface ISkill
    public abstract class SkillBase : MonoBehaviour
    {
        public abstract SkillActivationType activationType { get; }
        public virtual float GetCooldownPercent => Mathf.Clamp01(cooldownCurrent / cooldownMax);
        public virtual bool IsOnCooldown => cooldownCurrent > 0;

        [BoxGroup("Core")]
        [SerializeField] protected float cooldownMax = 1;
        [BoxGroup("Core")]
        [SerializeField] protected float cooldownCurrent;
        [BoxGroup("Core")]
        [SerializeField] protected AudioSource audioSource;

        protected virtual void FixedUpdate() => TickCooldown(Time.fixedDeltaTime);
        protected virtual void TickCooldown(float deltaTime)
        {
            if (cooldownCurrent > 0)
                cooldownCurrent -= deltaTime;
        }

        public virtual void Initialize(Entity caster) { }
        public virtual void Deinitialize(Entity caster) { }

        public virtual void TapSkill(Entity caster) { }
        public virtual void HoldSkill(Entity caster, float deltaTime) { }
        public virtual void ReleaseSkill(Entity caster) { }

        public virtual void StartCooldown(float multiplier) => cooldownCurrent = cooldownMax = multiplier;
        public virtual void StartCooldown() => cooldownCurrent = cooldownMax;

        //public static string SkillID_ToName(SkillIDs id) => System.Enum.GetName(typeof(SkillIDs), id);
    }
}