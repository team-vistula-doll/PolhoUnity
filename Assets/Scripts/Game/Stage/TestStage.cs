using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;
[CreateAssetMenu(fileName = "TestStage",menuName = "Stage/TestStage",order=1)]
public class TestStage : Stage
{
    public GameObject enemy1;
    public override IEnumerator StageScript(StageArgs args)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            args.EnemyManager.SpawnEnemy("enemy2", 
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, 0), 20, "-x", 45));
            yield return new WaitForSeconds(0.5f);
            args.EnemyManager.SpawnEnemy("enemy1",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(5, 5), 20, "-x", -90));
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);
        Debug.Log("END! Good Job!");
    }
}
