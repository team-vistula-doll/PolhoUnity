using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    public Rigidbody2D Rigidbody2D{ get; set; }
    public void Move(Vector2 input);
}
