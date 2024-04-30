using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Entity))]
    public abstract class EnemyBase : MonoBehaviour
    {
        public abstract void SetTarget(Entity target);

        public abstract void SetStats(int hp, int dmg);

        public Entity entity => GetComponent<Entity>();

        public Animation healthbarFadeAnim;

        public ParticleSystem deathParticlePrefab;

        protected void TriggerHealthbarFade()
        {
            if (healthbarFadeAnim != null) 
                healthbarFadeAnim.Play();
        }
    }
}
