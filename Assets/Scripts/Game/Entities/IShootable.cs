using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IShootable
{
    public float timer { get; set; }
    public Range FireRate { get; set; }
    public bool canShoot { get; set; }
}
