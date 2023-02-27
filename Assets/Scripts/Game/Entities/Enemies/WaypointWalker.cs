using System;
using System.Collections.Generic;
using UnityEngine;
using WaypointPath;

[RequireComponent(typeof(SimpleEnemy))]
public class WaypointWalker : MonoBehaviour
{
    public List<Vector2> Path = new() { Vector2.zero };

    private IMoveable _moveableEntity;
    private Vector2 _nextWaypoint;
    private int _waypoints;
    private int _currentWaypoint;

    public event EventHandler<int> WaypointEvent;

    private bool _isMoving = true;
    private Action move;


    public void Start()
    {
        _moveableEntity = GetComponent<IMoveable>();
        transform.position = Path[0];
    }

    /// <summary>
    /// Validates and sets the path from a function
    /// </summary>
    /// <param name="isAdd">If true the path is added to the existing path, if false the path replaces the existing path</param>
    /// <param name="isTemp">If true sets the temp path, if false sets the real path</param>

    public void FixedUpdate()
    {
        move?.Invoke();
    }

    public void Move()
    {
        if (_moveableEntity != null && _moveableEntity.Rigidbody2D != null)
        {
            if (_isMoving)
                _moveableEntity.Move(GetCurrentAxis());

            else
            {
                _moveableEntity.Move(new Vector2(0, 0));
                move -= Move;
            }
        }
    }

    /// <summary>
    /// Changes _nextWaypoint if you are close enough to the next waypoint
    /// </summary>
    public void ChangeNextWaypoint()
    {
        if (Vector2.Distance(transform.position, _nextWaypoint) < 0.1f && _currentWaypoint < _waypoints - 1)
        {
            while (Path.Count > _currentWaypoint && Vector2.Distance(_nextWaypoint, Path[_currentWaypoint]) < 0.01f)
                Path.RemoveAt(_currentWaypoint);
            _nextWaypoint = Path[_currentWaypoint];
            WaypointEvent?.Invoke(this, _currentWaypoint);
            _currentWaypoint++;
        }
        else if (_currentWaypoint >= _waypoints - 1)
            _isMoving = false;
    }

    public Vector2 GetCurrentAxis()
    {
        ChangeNextWaypoint();

        Vector2 axis = _nextWaypoint - new Vector2(_moveableEntity.Rigidbody2D.position.x,
            _moveableEntity.Rigidbody2D.position.y);

        return axis.normalized;
    }
}
