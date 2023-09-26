using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

public class CurrentStageEnemiesEditorData : WaypointPathEditorData
{
    public GameObject EnemyPrefab;
    public List<bool> Foldouts = new();
    public int FoldedOut = -1;
    public Vector2 EnemySpawnPosition;
    public Vector2 EnemyScale;
    public Sprite EnemySprite;
}
