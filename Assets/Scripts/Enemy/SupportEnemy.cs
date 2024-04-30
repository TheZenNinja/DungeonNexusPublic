using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class SupportEnemy : EnemyBase
    {
        public enum SupportType
        {
            defender,
            buffer,
            healer,
        }

        [SerializeField] private SupportType supportType;
        [SerializeField] float supportCooldown;
        float currentSupportCooldown;
        [SerializeField] float supportRange = 10;
        [SerializeField] float supportAmount = 10;

        [SerializeField] LayerMask supportTargetLayer;

        NavMeshAgent agent;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            StartCoroutine(SetDestination());

            currentSupportCooldown = supportCooldown;
            entity.Health.onTakeDamage.AddListener(_ => TriggerHealthbarFade());
        }

        IEnumerator SetDestination()
        {
            while (true)
            {
                agent.SetDestination(EnemyDirector.instance.centerOfEnemies);
                yield return new WaitForSeconds(1);
            }
        }

        public override void SetTarget(Entity target) { }
        public override void SetStats(int hp, int dmg)
        {
            entity.Health.SetMaxHP(hp, true);
        }

        private void FixedUpdate()
        {
            if (currentSupportCooldown > 0)
                currentSupportCooldown -= Time.deltaTime;
            else
                TriggerSupportSkill();
        }

        void TriggerSupportSkill()
        {
            currentSupportCooldown = supportCooldown;

            switch (supportType)
            {
                case SupportType.defender:
                    break;
                case SupportType.buffer:
                    break;
                case SupportType.healer:
                default:
                    TriggerHeal();
                    break;
            }
        }

        void TriggerHeal()
        {
            foreach (var targets in GetTargets())
                targets.entity.Health.AddHealth(Mathf.RoundToInt(supportAmount));
        }

        EnemyBase[] GetTargets()
        {
            var cols = Physics.OverlapSphere(transform.position, supportRange, supportTargetLayer);

            var targets = new List<EnemyBase>();

            foreach (var c in cols)
                if (c.gameObject.TryGetComponent(out EnemyBase enemy))
                    if (enemy != this)
                        if (!targets.Contains(enemy))
                            targets.Add(enemy);

            return targets.ToArray();
        }
    }
}
