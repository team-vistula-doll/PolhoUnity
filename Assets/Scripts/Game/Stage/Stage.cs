using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StageArgs
{
    public EnemyManager EnemyManager;
}
public abstract class Stage : ScriptableObject
{
    public abstract IEnumerator StageScript(StageArgs args);
}
