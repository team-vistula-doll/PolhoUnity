using DanmakU;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnemyStruct;

[RequireComponent(typeof(EnemyBank))]
public class EnemyManager : MonoBehaviour
{
    private Dictionary<int, GameObject> _enemies;
    private int _enemyIDs = 0;
    private EnemyBank _enemyBank;
    // Start is called before the first frame update
    void Start()
    {
        _enemies = new Dictionary<int, GameObject>();
        _enemyBank = GetComponent<EnemyBank>();
    }

    public Dictionary<int, Enemy> AddEnemiesFromFile(string filename)
    {
        return;
    }

    public int SpawnNewEnemy(string enemyName, List<Vector2> path)
    {
        GameObject enemy = Instantiate(_enemyBank.EnemyEntries[enemyName], path[0], Quaternion.identity, transform);
        enemy.GetComponent<WaypointWalker>().SetWaypointPath(path);

        _enemies.Add(++_enemyIDs, enemy);
        return _enemyIDs;
    }

    public bool AddEventWaypoints(int enemyID, in List<int> eventWaypoints, in List<IFireable> waypointEvents, DanmakuConfig danmakuConfig)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().EventWaypoints(eventWaypoints, waypointEvents, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetWaypointEvents(int enemyID, bool set)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetWaypointEventsAction(set); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool AddEventTimes(int enemyID, in List<float> eventTimes, in List<IFireable> timeEvents, DanmakuConfig danmakuConfig)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().EventTimes(eventTimes, timeEvents, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetTimeEvents(int enemyID, bool set)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetTimeEventsAction(set); }
        catch (KeyNotFoundException){ return false; }
        return true;
    }

    public bool AddTimerShots(int enemyID, in float fireRate, in IFireable timerShot, DanmakuConfig danmakuConfig)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().TimerShots(fireRate, timerShot, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetTimer(int enemyID, bool set)
    {
        try { _enemies[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetTimer(set); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool NoEnemiesPresent()
    {
        return _enemies.Values.All(e => !e.activeInHierarchy);
    }

    public void Clear()
    {
        foreach (var enemy in _enemies.Values)
        {
            Destroy(enemy);
        }
        _enemies.Clear();
        _enemyIDs = 0;
    }
}
