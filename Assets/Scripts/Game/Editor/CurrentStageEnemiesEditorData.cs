#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class CurrentStageEnemiesEditorData : ScriptableObject
{
    //public GameObject EnemyPrefab;
    public List<bool> Foldouts = new();
    public int FoldedOut = -1;
    //public Vector2 EnemySpawnPosition;
    //public Vector2 EnemyScale;
    //public Sprite EnemySprite;
    public int IDIncrement = 0;
}
#endif //UNITY_EDITOR