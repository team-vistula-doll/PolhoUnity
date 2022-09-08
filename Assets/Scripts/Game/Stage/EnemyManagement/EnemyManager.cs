using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyBank))]
public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies;
    private EnemyBank _enemyBank;
    
    void Start()
    {
        _enemies = new List<GameObject>();
        _enemyBank = GetComponent<EnemyBank>();
    }

    public void SpawnEnemy(string enemyName, List<Vector2> path)
    {
        GameObject enemy = Instantiate(_enemyBank.EnemyEntries[new Tuple<string, int>(enemyName, GameState.Difficulty)], path[0], Quaternion.identity, transform);
        enemy.GetComponent<WaypointWalker>().SetWaypointPath(path);
        _enemies.Add(enemy);
    }

    public bool NoEnemiesPresent()
    {
        return _enemies.All(e => !e.activeInHierarchy);
    }

    public void Clear()
    {
        _enemies.ForEach(Destroy);
        _enemies.Clear();
    }
}
