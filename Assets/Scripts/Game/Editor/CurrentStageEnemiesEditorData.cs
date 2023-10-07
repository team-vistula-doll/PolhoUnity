using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

public class CurrentStageEnemiesEditorData : WaypointPathEditorData
{
    [SerializeReference]
    public List<SingleEnemyEditor> EnemyEditors = new();
    //public GameObject EnemyPrefab;
    public List<bool> Foldouts = new();
    public int FoldedOut = -1;
    //public Vector2 EnemySpawnPosition;
    //public Vector2 EnemyScale;
    //public Sprite EnemySprite;
    public int IDIncrement = 0;
}
