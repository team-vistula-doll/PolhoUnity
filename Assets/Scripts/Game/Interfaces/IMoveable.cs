using UnityEngine;
using WaypointPath;

public interface IMoveable
{
    Transform transform { get; }
    void Move(Waypoint input);
}
