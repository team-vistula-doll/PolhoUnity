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
    public override IEnumerator StageScript(StageArgs args)
    {
        yield return new WaitForSeconds(1f);

        //Spawn positions for the first enemies
        List<Vector2> enemy1aVectors = new()
        {
            new Vector2(-6, 5), new Vector2(7, -2), new Vector2(-6, -1), new Vector2(2, -3)
        };

        List<Vector2> enemy1bVectors = new();
        foreach (Vector2 v in enemy1aVectors)
        {
            v.Set(v.x - 0.5f, v.y - 0.5f);
            enemy1bVectors.Add(v);
        }

        List<Vector2> enemy1cVectors = new();
        foreach (Vector2 v in enemy1aVectors)
        {
            v.Set(v.x * -1, v.y);
            enemy1cVectors.Add(v);
        }

        List<Vector2> enemy1dVectors = new();
        foreach (Vector2 v in enemy1cVectors)
        {
            v.Set(v.x + 0.5f, v.y - 0.5f);
            enemy1dVectors.Add(v);
        }

        int enemy1a = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1aVectors[0],
            Path = WaypointPathCreator.GeneratePathFromCurve(enemy1aVectors[0], enemy1aVectors[1],
            enemy1aVectors[2], enemy1aVectors[3]),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1bVectors[0],
            Path = WaypointPathCreator.GeneratePathFromCurve(enemy1bVectors[0], enemy1bVectors[1],
            enemy1bVectors[2], enemy1bVectors[3]),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1c = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1cVectors[0],
            Path = WaypointPathCreator.GeneratePathFromCurve(enemy1cVectors[0], enemy1cVectors[1],
            enemy1cVectors[2], enemy1cVectors[3]),
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1d = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1dVectors[0],
            Path = WaypointPathCreator.GeneratePathFromCurve(enemy1dVectors[0], enemy1dVectors[1],
            enemy1dVectors[2], enemy1dVectors[3]),
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
                Path = WaypointPathCreator.GeneratePathFromExpression(new Vector2(-6, i + 0.5f), 20,
                "-x", 45)
            });
            enemy2b = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = new Vector2(6, i),
                Path = WaypointPathCreator.GeneratePathFromExpression(new Vector2(6, i), 20,
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
                Path = WaypointPathCreator.GeneratePathFromExpression(new Vector2(i, 7), 20,
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
            Path = WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 20,
            "sin(3*x)*0.5", -90)
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3b));
        yield return new WaitForSeconds(6f);

        int boss = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "boss",
            SpawnTime = -1,
            SpawnPosition = new Vector2(0, 7),
            Path = WaypointPathCreator.GeneratePathFromExpression(new Vector2(0, 7), 3.5f,
            "-x", -45)
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(boss));

        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
