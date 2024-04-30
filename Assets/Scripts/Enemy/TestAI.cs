using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class TestAI : MonoBehaviour
    {
        public float sprintSpeed;
        public float walkSpeed;
        public float sprintDistance = 20;

        public float swingDist;
        public float swingDelay;
        float currentSwingDelay;
        public Animator anim;

        NavMeshAgent agent;
        public Transform goal;
        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            currentSwingDelay = 0;
        }

        private void FixedUpdate()
        {
            if (goal != null)
            {
                agent.SetDestination(goal.position);

                if (agent.remainingDistance >= sprintDistance)
                    agent.speed = sprintSpeed;
                else
                    agent.speed = walkSpeed;
            }

            if (Vector3.Distance(goal.position, transform.position) <= swingDist)
            {
                if  (currentSwingDelay <= 0)
                {
                    currentSwingDelay = swingDelay;
                    anim.SetTrigger("Swing");
                    Debug.Log("Swing");
                }
            }

            if (currentSwingDelay > 0)
                currentSwingDelay -= Time.fixedDeltaTime;
        }
        public void Die()
        {
            Destroy(gameObject);
        }

        public void HitThing(Health hp)
        {
            Debug.Log(hp.gameObject.name);
        }
    }
}