using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStruct;
[CreateAssetMenu(fileName = "TestStage",menuName = "Stage/TestStage",order=1)]
public class TestStage : Stage
{
    List<int> _enemyIDs = new List<int>();
    public override IEnumerator StageScript(StageArgs args)
    {
        yield return new WaitForSeconds(1f);
        int enemy1a = args.EnemyManager.CreateNewEnemy("enemy1", -1, new Vector2(-6, 5),
            WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, 5), 20, "-x", 0));
        int enemy1b = args.EnemyManager.CreateNewEnemy("enemy1", -1, new Vector2(6, 5),
            WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, 5), 20, "-x", -90));
        for (int i = 0; i < 3; i++)
        {
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1a));

            yield return new WaitForSeconds(0.5f);

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1b));

            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        int enemy2a, enemy2b;
        for (int i = 4; i >= 0; i -= 2)
        {
            enemy2a = args.EnemyManager.CreateNewEnemy("enemy2", -1, new Vector2(-6, i + 0.5f),
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, i + 0.5f), 20, "-x", 45));
            enemy2b = args.EnemyManager.CreateNewEnemy("enemy2", -1, new Vector2(6, i),
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, i), 20, "-x", 225));

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy2a));
            yield return new WaitForSeconds(0.5f);
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy2b));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        int enemy3a;
        for(int i = -2; i <= 2; i += 4)
        {
            enemy3a = args.EnemyManager.CreateNewEnemy("enemy3", -1, new Vector2(i, 7),
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(i, 7), 20, "sin(3*x)*0.5", -90));
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3a));
            yield return new WaitForSeconds(0.5f);
        }

        int enemy3b = args.EnemyManager.CreateNewEnemy("enemy3", -1, new Vector2(0, 7),
                WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 20, "sin(3*x)*0.5", -90));
        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3b));
        yield return new WaitForSeconds(6f);

        int boss = args.EnemyManager.CreateNewEnemy("boss", -1, new Vector2(0, 7),
            WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 3.5f, "-x", -45));
        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(boss));

        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
