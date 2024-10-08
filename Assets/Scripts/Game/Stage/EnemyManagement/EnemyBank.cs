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
    public Dictionary<Tuple<string, int>, GameObject> EnemyEntries;
    
    [SerializeField]
    private List<EnemyEntry> DefinedEnemies;

    public void Start()
    {
        EnemyEntries = new Dictionary<Tuple<string, int>, GameObject>();
        DefinedEnemies.ForEach(e => EnemyEntries.Add(new Tuple<string, int>(e.Name, e.Difficulty), e.Prefab));
    }
}
