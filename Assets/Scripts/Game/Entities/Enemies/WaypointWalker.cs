using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleEnemy))]
public class WaypointWalker : MonoBehaviour
{
    [SerializeField]
    private string _pathFormula;
    [SerializeField]
    private float _length;
    [SerializeField]
    private float _angle;

    private IMoveable _moveableEntity;
    private List<Vector2> _path;
    private Vector2 _nextWaypoint;
    private int _waypoints;
    private int _currentWaypoint;

    private Vector2 _startPosition;
    public void OnValidate()
    {
        _startPosition = transform.position;
        SetWaypointPath(WaypointPathCreator.GeneratePathFromExpression(_startPosition, _length, _pathFormula, _angle));
    }

    public void Start()
    {
        _startPosition = transform.position;
        _moveableEntity = GetComponent<IMoveable>();
        transform.position = _path[0];
    }

    public void SetWaypointPath(List<Vector2> pathToSet)
    {
        _path = pathToSet;
        _currentWaypoint = 0;
        _waypoints = _path.Count;
        _nextWaypoint = _path[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_moveableEntity!=null && _moveableEntity.Rigidbody2D!=null)
            _moveableEntity.Move(GetCurrentAxis());
    }

    public bool WaypointCheck() //check if you are close enough to set nextWaypoint to next waypoint in path
    {
        if(Vector2.Distance(transform.position, _nextWaypoint) < 0.1f && _currentWaypoint<_waypoints)
        {
            _currentWaypoint++;
            _nextWaypoint = _path[_currentWaypoint];
        }
        else if(_currentWaypoint>=_waypoints)
        {
            return false;
        }
        return true;

    }

    public Vector2 GetCurrentAxis()
    {
        if (WaypointCheck())
        {
            Vector2 axis = _nextWaypoint - new Vector2(_moveableEntity.Rigidbody2D.position.x,
                _moveableEntity.Rigidbody2D.position.y);

            return axis.normalized;
        }
        else
            return new Vector2(0, 0);

    }

    void OnDrawGizmosSelected()
    {
        if(_path!=null)
        {
            foreach (Vector2 point in _path)
            {
                // Draws a blue line from this transform to the target
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(point, 0.05f);
            }
        }

    }

}
