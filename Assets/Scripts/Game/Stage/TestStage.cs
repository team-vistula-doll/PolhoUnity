using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyClass;
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

        WaypointPathBezier enemy1aPath = new(new(new(-6, 5), 0, 0), new(7, -2), new(-6, -1), new(2, -3));

        var enemy1bPath = enemy1aPath.GetModifiedPathCopy(new(0.5f, 0.5f), (x, y) => x - y);
        var enemy1cPath = enemy1aPath.GetModifiedPathCopy(new(-1f, 1f), (x, y) => x * y);
        var enemy1dPath = enemy1cPath.GetModifiedPathCopy(new(0.5f, -0.5f), (x, y) => x + y);

        int enemy1a = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1aPath.StartPoint.Position,
            Path = new() { enemy1aPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1bPath.StartPoint.Position,
            Path = new() { enemy1bPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1c = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1cPath.StartPoint.Position,
            Path = new() { enemy1cPath },
            Fireable = new Arc(3, 90, 0)
        });
        int enemy1d = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy1",
            SpawnTime = -1,
            SpawnPosition = enemy1dPath.StartPoint.Position,
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
            WaypointPathExpression enemy2aPath = new(new(new(-6, i + 0.5f), 0, 0), "-x", 20, 45);
            enemy2a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = enemy2aPath.StartPoint.Position,
                Path = new() { enemy2aPath }
            });
            WaypointPathExpression enemy2bPath = new(new(new Vector2(6, i),0, 0), "-x", 20, 225);
            enemy2b = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy2",
                SpawnTime = -1,
                SpawnPosition = enemy2bPath.StartPoint.Position,
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
            WaypointPathExpression enemy3aPath = new(new(new Vector2(i, 7),0, 0), "sin(3*x)*0.5", 20, -90);
            enemy3a = args.EnemyManager.CreateNewEnemy(new Enemy
            {
                Name = "enemy3",
                SpawnTime = -1,
                SpawnPosition = enemy3aPath.StartPoint.Position,
                Path = new() { enemy3aPath }
            });

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3a));
            yield return new WaitForSeconds(0.5f);
        }
        WaypointPathExpression enemy3bPath = new(new(new Vector2(0, 7), 0, 0), "sin(3*x)*0.5", 20, -90);
        int enemy3b = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "enemy3",
            SpawnTime = -1,
            SpawnPosition = enemy3bPath.StartPoint.Position,
            Path = new() { enemy3bPath }
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(enemy3b));
        yield return new WaitForSeconds(6f);

        WaypointPathExpression bossPath = new(new(new Vector2(0, 7), 0, 0), "-x", 3.5f, -45);
        int boss = args.EnemyManager.CreateNewEnemy(new Enemy
        {
            Name = "boss",
            SpawnTime = -1,
            SpawnPosition = bossPath.StartPoint.Position,
            Path = new() { bossPath }
        });

        _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(boss));

        yield return new WaitForSeconds(1f);
        Map.Paused = true;

        yield return new WaitUntil(args.EnemyManager.NoEnemiesPresent);

        Debug.Log("END! Good Job!");
    }
}
