using Assets.Scripts.Level;
using Player;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyDirector : MonoBehaviour
    {
        public enum SpawningPhase
        {
            prep,
            spawning,
            complete,
        }
        [Serializable]
        public struct SpawnData
        {
            public List<EnemyBase> enemiesInWave;
        }

        public static EnemyDirector instance;
        private void Awake() => instance = this;


        [TabGroup("Targeting")]
        public Player.PlayerEntity target;

        [TabGroup("Spawning")]
        [SerializeField] Transform[] spawnPositions;
        [BoxGroup("Spawning")]
        [SerializeField] EnemyBase[] aggrEnemyPrefab;
        [BoxGroup("Spawning")]
        [SerializeField] EnemyBase[] supportEnemyPrefab;
        [TabGroup("Spawning")]
        [SerializeField] BossAI bossPrefab;
        [TabGroup("Spawning")]
        [SerializeField] Transform bossSpawnPos;
        [TabGroup("Spawning")]
        [SerializeField] Transform bossWaypointParent;

        public ParticleSystem spawnParticlePrefab;
        [Space]

        [SerializeField] private SpawningPhase phase;
        [SerializeField] List<SpawnData> waveData;
        [SerializeField] List<EnemyBase> currentEnemies;
        [SerializeField] List<EnemyBase> supportEnemies;
        [SerializeField] public Vector3 centerOfEnemies { get; private set; }

        void Start()
        {
            phase = SpawningPhase.prep;
        }

        public void InitializeBoss(int health, int damage, Entity player)
        {
            var boss = Instantiate(bossPrefab, bossSpawnPos.position, bossSpawnPos.rotation);

            boss.SetStats(health, damage);
            boss.StartMovement(bossWaypointParent);
            boss.SetTarget(player);
            currentEnemies.Add(boss);
            boss.entity.Health.onDie.AddListener((_) => RemoveEnemy(boss));
            boss.entity.Health.onDie.AddListener((_) => FindObjectOfType<LevelInfo>().ClearRoom());
        }

        public void Initialize(int[] aggrEnemyWaves, int[] supportEnemyWaves, PlayerEntity player)
        {
            this.target = player;
            waveData = new List<SpawnData>();
            for (int i = 0; i < aggrEnemyWaves.Length; i++)
            {
                var data = new SpawnData();

                data.enemiesInWave = new List<EnemyBase>();
                if (aggrEnemyWaves[i] > 0)
                    data.enemiesInWave.AddRange(aggrEnemyPrefab.GetRandomListFromPool(aggrEnemyWaves[i]));

                if (supportEnemyWaves[i] > 0)
                    data.enemiesInWave.AddRange(supportEnemyPrefab.GetRandomListFromPool(supportEnemyWaves[i]));

                waveData.Add(data);
            }
            StartSpawningWaves();
        }

        private void Update()
        {
            if (currentEnemies.Count > 0)
            {
                Vector3 total = Vector3.zero;
                foreach (var e in currentEnemies)
                    if (e != null)
                        total += e.transform.position;

                centerOfEnemies = total / currentEnemies.Count;
            }
        }

        [Button("Start Waves")]
        public void StartSpawningWaves()
        {
            if (phase == SpawningPhase.spawning)
                return;
            StartCoroutine(SpawnRoutine());
        }


        IEnumerator SpawnRoutine()
        {
            phase = SpawningPhase.spawning;
            yield return new WaitForSeconds(1);

            foreach (var wave in waveData)
            {
                SpawnWave(wave.enemiesInWave);

                yield return new WaitWhile(() => currentEnemies.Count > 0);
                yield return new WaitForSeconds(3);
            }

            FindObjectOfType<LevelInfo>().ClearRoom();

            phase = SpawningPhase.complete;
        }

        private void SpawnWave(List<EnemyBase> enemies)
        {
            var points = spawnPositions.GetRandomListFromPool(enemies.Count);
            
            for (int i = 0; i < points.Count; i++)
            {
                var e = Instantiate(enemies[i], points[i].position, Quaternion.identity);
                AddEnemy(e, e.GetType() == typeof(SupportEnemy));
                Instantiate(spawnParticlePrefab, points[i].position, Quaternion.identity);
            }
        }

        public void AddEnemy(EnemyBase enemy, bool isSupport)
        {
            enemy.SetStats(
                SessionDataManager.instance.GetEnemyHealth(SessionDataManager.instance.floor),
                SessionDataManager.instance.GetEnemyDamage(SessionDataManager.instance.floor)
                );
            enemy.SetTarget(target);
            
            if (isSupport)
                supportEnemies.Add(enemy);
            else
                currentEnemies.Add(enemy);

            //removes enemy from list of enemies on death
            enemy.entity.Health.onDie.AddListener((_) => RemoveEnemy(enemy));
        }

        public void RemoveEnemy(EnemyBase enemy)
        {
            currentEnemies.Remove(enemy);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(centerOfEnemies, .5f);
        }

        
    }
}