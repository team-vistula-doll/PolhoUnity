using System;
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
    public Dictionary<string, GameObject> EnemyEntries;
    
    [SerializeField]
    private List<EnemyEntry> definedEnemies;

    public void Start()
    {
        EnemyEntries = new Dictionary<string, GameObject>();
        definedEnemies.ForEach(e => EnemyEntries.Add(e.Name, e.Prefab));
    }
}
