using DanmakU;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyBank))]
public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies;
    private EnemyBank _enemyBank;
    // Start is called before the first frame update
    void Start()
    {
        _enemies = new List<GameObject>();
        _enemyBank = GetComponent<EnemyBank>();
    }

    public void SpawnEnemy(string enemyName, List<Vector2> path, List<int> eventWaypoints = null, List<DanmakU.IFireable> waypointEvents = null,
        List<float> eventTimes = null, List<DanmakU.IFireable> timeEvents = null, float timer = 0, DanmakuConfig danmakuConfig = new DanmakuConfig())
    {
        GameObject enemy = Instantiate(_enemyBank.EnemyEntries[enemyName], path[0], Quaternion.identity, transform);
        enemy.GetComponent<WaypointWalker>().SetWaypointPath(path);

        if (eventWaypoints == null ^ waypointEvents == null)
            Debug.LogWarning("Event waypoints or waypoint events are null!");
        else if (eventWaypoints != null && waypointEvents != null)
            enemy.GetComponentInChildren<EnemyDanmakuEmitter>().EventWaypoints(in eventWaypoints, in waypointEvents);

        if (eventTimes == null ^ timeEvents == null)
        {
            Debug.LogWarning("Event times or time events are null!");
        }
        else if (eventTimes != null && timeEvents != null)
            enemy.GetComponentInChildren<EnemyDanmakuEmitter>().EventTimes(in eventTimes, in timeEvents);

        _enemies.Add(enemy);
    }

    public bool NoEnemiesPresent()
    {
        return _enemies.All(e => !e.activeInHierarchy);
    }

    public void Clear()
    {
        foreach (var enemy in _enemies)
        {
            Destroy(enemy);
        }
        _enemies.Clear();
    }
}
