using DanmakU;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnemyStruct;
using WaypointPath;

[RequireComponent(typeof(EnemyBank))]
public class EnemyManager : MonoBehaviour
{
    private Dictionary<int, GameObject> _enemyObjects;
    private List<Enemy> _enemies;
    private int _enemyObjectIDs = 0;
    private int _enemyStructIDs = 0;
    private EnemyBank _enemyBank;
    
    void Start()
    {
        _enemyObjects = new Dictionary<int, GameObject>();
        _enemies = new List<Enemy>();
        _enemyBank = GetComponent<EnemyBank>();
    }

    //public Dictionary<int, Enemy> AddEnemiesFromFile(string filename)
    //{
    //    return;
    //}

    /// <summary>
    /// Creates a new enemy struct adding it to the list of all enemy structs
    /// </summary>
    /// <param name="spawnRepeats">optional</param>
    /// <returns>Enemy struct ID</returns>
    public int CreateNewEnemy (Enemy enemy)
    {
        enemy.ID = ++_enemyStructIDs;
        enemy.Path[0].StartPosition = enemy.SpawnPosition;
        _enemies.Add(enemy);
        return _enemyStructIDs;
    }

    /// <summary>
    /// Adds waypoints to enemy's path
    /// </summary>
    /// <param name="id">Enemy struct ID</param>
    /// <param name="waypoints">List of waypoints to add</param>
    /// <param name="insertAt">Optional; inserts the waypoints at the given index (at the end by default)</param>
    public void AddWaypointsToEnemy(int id, List<WaypointPathCreator> waypoints, int insertAt = -1)
    {
        id--;
        if(insertAt <= -1)
        {
            _enemies[id].Path.AddRange(waypoints);
        }
        else _enemies[id].Path.InsertRange(insertAt, waypoints);
    }

    /// <summary>
    /// Spawns an enemy on the stage
    /// </summary>
    /// <param name="enemy">The enemy struct ID</param>
    /// <returns>Spawned enemy object ID</returns>
    public int SpawnNewEnemy(int id)
    {
        id--;
        GameObject enemyObject = Instantiate(_enemyBank.EnemyEntries[_enemies[id].Name], _enemies[id].SpawnPosition, Quaternion.identity, transform);
        if (_enemies[id].Fireable != null)
            enemyObject.GetComponentInChildren<EnemyDanmakuEmitter>().Fireable = _enemies[id].Fireable;
        enemyObject.GetComponent<WaypointPathData>().Path = _enemies[id].Path.ToList();

        _enemyObjects.Add(++_enemyObjectIDs, enemyObject);
        return _enemyObjectIDs;
    }

    public bool AddEventWaypoints(int enemyID, in List<int> eventWaypoints, in List<IFireable> waypointEvents, DanmakuConfig danmakuConfig)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().EventWaypoints(eventWaypoints, waypointEvents, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetWaypointEvents(int enemyID, bool set)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetWaypointEventsAction(set); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool AddEventTimes(int enemyID, in List<float> eventTimes, in List<IFireable> timeEvents, DanmakuConfig danmakuConfig)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().EventTimes(eventTimes, timeEvents, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetTimeEvents(int enemyID, bool set)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetTimeEventsAction(set); }
        catch (KeyNotFoundException){ return false; }
        return true;
    }

    public bool AddTimerShots(int enemyID, in float fireRate, in IFireable timerShot, DanmakuConfig danmakuConfig)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().TimerShots(fireRate, timerShot, danmakuConfig); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool SetTimer(int enemyID, bool set)
    {
        try { _enemyObjects[enemyID].GetComponentInChildren<EnemyDanmakuEmitter>().SetTimer(set); }
        catch (KeyNotFoundException) { return false; }
        return true;
    }

    public bool NoEnemiesPresent()
    {
        return _enemyObjects.Values.All(e => !e.activeInHierarchy);
    }

    public void Clear()
    {
        foreach (var enemy in _enemyObjects.Values)
        {
            Destroy(enemy);
        }
        _enemyObjects.Clear();
        _enemyObjectIDs = 0;
    }
}
