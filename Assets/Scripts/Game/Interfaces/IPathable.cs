using UnityEngine;
using WaypointPath;

public interface IPathable
{
    Transform transform { get; }
    void Move(Waypoint input);
}
