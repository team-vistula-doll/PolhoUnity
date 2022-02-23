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
            args.EnemyManager.SpawnEnemy("enemy1",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, 5), 20, "-x", 0));
            yield return new WaitForSeconds(0.5f);
            args.EnemyManager.SpawnEnemy("enemy1",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, 5), 20, "-x", -90));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        for(int i = 4; i >= 0; i -= 2)
        {
            args.EnemyManager.SpawnEnemy("enemy2",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, i+0.5f), 20, "-x", 45));
            yield return new WaitForSeconds(0.5f);
            args.EnemyManager.SpawnEnemy("enemy2",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, i), 20, "-x", 225));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);
        for(int i = -2; i <= 2; i += 4)
        {
            args.EnemyManager.SpawnEnemy("enemy3",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(i, 6), 20, "sin(3*x)*0.5", -90));
            yield return new WaitForSeconds(0.5f);
        }

        args.EnemyManager.SpawnEnemy("enemy3",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 6), 20, "sin(3*x)*0.5", -90));
        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);
        Debug.Log("END! Good Job!");
    }
}
