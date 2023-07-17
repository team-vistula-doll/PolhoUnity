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
    List<int> _enemyIDs;

    public override IEnumerator StageScript(StageArgs args)
    {
        _enemyIDs = new();
        yield return new WaitForSeconds(1f);

        //Spawn positions for the first enemies

        WaypointPathBezier enemy1aPath = CreateInstance<WaypointPathBezier>();
        enemy1aPath.Init(new(-6, 5), new(7, -2), new(-6, -1), new(2, -3));

        var enemy1bPath = enemy1aPath.GetModifiedCurveCopy(new(0.5f, 0.5f), (x, y) => x - y);
        var enemy1cPath = enemy1aPath.GetModifiedCurveCopy(new(-1f, 1f), (x, y) => x * y);
        var enemy1dPath = enemy1cPath.GetModifiedCurveCopy(new(0.5f, -0.5f), (x, y) => x + y);

        int enemy1a = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1aPath.StartPosition,
            Path = new() { enemy1aPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1bPath.StartPosition,
            Path = new() { enemy1bPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1c = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1cPath.StartPosition,
            Path = new() { enemy1cPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1d = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1dPath.StartPosition,
            Path = new() { enemy1dPath },
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
            WaypointPathExpression enemy2aPath = CreateInstance<WaypointPathExpression>();
            enemy2aPath.Init(new Vector2(-6, i + 0.5f), "-x", 20, 45);
            enemy2a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = enemy2aPath.StartPosition,
                Path = new() { enemy2aPath }
            });
            WaypointPathExpression enemy2bPath = CreateInstance<WaypointPathExpression>();
            enemy2bPath.Init(new Vector2(6, i), "-x", 20, 225);
            enemy2b = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = enemy2bPath.StartPosition,
                Path = new() { enemy2bPath }
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
            WaypointPathExpression enemy3aPath = CreateInstance<WaypointPathExpression>();
            enemy3aPath.Init(new Vector2(i, 7), "sin(3*x)*0.5", 20, -90);
            enemy3a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy3",
                SpawnTime = -1,
                SpawnPosition = enemy3aPath.StartPosition,
                Path = new() { enemy3aPath }
            });

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3a));
            yield return new WaitForSeconds(0.5f);
        }
        WaypointPathExpression enemy3bPath = CreateInstance<WaypointPathExpression>();
        enemy3bPath.Init(new Vector2(0, 7), "sin(3*x)*0.5", 20, -90);
        int enemy3b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy3",
            SpawnTime = -1,
            SpawnPosition = enemy3bPath.StartPosition,
            Path = new() { enemy3bPath }
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3b));
        yield return new WaitForSeconds(6f);

        WaypointPathExpression bossPath = CreateInstance<WaypointPathExpression>();
        bossPath.Init(new Vector2(0, 7), "-x", 3.5f, -45);
        int boss = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "boss",
            SpawnTime = -1,
            SpawnPosition = bossPath.StartPosition,
            Path = new() { bossPath }
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(boss));

        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
