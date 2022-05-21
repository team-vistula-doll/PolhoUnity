using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Fireables;
using System;
using UnityEngine;

public class EnemyDanmakuEmitter : DanmakuBehaviour, IShootable
{

    public DanmakuPrefab DanmakuType;

    public DanmakU.Range Speed = 2f;
    public DanmakU.Range AngularSpeed = 0;
    public Color Color = Color.white;
    public Arc Arc;
    public Line Line;

    [field: SerializeField]
    public DanmakU.Range FireRate { get; set; } = 1f;
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }
    public IFireable Fireable { get; set; }
    public DanmakuConfig Config
    {
        get { return config; }
        set { config = value; }
    }

    DanmakuConfig config;

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
        Fireable = Arc.Of(Line).Of(Set);
        Timer = 1f / FireRate.GetValue();
        timerEvent = ShotTimer;

        config = new DanmakuConfig
        {
            Position = new Vector2(0, 0),
            Rotation = 0,
            Speed = Speed,
            AngularSpeed = AngularSpeed,
            Color = Color
        };
    }


    DanmakuConfig UpdateDanmakuConfig()
    {
        config.Position += new Vector2(transform.position.x, transform.position.y);
        config.Rotation += transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        return config;
    }

    public void EventWaypoints(in List<int> eventWaypoints, in List<IFireable> waypointEvents, DanmakuConfig danmakuConfig)
    {
        _eventWaypoints = eventWaypoints;
        _waypointEvents = waypointEvents;
        config = danmakuConfig;

        GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    void WaypointEvents(object sender, int waypoint)
    {
        if (_eventWaypoints[_eventWaypointsIterator] == waypoint)
        {
            Fireable = _waypointEvents[_eventWaypointsIterator++];
            Fireable.Fire(UpdateDanmakuConfig());
        }
    }

    public void SetWaypointEventsAction(bool set)
    {
        GetComponentInParent<WaypointWalker>().WaypointEvent -= WaypointEvents;
        if (set) GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    public void EventTimes(in List<float> eventTimes, in List<IFireable> timeEvents, DanmakuConfig danmakuConfig)
    {

        _eventTimes = eventTimes;
        _timeEvents = timeEvents;
        config = danmakuConfig;

        timedEvents = TimeEvents;
    }

    void TimeEvents()
    {
        if (_eventTimes[_eventTimesIterator] >= _time)
        {
            Fireable = _timeEvents[_eventTimesIterator++];
            Fireable.Fire(UpdateDanmakuConfig());
        }
    }

    public void SetTimeEventsAction(bool set)
    {
        timedEvents -= TimeEvents;
        if (set) timedEvents += TimeEvents;
    }

    public void TimerShots(in DanmakU.Range fireRate, in IFireable timerShot, DanmakuConfig danmakuConfig)
    {
        Timer = 1f / fireRate.GetValue();
        Fireable = timerShot;
        config = danmakuConfig;
    }

    void ShotTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            Fireable.Fire(UpdateDanmakuConfig());
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
        if (Fireable == null) return;
        timedEvents?.Invoke();
        timerEvent?.Invoke();
        
        _time += Time.deltaTime;
    }

}

