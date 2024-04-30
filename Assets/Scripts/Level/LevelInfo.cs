using Enemy;
using Player;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Level
{
    public class LevelInfo : MonoBehaviour
    {
        public Transform levelStartPosition;
        public Vector3 levelCenter;
        public float levelOutOfBoundsRange = 50;
        public PlayerEntity player;
        public EnemyDirector enemyDirector;
        public Transform bossWaypointsParent;
        public AudioSource completeSound;

        public GameObject returnButton;

        IEnumerator CheckTick()
        {
            CheckOutOfBounds();
            yield return new WaitForSeconds(1f);
        }

        void CheckOutOfBounds()
        {
            //player
            if (Vector3.Distance(player.position, levelCenter) > levelOutOfBoundsRange)
                player.transform.position = levelStartPosition.position;

            //enemies
            //if (enemyDirector.currentEnemies.Count > 0)
            //    foreach (var e in enemyDirector.currentEnemies)
            //        if (Vector3.Distance(e.position, levelCenter) > levelOutOfBoundsRange)
            //            e.Health.InstantKill();
        }

        public void Initialize()
        {
            this.player = FindObjectOfType<PlayerEntity>();
            player.transform.position = levelStartPosition.position;
            StartCoroutine(CheckTick());

            enemyDirector = FindObjectOfType<EnemyDirector>();

            if (SessionDataManager.instance.IsBossFloor())
            {
                enemyDirector.InitializeBoss(SessionDataManager.instance.GetBossHP(), 
                    SessionDataManager.instance.GetEnemyDamage(SessionDataManager.instance.floor) * 2,
                    player);
            }
            else
            {
                var aggrWaves = SessionDataManager.instance.GetAggressiveWaves();
                var supportWaves = SessionDataManager.instance.GetSupportWaves();

                for (int i = 0; i < aggrWaves.Length; i++)
                    Debug.Log(aggrWaves[i] + "\t" + supportWaves[i]);

                enemyDirector.Initialize(aggrWaves, supportWaves, player);
            }

            returnButton.SetActive(false);
        }

        public void ClearRoom()
        {
            int healAmt = Mathf.RoundToInt(.3f * player.Health.GetMaxHP);
            player.Health.AddHealth(healAmt);
            returnButton.SetActive(true);
            player.TriggerUI_Notif("Cleared Floor " + SessionDataManager.instance.floor);
            completeSound.Play();
        }

        public void ReturnToCamp()
        {
            var gsm = FindObjectOfType<GameSceneManager>();
            gsm.ReturnToCamp();
            SessionDataManager.instance.IncrFloor();
        }
    }
}