using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyClass;
using DanmakU.Fireables;
using System.Linq;
using WaypointPath;

[CreateAssetMenu(fileName = "StageOne", menuName = "Stage/StageOne", order = 2)]
public class StageOne : Stage
{
    List<int> _enemyIDs;
    int currentID = 0;
    float timePassed = 0f;

    public void Init()
    {
        currentID = 0;
        timePassed = 0f;
    }

    public override IEnumerator StageScript(StageArgs args)
    {
        Init();
        //yield return new WaitUntil(args.EnemyManager.Enemies);
        while (currentID < args.EnemyManager.Enemies.Count)
        {
            Enemy currentEnemy = args.EnemyManager.Enemies[currentID];
            if (currentEnemy.SpawnTime - timePassed <= 0.01f)
                yield return new WaitForSeconds(currentEnemy.SpawnTime - timePassed);

            _enemyIDs.Add(args.EnemyManager.SpawnNewEnemy(currentID));
            args.EnemyManager.SetTimer(_enemyIDs.Last(), true);

            timePassed += currentEnemy.SpawnTime;
            currentID++;
        }

        Debug.Log("Stage ended.");
    }

    public float WaitForNextEnemy(Enemy enemy)
    {
        return 1;
    }
}
