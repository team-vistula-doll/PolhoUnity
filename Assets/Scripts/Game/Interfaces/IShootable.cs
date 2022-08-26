using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IShootable
{
    float Timer { get; set; }
    DanmakuSet Set { get; set; }
}