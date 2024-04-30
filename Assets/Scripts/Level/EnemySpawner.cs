using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawningPhase{ 
        prep,
        spawning,
        complete,
    }
    [Serializable]
    struct WaveData {
        public GameObject[] enemies;
    }

    [SerializeField] private WaveData[] waves;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private bool showPositions;

    [SerializeField] private int currentWave;
    [SerializeField] private SpawningPhase phase;

    [SerializeField] private List<GameObject> enemiesOnField;
    IEnumerator Start()
    {
        phase = SpawningPhase.prep;
        currentWave = 0;

        yield return new WaitForSeconds(5);
    }

    void Update()
    {
        
    }
    public void IteratePhase()
    {
        switch (phase)
        {
            case SpawningPhase.prep:
                SpawnWave(currentWave);
                break;
            case SpawningPhase.spawning:
            default:
                currentWave++;
                SpawnWave(currentWave);
                break;
            case SpawningPhase.complete:
                break;
        }

    }
    IEnumerator SpawnRoutine()
    {
        phase = SpawningPhase.spawning;
        
        for (int i = 0; i < waves.Length; i++)
        {
            SpawnWave(i);
            yield return new WaitWhile(() => enemiesOnField.Count > 0);
            yield return new WaitForSeconds(5);
        }

        phase = SpawningPhase.complete;
    }

    private void SpawnWave(int wave)
    {
        if (wave > waves.Length)
            throw new System.ArgumentOutOfRangeException();

        Debug.Log($"Spawning wave {wave}");

        var data = waves[wave];
        var points = spawnPositions.GetRandomListFromPool(data.enemies.Length);

        for (int i = 0; i < points.Count; i++)
        {
            var e = Instantiate(data.enemies[i], points[i]);
            enemiesOnField.Add(e);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (spawnPositions.Length > 0)
            foreach (var pos in spawnPositions)
                Gizmos.DrawSphere(pos.position, .1f);
    }
}
