using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Fireables;
using UnityEngine;

public class EnemyDanmakuEmitter : DanmakuBehaviour, IShootable
{

    public DanmakuPrefab DanmakuType;

    public Range Speed = 2f;
    public Range AngularSpeed = 0;
    public Color Color = Color.white;
    public Range FireRate = 1f;
    public Arc  Arc;
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
    Action times;

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
    }

    public void EventWaypoints(in List<int> eventWaypoints, in List<IFireable> waypointEvents)
    {
        _eventWaypoints = eventWaypoints;
        _waypointEvents = waypointEvents;

        GetComponentInParent<WaypointWalker>().WaypointEvent += WaypointEvents;
    }

    void WaypointEvents(object sender, int waypoint)
    {
        if(_eventWaypoints[_eventWaypointsIterator] == waypoint)
        {
            _fireable = _waypointEvents[_eventWaypointsIterator++];
        }
    }

    public void EventTimes(in List<float> eventTimes, in List<IFireable> timeEvents)
    {

        _eventTimes = eventTimes;
        _timeEvents = timeEvents;

        times += TimeEvents;
    }

    void TimeEvents()
    {
        if(_eventTimes[_eventTimesIterator] >= _time)
        {
            _fireable = _timeEvents[_eventTimesIterator++];
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_fireable == null) return;
        times?.Invoke();
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            _config = new DanmakuConfig
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad,
                Speed = Speed,
                AngularSpeed = AngularSpeed,
                Color = Color
            };
            _fireable.Fire(_config);
            Timer = 1f / FireRate.GetValue();
        }
        _time += Time.deltaTime;
    }

}

