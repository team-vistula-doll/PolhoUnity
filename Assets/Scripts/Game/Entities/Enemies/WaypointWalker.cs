using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SimpleEnemy))]
public class WaypointWalker : MonoBehaviour
{
    [Delayed]
    public string PathFormula = "x";
    public float Length = 20, Angle;
    public int PathTypeSelection = 0;
    
    public Vector2 StartControl, EndControl, EndPos = Vector2.zero;

    [Range(0.2f, 50f)]
    public float StepSize = 0.5f;

    private IMoveable _moveableEntity;
    private List<Vector2> _path = new(), _tempPath = new();
    private Vector2 _nextWaypoint;
    private int _waypoints;
    private int _currentWaypoint;
    private Vector2 _startPosition;

    public event EventHandler<int> WaypointEvent;

    private bool _isMoving = true;
    private Action move;


    public void Start()
    {
        _startPosition = transform.position;
        _moveableEntity = GetComponent<IMoveable>();
        transform.position = _path[0];
        _tempPath = _path;
    }

    /// <summary>
    /// Validates and sets the path from a function
    /// </summary>
    /// <param name="isAdd">If true the path is added to the existing path, if false the path replaces the existing path</param>
    /// <param name="isTemp">If true sets the temp path, if false sets the real path</param>
    public void ValidatePath(bool isAdd = false, bool isTemp = true)
    {
        _startPosition = transform.position;
        if (isTemp)
            SetTempPath(isAdd: isAdd);
        else
            SetWaypointPath(isAdd: isAdd);
    }

    public void SetWaypointPath(List<Vector2> pathToSet = null, bool isAdd = false)
    {
        if (isAdd)
            _path.AddRange(pathToSet ?? _tempPath);
        else
            _path = pathToSet ?? _tempPath;
        if (pathToSet != null) _tempPath = _path;
        _currentWaypoint = 0;
        _waypoints = _path.Count;
        if (_path.Count != 0)
        {
            _nextWaypoint = _path[0];
            _isMoving = true;
        }
        move += Move;
    }

    public void SetTempPath(List<Vector2> pathToSet = null, bool isAdd = false)
    {
        if (pathToSet != null)
            _tempPath = pathToSet;
        else
        {
            Vector2 startPos = (isAdd && _path.Count() != 0) ? _path.Last() : _startPosition;
            switch (PathTypeSelection)
            {
                case 1:
                    _tempPath = WaypointPathCreator.GeneratePathFromCurve(startPos, EndPos, StartControl, EndControl, StepSize);
                    break;
                default:
                    _tempPath = WaypointPathCreator.GeneratePathFromExpression(startPos, Length, PathFormula, Angle, StepSize);
                    break;
            }
        }
    }

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
            while (_path.Count > _currentWaypoint && Vector2.Distance(_nextWaypoint, _path[_currentWaypoint]) < 0.01f)
                _path.RemoveAt(_currentWaypoint);
            _nextWaypoint = _path[_currentWaypoint];
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

    void OnDrawGizmosSelected()
    {
        if (_tempPath != null)
        {
            foreach (Vector2 point in _tempPath)
            {
                // Draws a blue line from this transform to the target
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(point, 0.03f);
            }
        }
        if (_path != null)
        {
            foreach (Vector2 point in _path)
            {
                // Draws a blue line from this transform to the target
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
    }
}
