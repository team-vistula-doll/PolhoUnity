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

    [field: SerializeField]
    public DanmakU.Range FireRate { get; set; } = 1f;
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }

    private IFireable fireable = null;
    public IFireable Fireable
    {
        get => fireable;
        set
        {
            Fireable tempFireable = (Fireable)value;
            fireable = tempFireable.Of(Set);
        }
    }

    DanmakuConfig config;
    public DanmakuConfig Config
    {
        get => config;
        set => config = value;
    }
    
    public DanmakuConfig NewConfig { get; set; }

    List<int> _eventWaypoints;
    List<float> _eventTimes;
    List<IFireable> _waypointEvents, _timeEvents;
    int _eventWaypointsIterator = 0, _eventTimesIterator = 0;
    float _time;
    Action timedEvents, timerEvent;

    void Awake()
    {
        Set = EnemyDanmakuSet.Set;
        Set.AddModifiers(GetComponents<IDanmakuModifier>());
    }

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

        _time = 0f;
        Timer = 1f / FireRate.GetValue();

        config = new DanmakuConfig
        {
            Position = new Vector2(0, 0),
            Rotation = 0,
            Speed = Speed,
            AngularSpeed = AngularSpeed,
            Color = Color
        };
        NewConfig = config;
    }


    DanmakuConfig UpdateDanmakuConfig()
    {
        config = NewConfig;
        config.Position += new Vector2(transform.position.x, transform.position.y);
        config.Rotation += transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        return config;
    }

    public void EventWaypoints(in List<int> eventWaypoints, in List<IFireable> waypointEvents, DanmakuConfig danmakuConfig)
    {
        _eventWaypoints = eventWaypoints;
        _waypointEvents = waypointEvents;
        NewConfig = danmakuConfig;

        GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    void WaypointEvents(object sender, int waypoint)
    {
        if (_eventWaypoints[_eventWaypointsIterator] == waypoint)
        {
            fireable = _waypointEvents[_eventWaypointsIterator++];
            fireable.Fire(UpdateDanmakuConfig());
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
        NewConfig = danmakuConfig;

        timedEvents = TimeEvents;
    }

    void TimeEvents()
    {
        if (_eventTimes[_eventTimesIterator] >= _time)
        {
            fireable = _timeEvents[_eventTimesIterator++];
            fireable.Fire(UpdateDanmakuConfig());
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
        fireable = timerShot;
        NewConfig = danmakuConfig;
    }

    void ShotTimer()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            fireable.Fire(UpdateDanmakuConfig());
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
        if (fireable == null) return;
        timedEvents?.Invoke();
        timerEvent?.Invoke();
        
        _time += Time.deltaTime;
    }

}

