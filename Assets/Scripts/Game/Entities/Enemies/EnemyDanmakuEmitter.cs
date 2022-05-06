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
    public Arc Arc;
    public Line Line;

    [field: SerializeField]
    public Range FireRate { get; set; } = 1f;
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }

    DanmakuConfig _config;
    IFireable _fireable;

    List<int> _eventWaypoints;
    List<float> _eventTimes;
    List<IFireable> _waypointEvents, _timeEvents;
    int _eventWaypointsIterator = 0, _eventTimesIterator = 0;
    float _time;
    Action timedEvents, timerEvent;

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
        timerEvent = ShotTimer;

        _config = new DanmakuConfig
        {
            Position = new Vector2(0, 0),
            Rotation = 0,
            Speed = Speed,
            AngularSpeed = AngularSpeed,
            Color = Color
        };
    }

    public void AddDanmakuConfig(DanmakuConfig danmakuConfig) => _config = danmakuConfig;

    DanmakuConfig UpdateDanmakuConfig(DanmakuConfig danmakuConfig)
    {
        DanmakuConfig config = danmakuConfig;
        config.Position += new Vector2(transform.position.x, transform.position.y);
        config.Rotation += transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        return config;
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
            _fireable.Fire(UpdateDanmakuConfig(_config));
        }
    }

    public void SetWaypointEvents(bool set)
    {
        GetComponentInParent<WaypointWalker>().WaypointEvent -= WaypointEvents;
        if (set) GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    public void EventTimes(in List<float> eventTimes, in List<IFireable> timeEvents, DanmakuConfig danmakuConfig)
    {

        _eventTimes = eventTimes;
        _timeEvents = timeEvents;
        _config = danmakuConfig;

        timedEvents = TimeEvents;
    }

    void TimeEvents()
    {
        if (_eventTimes[_eventTimesIterator] >= _time)
        {
            _fireable = _timeEvents[_eventTimesIterator++];
            _fireable.Fire(UpdateDanmakuConfig(_config));
        }
    }

    public void SetTimeEvents(bool set)
    {
        timedEvents -= TimeEvents;
        if (set) timedEvents += TimeEvents;
    }

    public void TimerShots(in Range fireRate, in IFireable timerShot, DanmakuConfig danmakuConfig)
    {
        Timer = 1f / fireRate.GetValue();
        _fireable = timerShot;
        _config = danmakuConfig;
    }

    void ShotTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            _fireable.Fire(UpdateDanmakuConfig(_config));
            Timer = 1f / FireRate.GetValue();
        }
    }

    public void SetTimer(bool set)
    {
        timerEvent -= ShotTimer;
        if (set) timerEvent += ShotTimer;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_fireable == null) return;
        timedEvents?.Invoke();
        timerEvent?.Invoke();
        
        _time += Time.deltaTime;
    }

}

