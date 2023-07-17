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
    private List<Vector2> _vector2Path;
    private IMoveable _moveableEntity;
    private Vector2 _nextWaypoint;
    private int _currentWaypointIndex;
    private int _currentPathIndex;

    private bool _isMoving = false;
    private Action move;


    void Start()
    {
        _moveableEntity = GetComponent<IMoveable>();
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

    /// <summary>
    /// Validates and sets the path from a function
    /// </summary>
    /// <param name="isAdd">If true the path is added to the existing path, if false the path replaces the existing path</param>
    /// <param name="isTemp">If true sets the temp path, if false sets the real path</param>

    void FixedUpdate()
    {
        move?.Invoke();
    }

    private void Move()
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
        if (Vector2.Distance(transform.position, _nextWaypoint) < 0.1f && _currentWaypointIndex < _vector2Path.Count - 1)
        {
            while (_pathData.Path.Count > _currentWaypointIndex && Vector2.Distance(_nextWaypoint, _vector2Path[_currentWaypointIndex]) < 0.01f)
                _pathData.Path.RemoveAt(_currentWaypointIndex);
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

    public Vector2 GetCurrentAxis()
    {
        ChangeNextWaypoint();

        Vector2 axis = _nextWaypoint - new Vector2(_moveableEntity.Rigidbody2D.position.x,
            _moveableEntity.Rigidbody2D.position.y);

        return axis.normalized;
    }
}
