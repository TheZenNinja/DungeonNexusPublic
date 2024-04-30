using UnityEngine;

namespace Skills.Warlock
{
    public class FireballProjectile : MonoBehaviour
    {
        public float speed;
        public float AoE_Range;
        public LayerMask hitLayer;
        public GameObject hitVFX;

        [Space]

        public int damage;

        System.Action onHit;

        public void Init(Entity caster, int damage, Vector3 direction)
        {
            this.damage = damage;
            GetComponent<Rigidbody>().velocity = direction * speed;
            onHit += () => caster.OnDealDamage?.Invoke(null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (MyUtils.LayerIsInLayermask(hitLayer, other.gameObject.layer))
            {
                HitStuff();
                Destroy(gameObject);
            }
        }
        public void HitStuff()
        {
            var cols = Physics.OverlapSphere(transform.position, AoE_Range, hitLayer);

            bool hitAnEntity = false;


            foreach (var col in cols)
                if (col.TryGetComponent(out Entity e))
                {
                    e.Health.TakeDamage(damage);
                    hitAnEntity = true;
                }

            if (hitAnEntity)
                onHit.Invoke();

            var g = Instantiate(hitVFX, transform.position, Quaternion.identity);
            g.SetActive(true);
            Destroy(g, 5);
            Debug.DrawLine(transform.position, transform.position + Vector3.up * AoE_Range, Color.red, 1);
        }
    }
}