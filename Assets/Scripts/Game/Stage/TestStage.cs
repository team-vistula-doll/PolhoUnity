using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;
[CreateAssetMenu(fileName = "TestStage",menuName = "Stage/TestStage",order=1)]
public class TestStage : Stage
{
    List<int> _enemyIDs = new List<int>();
    public override IEnumerator StageScript(StageArgs args)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy1",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, 5), 20, "-x", 0)));
            yield return new WaitForSeconds(0.5f);
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy1",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, 5), 20, "-x", -90)));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        for(int i = 4; i >= 0; i -= 2)
        {
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy2",
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, i+0.5f), 20, "-x", 45)));
            yield return new WaitForSeconds(0.5f);
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy2",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, i), 20, "-x", 225)));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);
        for(int i = -2; i <= 2; i += 4)
        {
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy3",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(i, 7), 20, "sin(3*x)*0.5", -90)));
            yield return new WaitForSeconds(0.5f);
        }

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("enemy3",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 20, "sin(3*x)*0.5", -90)));
        yield return new WaitForSeconds(6f);


        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy("boss",
                    WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 3.5f, "-x", -45)));
        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
