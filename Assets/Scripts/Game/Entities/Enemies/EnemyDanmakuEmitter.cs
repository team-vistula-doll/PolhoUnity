using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Fireables;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDanmakuEmitter : DanmakuBehaviour, IShootable
{

    public DanmakuPrefab DanmakuType;

    public Range Speed = 2f;
    public Range AngularSpeed = 0;
    public Color Color = Color.white;
    public Range FireRate = 1f;
    public Arc Arc;
    public Line Line;

    public DanmakuSet Set { get; set; }
    public float Timer { get; set; }

    DanmakuConfig _config;
    IFireable _fireable;

    List<int> _eventWaypoints;
    List<float> _eventTimes;
    List<IFireable> _waypointEvents, _timeEvents;
    int _eventWaypointsIterator = 0, _eventTimesIterator = 0;
    float _time;
    Action eventTimes, timer;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (DanmakuType == null)
        {
            Debug.LogWarning($"Emitter doesn't have a valid DanmakuPrefab", this);
            return;
        }
        Set = EnemyDanmakuSet.Set;
        Set.AddModifiers(GetComponents<IDanmakuModifier>());

        _time = 0f;
        _fireable = Arc.Of(Line).Of(Set);
        Timer = 1f / FireRate.GetValue();
        timer += StandardTimer;
    }

    DanmakuConfig CreateDanmakuConfig(DanmakuConfig danmakuConfig)
    {
        _config = danmakuConfig;
        _config.Position += new Vector2(transform.position.x, transform.position.y);
        _config.Rotation += transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        return _config;
    }

    public void EventWaypoints(in List<int> eventWaypoints, in List<IFireable> waypointEvents, DanmakuConfig danmakuConfig)
    {
        _eventWaypoints = eventWaypoints;
        _waypointEvents = waypointEvents;
        _config = danmakuConfig;

        GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    void WaypointEvents(object sender, int waypoint)
    {
        if (_eventWaypoints[_eventWaypointsIterator] == waypoint)
        {
            _fireable = _waypointEvents[_eventWaypointsIterator++];
            _fireable.Fire(CreateDanmakuConfig(_config));
        }
    }

    public void EventTimes(in List<float> eventTimes, in List<IFireable> timeEvents, DanmakuConfig danmakuConfig)
    {

        _eventTimes = eventTimes;
        _timeEvents = timeEvents;
        _config = danmakuConfig;

        this.eventTimes += TimeEvents;
    }

    void TimeEvents()
    {
        if (_eventTimes[_eventTimesIterator] >= _time)
        {
            _fireable = _timeEvents[_eventTimesIterator++];
            _fireable.Fire(CreateDanmakuConfig(_config));
        }
    }

    public void ShotTimer(in float fireRate, in IFireable timerShot, DanmakuConfig danmakuConfig)
    {
        Timer = fireRate;
        _fireable = timerShot;
        _config = danmakuConfig;
    }

    void StandardTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            _fireable.Fire(CreateDanmakuConfig(_config));
            Timer = 1f / FireRate.GetValue();
        }
    }

    public void EnableTimer() => timer += StandardTimer;
    public void DisableTimer() => timer -= StandardTimer;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_fireable == null) return;
        eventTimes?.Invoke();
        timer?.Invoke();
        
        _time += Time.deltaTime;
    }

}

