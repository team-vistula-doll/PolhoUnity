using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStruct;
using DanmakU.Fireables;
using System.Linq;
using WaypointPath;

[CreateAssetMenu(fileName = "TestStage", menuName = "Stage/TestStage", order=1)]
public class TestStage : Stage
{
    List<int> _enemyIDs = new();
    WaypointPathExpression _pathExpression = new();
    public override IEnumerator StageScript(StageArgs args)
    {
        yield return new WaitForSeconds(1f);

        //Spawn positions for the first enemies

        BezierControlPoints enemy1aVectors = new(new(-6, 5), new(7, -2), new(-6, -1), new(2, -3));

        BezierControlPoints enemy1bVectors = enemy1aVectors.GetModifiedCurveCopy(new(0.5f, 0.5f), (x, y) => x - y);
        BezierControlPoints enemy1cVectors = enemy1aVectors.GetModifiedCurveCopy(new(-1f, 1f), (x, y) => x * y);
        BezierControlPoints enemy1dVectors = enemy1cVectors.GetModifiedCurveCopy(new(0.5f, -0.5f), (x, y) => x + y);

        int enemy1a = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1aVectors.StartPosition,
            Path = _pathExpression.GeneratePathFromCurve(enemy1aVectors),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1bVectors.StartPosition,
            Path = _pathExpression.GeneratePathFromCurve(enemy1bVectors),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1c = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1cVectors.StartPosition,
            Path = _pathExpression.GeneratePathFromCurve(enemy1cVectors),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1d = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1dVectors.StartPosition,
            Path = _pathExpression.GeneratePathFromCurve(enemy1dVectors),
            Fireable = new Arc(3, 3, 45)
        });
        

        for (int i = 0; i < 8; i++)
        {
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1a));
            args.EnemyManager.SetTimer(_enemyIDs.Last(), true);
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1c));
            args.EnemyManager.SetTimer(_enemyIDs.Last(), true);

            yield return new WaitForSeconds(0.2f);

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1b));
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy1d));

            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(5f);

        int enemy2a, enemy2b;
        for (int i = 4; i >= 0; i -= 2)
        {
            enemy2a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = new Vector2(-6, i + 0.5f),
                Path = _pathExpression.GeneratePath(new Vector2(-6, i + 0.5f), 20,
                "-x", 45)
            });
            enemy2b = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = new Vector2(6, i),
                Path = _pathExpression.GeneratePath(new Vector2(6, i), 20,
                "-x", 225)
            });

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy2a));
            yield return new WaitForSeconds(0.5f);
            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy2b));
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        int enemy3a;
        for(int i = -2; i <= 2; i += 4)
        {
            enemy3a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy3",
                SpawnTime = -1,
                SpawnPosition = new Vector2(i, 7),
                Path = _pathExpression.GeneratePath(new Vector2(i, 7), 20,
                "sin(3*x)*0.5", -90)
            });

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3a));
            yield return new WaitForSeconds(0.5f);
        }
        int enemy3b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy3",
            SpawnTime = -1,
            SpawnPosition = new Vector2(0, 7),
            Path = _pathExpression.GeneratePath(new Vector2(0, 7), 20,
            "sin(3*x)*0.5", -90)
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3b));
        yield return new WaitForSeconds(6f);

        int boss = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "boss",
            SpawnTime = -1,
            SpawnPosition = new Vector2(0, 7),
            Path = _pathExpression.GeneratePath(new Vector2(0, 7), 3.5f,
            "-x", -45)
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(boss));

        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
