using Player;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

namespace Skills
{
    public class SummonedObject : MonoBehaviour
    {
        [SerializeField] int damage;
        [SerializeField] float dmgTickDelay;
        [SerializeField] LayerMask targetLayer;
        [SerializeField] Vector3 offset;
        [SerializeField] float radius;
        [SerializeField] bool isSquare;
        [SerializeField] Vector3 boxSize;

        System.Action onHit;

        public void Intialize(Entity caster, int damage, float dmgTickDelay, LayerMask targetLayer)
        {
            this.damage = damage;
            this.dmgTickDelay = dmgTickDelay;
            this.targetLayer = targetLayer;

            onHit += () => caster.OnDealDamage?.Invoke(null);

            StartCoroutine(DamageTick());
        }
        IEnumerator DamageTick()
        {
            while (true)
            {
                Collider[] cols;

                if (isSquare)
                    cols = Physics.OverlapBox(transform.TransformPoint(offset), boxSize, transform.rotation, targetLayer);
                else
                    cols = Physics.OverlapSphere(transform.TransformPoint(offset), radius, targetLayer);

               // bool hasHitEnemy = false;

                foreach (var c in cols)
                    if (c.gameObject.TryGetComponent(out Entity entity))
                        if (entity.GetType() != typeof(PlayerEntity))
                        {
                            DamageTarget(entity);
                            //entity.Health.TakeDamage(damage);
                            //hasHitEnemy = true;
                        }

                //if (hasHitEnemy)
                //    onHit.Invoke();
                
                yield return new WaitForSeconds(dmgTickDelay);
            }
        }

        public void DamageTarget(Entity entity)
        {
            //Debug.Log("Hit " + entity.gameObject.name);
            entity.Health.TakeDamage(damage);
                    onHit.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Entity target))
                if (target.GetType() != typeof(PlayerEntity))
                        DamageTarget(target);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (isSquare)
                Gizmos.DrawWireCube(transform.TransformPoint(offset), boxSize);
            else
                Gizmos.DrawWireSphere(transform.TransformPoint(offset), radius);
        }
    }
}
