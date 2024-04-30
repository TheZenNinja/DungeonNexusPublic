using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class RangedEnemy : EnemyBase
    {
        enum AIState
        { 
            chase,
            wander,
        }
        NavMeshAgent agent;
        public Entity target;
        public Vector3 goalPos;


        [SerializeField] AIState state;

        public float rechaseDistance;
        public float wanderStartDistance;
        public float stopDistance = 3;

        [SerializeField] float debugRemainingDist;

        public float wanderInterval;
        public float wanderDist = 2;
        float wanderTimer;
        public float lookTowardsPlayerSpeed = 10;

        [Space]

        public Transform aimDirector;
        public Vector3 aimoffset;
        public float aimSpeed = 4;
        Vector3 currentAimDir;

        [Space]
        public EnemyProjectile projectilePrefab;
        //replace with particle
        public int projecitleDamage;
        public float projectileSpeed;

        [Space]
        [SerializeField] Animator anim;
        [SerializeField] ParticleSystem shootParticle;
        [SerializeField] Transform shootOrigin;
        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            state = AIState.chase;
            currentAimDir = aimDirector.forward;
            StartCoroutine(ShootRoutine());
            entity.Health.onDie.AddListener(_ => Kill());
            entity.Health.onTakeDamage.AddListener(_ => TriggerHealthbarFade());
        }

        void FixedUpdate()
        {
            if (target == null)
                return;



            Vector3 aimDir = (target.centerPosition - shootOrigin.position).normalized;

            currentAimDir = Vector3.Lerp(currentAimDir, aimDir, Time.fixedDeltaTime * aimSpeed);

            aimDirector.forward = currentAimDir;    

            //aimDirector.LookAt(target.position + aimoffset, Vector3.up);
            debugRemainingDist = agent.remainingDistance;

            if (state == AIState.chase)
            {

                if (Vector3.Distance(transform.position, goalPos) < stopDistance)
                    //state = AIState.wander;
                //if (agent.remainingDistance <= stopDistance)
                {
                    state = AIState.wander;
                    goalPos = transform.position;
                    agent.SetDestination(goalPos);
                    //agent.ResetPath();
                    StopAgent();
                }

                agent.stoppingDistance = stopDistance;
                goalPos = target.position + (transform.position - target.position).normalized * stopDistance;
                wanderTimer = 0;
            }
            else if (state == AIState.wander)
            {
                Wander();
            }
            agent.SetDestination(goalPos);


            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        public void Wander()
        {
            if (Vector3.Distance(transform.position, target.position) > rechaseDistance)
            {
                state = AIState.chase;
                agent.angularSpeed = 400;
                return;
            }
            if (agent.velocity.magnitude < .1f)
            {
                agent.angularSpeed = 0;

                var dirToTarget = (target.position.ZeroY() - transform.position.ZeroY()).normalized;
                float goalAngle = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;

                Debug.DrawRay(entity.centerPosition, dirToTarget, Color.magenta);

                var smoothAngle = Mathf.Lerp(transform.localEulerAngles.y, goalAngle, lookTowardsPlayerSpeed * Time.fixedDeltaTime);

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, smoothAngle, transform.localEulerAngles.z);
            }
            else
                agent.angularSpeed = 400;



            wanderTimer += Time.deltaTime;

            if (wanderTimer >= wanderInterval)
            {
                float angle = Random.Range(-.5f * Mathf.PI, .5f * Mathf.PI) + Mathf.Deg2Rad * target.transform.eulerAngles.y;
                //float angle = Random.Range(0, 2 * Mathf.PI);
                Vector3 testPos = goalPos + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * wanderDist;
                if (NavMesh.SamplePosition(testPos, out NavMeshHit hit, wanderDist / 2, NavMesh.AllAreas))
                {
                    if (Vector3.Distance(hit.position, target.position) < rechaseDistance)
                    {
                        goalPos = hit.position;
                        Debug.DrawLine(goalPos, goalPos + Vector3.up, Color.red, .5f);
                        wanderTimer = 0;
                    }
                }
            }
        }

        public override void SetStats(int hp, int dmg)
        {
            entity.Health.SetMaxHP(hp, true);
            projecitleDamage = dmg;
        }

        public override void SetTarget(Entity target)
        {
            this.target = target;
        }

        Vector3 GetPositionToPlayer(Vector3 center, Vector3 offset, float maxDist = 1)
        {
            int infiniteCheck = 20;
            while (true)
            {
                if (NavMesh.SamplePosition(center + offset, out NavMeshHit hit, wanderDist / 2, NavMesh.AllAreas))
                    return hit.position;

                infiniteCheck--;
                if (infiniteCheck < 0)
                    break;
            }
            return Vector3.zero;
        }

        public void Kill()
        {
            Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            if (target == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(goalPos, 0.25f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position + aimoffset, 0.25f);


            Gizmos.color = Color.gray;
            float angle = .5f * Mathf.PI + Mathf.Deg2Rad * target.transform.eulerAngles.y;
            Gizmos.DrawLine(target.position, target.position + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * wanderDist);
            Gizmos.DrawLine(target.position, target.position + new Vector3(Mathf.Sin(-angle), 0, Mathf.Cos(-angle)) * wanderDist);



            Gizmos.color = Color.blue;
            Gizmos.DrawLine(aimDirector.position, aimDirector.position + currentAimDir*3);
            Gizmos.color = Color.red;
            Vector3 aimDir = ((target.position + aimoffset) - transform.position).normalized;
            Gizmos.DrawLine(aimDirector.position, aimDirector.position + aimDir*3);

        }

        IEnumerator ShootRoutine()
        {
            while (true)
            {
                agent.isStopped = true;
                shootParticle.Play();
                yield return new WaitForSeconds(1);
                agent.isStopped = false;

                var p = Instantiate(projectilePrefab, shootOrigin.position + shootOrigin.forward, Quaternion.identity);
                p.gameObject.SetActive(true);
                p.SetVeloctiy(currentAimDir.normalized * projectileSpeed);
                p.SetDamage(projecitleDamage);

                yield return new WaitForSeconds(Random.Range(2f,4f));
            }

        }


        void StopAgent() => StartCoroutine(StopAgentRoutine());
        private IEnumerator StopAgentRoutine()
        {
            agent.isStopped = true;
            yield return new WaitForFixedUpdate();
            agent.isStopped = false;
        }
    }
}