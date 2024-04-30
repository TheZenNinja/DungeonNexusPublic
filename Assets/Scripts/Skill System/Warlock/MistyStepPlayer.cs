using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Player;

namespace Skills.Warlock
{
    public class MistyStepPlayer : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.tap;

        [Space]
        [SerializeField] float moveDuration = .1f;
        [SerializeField] float moveDelay = .3f;
        [SerializeField] float invulnDuration = .3f;
        [SerializeField] float distance = 12;

        PlayerMovement playerMove;
        //remove later
        CharacterController cc;
        [Header("Visuals")]
        [SerializeField] float visualDuration = 1f;
        [SerializeField] Volume overlayVolume;
        [SerializeField] AnimationCurve visualBlend;

        public override void Initialize(Entity caster)
        {
            playerMove = caster.GetComponent<PlayerMovement>();
            cc = caster.GetComponent<CharacterController>();
        }
        
        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;

            StartCooldown();

            Vector3 dir = playerMove.GetInputVector();
            //int iterations = Mathf.RoundToInt(moveDuration / Time.fixedDeltaTime);
            Vector3 vel = dir.normalized * distance / moveDuration;

            audioSource.Play();

            StartCoroutine(DashRoutine());
            IEnumerator DashRoutine()
            {
                yield return new WaitForSeconds(moveDelay);
                caster.SetMoveLock(true);

                float grav = playerMove.gravity;
                playerMove.gravity = 0;
                playerMove.velocity = Vector3.zero;
                float time = 0;
                while (time < moveDuration)
                {
                    cc.Move(vel * Time.deltaTime);
                    time += Time.deltaTime;
                    yield return 0;
                }

                playerMove.gravity = grav;


                caster.SetMoveLock(false);
            }

            StartCoroutine(Invuln());
            IEnumerator Invuln()
            {
                //have specific invuln for dashing and parry?
                caster.Health.SetInvulnerable(true);
                yield return new WaitForSeconds(invulnDuration);
                caster.Health.SetInvulnerable(false);
            }

            StartCoroutine(Visuals());
            IEnumerator Visuals()
            {
                overlayVolume.enabled = true;
                float time = visualDuration;
                while (time > 0)
                {
                    overlayVolume.weight = visualBlend.Evaluate(visualDuration - time);
                    time -= Time.deltaTime;
                    yield return 0;
                }
                overlayVolume.enabled = false;
            }
        }
    }
}