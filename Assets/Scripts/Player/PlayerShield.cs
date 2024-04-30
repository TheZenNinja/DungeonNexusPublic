using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Zen.Depr
{
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference input_block;

        public bool isBlocking;
        public bool isParrying;
        public float parryDuration = .3f;
        public float blockCooldown = 1;
        public float currentBlockCooldown = 0;

        private bool canBlock => currentBlockCooldown <= 0;

        private bool canParry;

        public bool isKeyLocked = false;

        void Start()
        {
            input_block.action.started += _ =>
            {
                isKeyLocked = !canBlock;

                if (isKeyLocked)
                    return;

                isBlocking = true;
                canParry = true;
            };
            input_block.action.canceled += _ =>
            {
                if (isKeyLocked)
                    return;

                EvaluateRelease();
            };

            input_block.action.performed += _ =>
            {
                if (isKeyLocked)
                    return;

                canParry = false;
            };
        }

        private void EvaluatePress(InputAction.CallbackContext obj)
        {
        }

        void FixedUpdate()
        {
            if (currentBlockCooldown > 0)
                currentBlockCooldown -= Time.fixedDeltaTime;
        }

        public void EvaluateRelease()
        {
            if (currentBlockCooldown > 0)
                return;

            if (canParry)
                StartCoroutine(Parry());

            currentBlockCooldown = blockCooldown;
            isBlocking = false;
            canParry = true;
        }

        private IEnumerator Parry()
        {
            isParrying = true;
            yield return new WaitForSeconds(parryDuration);
            isParrying = false;
        }
        public void OnParry()
        {
            currentBlockCooldown = 0;
        }
    }
}