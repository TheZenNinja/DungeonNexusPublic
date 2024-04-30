using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Health))]
public class Entity : MonoBehaviour
{
    protected Health _health;
    public Health Health { get
        { 
            if (_health == null)
                _health = GetComponent<Health>();
            return _health;
        }
    }

    [TabGroup("References")]
    [SerializeField] Transform lookDirTransform;

    [TabGroup("Callbacks")]
    public UnityEvent<bool> OnMoveLockChanged;
    [TabGroup("Callbacks")]
    public UnityEvent<Entity> OnDealDamage;

    public Vector3 centerOffset;

    [SerializeField] protected Transform handTransform;

    public int level { get; protected set; }

    public bool IsMovementLocked { get; protected set; }
    public virtual void SetMoveLock(bool isLocked)
    {
        OnMoveLockChanged?.Invoke(isLocked);
        IsMovementLocked = isLocked;
    }

    public Vector3 position => transform.position;

    public Vector3 centerPosition => transform.TransformPoint(centerOffset);

    //public bool IsAllInputLocked { get; protected set; }
    //public virtual void SetLockedAllInput(bool lockInput) => IsAllInputLocked = lockInput;

    public virtual Vector3 GetLookDirection() => lookDirTransform.forward;

    public virtual Ray GetSightRay() => new Ray(lookDirTransform.position, lookDirTransform.forward);

    public Transform GetHandTransform() => handTransform;

    public virtual int GetSpellLevel() => 1;

    public int GetSpellDamage(int baseDmg, int spellLevel = 1)
    {
        var dmgMulti = 1 + (spellLevel - GetSpellLevel());
        return baseDmg * dmgMulti;
    }

}