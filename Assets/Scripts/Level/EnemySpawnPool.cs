using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy Spawn Pool")]
public class EnemySpawnPool : ScriptableObject
{
    [SerializeField] private Dictionary<GameObject, int> enemyPool;
    public Dictionary<GameObject, int> GetEnemyPool() => enemyPool;
}
