﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    Rigidbody2D Rigidbody2D{ get; set; }
    void Move(Vector2 input);
}
