using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IShootable
{
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }
}