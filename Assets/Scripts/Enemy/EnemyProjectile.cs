using Player;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        Rigidbody rb;

        [SerializeField] int damage;

        [SerializeField] LayerMask playerLayer;
        [SerializeField] LayerMask terrainLayer;

        private void Awake() => rb = GetComponent<Rigidbody>();

        public void SetVeloctiy(Vector3 velocity)
        { 
            rb.velocity = velocity;
            transform.forward = velocity.normalized;
        }
        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (MyUtils.LayerIsInLayermask(playerLayer, other.gameObject.layer))
            {
                var player = other.GetComponentInParent<PlayerEntity>();
                player.Health.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (MyUtils.LayerIsInLayermask(terrainLayer, other.gameObject.layer))
            {
                Destroy(gameObject);
            }
        }
    }
}