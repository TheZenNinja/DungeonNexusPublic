using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] PlayerEntity player;

        [SerializeField] AudioSource audioSource;
        [SerializeField] Animation hitmarker;

        private void Start()
        {
            player.OnDealDamage.AddListener(OnDamageDealt);
        }

        [Button("Test")]
        void OnDamageDealt(Entity target)
        {
            audioSource.Play();
            hitmarker.Play();
        }
    }
}
