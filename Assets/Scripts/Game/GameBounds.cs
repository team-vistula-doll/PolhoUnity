using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class BoundsExtension
{
    public static bool ContainBounds(this Bounds bounds, Bounds target)
    {
        return bounds.Contains(target.min) && bounds.Contains(target.max);
    }
}

[RequireComponent(typeof(BoxCollider2D))]
public class GameBounds : MonoBehaviour
{
    public BoxCollider2D BoxCollider { get; private set; }
    
    public void Start()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
    }
    
    public bool IsInBounds(Vector2 position)
    {
        return BoxCollider.bounds.Contains(position);
    }

    public bool IsInBounds(Collider2D col)
    {
        return BoxCollider.bounds.ContainBounds(col.bounds);
    }
}
