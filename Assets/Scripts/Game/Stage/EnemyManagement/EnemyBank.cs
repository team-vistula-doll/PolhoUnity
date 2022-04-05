using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyEntry
{
    public string Name;
    public GameObject Prefab;
}
public class EnemyBank : MonoBehaviour
{
    [SerializeField]
    private List<EnemyEntry> DefinedEnemies;

    public Dictionary<string, GameObject> EnemyEntries;

    public void Start()
    {
        EnemyEntries = new Dictionary<string, GameObject>();
        foreach (var enemyEntry in DefinedEnemies)
        {
            EnemyEntries.Add(enemyEntry.Name,  enemyEntry.Prefab);
        }
    }

}
