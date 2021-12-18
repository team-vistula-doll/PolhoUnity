using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IShootable
{
    public float Timer { get; set; }
    public Range FireRate { get; set; }
    public bool CanShoot { get; set; }
    public DanmakuSet Set { get; set; }
}