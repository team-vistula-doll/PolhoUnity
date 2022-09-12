using System;
using System.Collections.Generic;
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
    private List<Vector2> _path, _tempPath;
    private Vector2 _nextWaypoint;
    private int _waypoints;
    private int _currentWaypoint;
    private Vector2 _startPosition;

    public event EventHandler<int> WaypointEvent;

    private bool _isMoving = true;
    private Action move;

    /// <summary>
    /// Validates and sets the path from a function
    /// </summary>
    /// <param name="pathType">0: function, 1: curve</param>
    /// <param name="isTemp">If true sets the temp path, if false sets the real path</param>
    public void ValidatePath(int pathType = 0, bool isTemp = true)
    {
        _startPosition = transform.position;
        switch (pathType)
        {
            case 1:
                if (isTemp)
                    _tempPath = WaypointPathCreator.GeneratePathFromCurve(_startPosition, EndPos, StartControl, EndControl, StepSize);
                else
                    SetWaypointPath();
                break;
            default:
                if (isTemp)
                    _tempPath = WaypointPathCreator.GeneratePathFromExpression(_startPosition, Length, PathFormula, Angle, StepSize);
                else
                    SetWaypointPath();
                break;
        }
    }

    public void Start()
    {
        _startPosition = transform.position;
        _moveableEntity = GetComponent<IMoveable>();
        transform.position = _path[0];
        _tempPath = _path;
    }

    public void SetWaypointPath(List<Vector2> pathToSet = null)
    {
        _path = pathToSet ?? _tempPath;
        if (pathToSet != null) _tempPath = _path;
        _currentWaypoint = -1;
        _waypoints = _path.Count;
        if (_path.Count != 0)
        {
            _nextWaypoint = _path[0];
            _isMoving = true;
        }
        move += Move;
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

    public void ChangeNextWaypoint() //changes _nextWaypoint if you are close enough to the next waypoint
    {
        if (Vector2.Distance(transform.position, _nextWaypoint) < 0.1f && _currentWaypoint + 1 < _waypoints)
        {
            while (_path.Count > _currentWaypoint + 1 && Vector2.Distance(_nextWaypoint, _path[_currentWaypoint + 1]) < 0.01f)
                _path.RemoveAt(_currentWaypoint + 1);
            _nextWaypoint = _path[++_currentWaypoint];
            WaypointEvent?.Invoke(this, _currentWaypoint);
        }
        else if (_currentWaypoint + 1 >= _waypoints)
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
