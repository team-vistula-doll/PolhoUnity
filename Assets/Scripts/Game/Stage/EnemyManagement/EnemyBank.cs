using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyEntry
{
    public string Name;
    public int Difficulty;
    public GameObject Prefab;
}
public class EnemyBank : MonoBehaviour
{
    [SerializeField]
    private List<EnemyEntry> DefinedEnemies;

    public Dictionary<Tuple<string, int>, GameObject> EnemyEntries;

    public void Start()
    {
        EnemyEntries = new Dictionary<Tuple<string, int>, GameObject>();
        foreach (var enemyEntry in DefinedEnemies)
        {
            EnemyEntries.Add(new Tuple<string, int>(enemyEntry.Name, enemyEntry.Difficulty),  enemyEntry.Prefab);
        }
    }

}
