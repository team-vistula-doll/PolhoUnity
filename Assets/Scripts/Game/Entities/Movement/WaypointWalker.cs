using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WaypointPath;

[RequireComponent(typeof(SimpleEnemy))]
public class WaypointWalker : MonoBehaviour
{
    public event EventHandler<int> WaypointEvent;

    private WaypointPathData _pathData;
    private List<Waypoint> _vector2Path;
    private IPathable _moveableEntity;
    private Waypoint _nextWaypoint = new(new(), null, null);
    private int _currentWaypointIndex;
    private int _currentPathIndex;

    private bool _isMoving = false;
    private Action move;


    void Start()
    {
        _moveableEntity = GetComponent<IPathable>();
        _pathData = GetComponent<WaypointPathData>();
        //transform.position = pathData.Path[0];
        _currentPathIndex = 0;
        if (_pathData.Path.Count > 0)
        {
            _vector2Path = _pathData.Path[_currentPathIndex++].GeneratePath();
            _nextWaypoint = _vector2Path[0];
            move += Move;
            _isMoving = true;
        }
    }

    void FixedUpdate()
    {
        move?.Invoke();
    }

    private void Move()
    {
        if (_moveableEntity != null)
        {
            if (_isMoving)
                _moveableEntity.Move(GetCurrentWaypoint());

            else
            {
                //_moveableEntity.Move(_moveableEntity.transform.position);
                move -= Move;
            }
        }
    }

    public void AddPath(List<WaypointPathCreator> vectors)
    {
        _pathData.Path.AddRange(vectors);
        if (!_isMoving)
        {
            _vector2Path.AddRange(_pathData.Path[_currentPathIndex++].GeneratePath());
            ChangeNextWaypoint();
        }
        _isMoving = true;
        move += Move;
    }

    /// <summary>
    /// Changes _nextWaypoint if you are close enough to the next waypoint
    /// </summary>
    private void ChangeNextWaypoint()
    {
        if (Vector2.Distance(transform.position, _nextWaypoint.Position) < 0.1f && _currentWaypointIndex < _vector2Path.Count - 1)
        {
            while (_vector2Path.Count > _currentWaypointIndex && Vector2.Distance(_nextWaypoint.Position, _vector2Path[_currentWaypointIndex].Position) < 0.01f)
                _vector2Path.RemoveAt(_currentWaypointIndex);
            _nextWaypoint = _vector2Path[_currentWaypointIndex];
            WaypointEvent?.Invoke(this, _currentWaypointIndex);
            _currentWaypointIndex++;
        }
        else if (_currentWaypointIndex >= _vector2Path.Count - 1)
        {
            if (_currentPathIndex < _pathData.Path.Count - 1)
                _vector2Path.AddRange(_pathData.Path[_currentPathIndex++].GeneratePath());
            else
                _isMoving = false;
        }
    }

    public Waypoint GetCurrentWaypoint()
    {
        ChangeNextWaypoint();

        return _nextWaypoint;
    }
}
