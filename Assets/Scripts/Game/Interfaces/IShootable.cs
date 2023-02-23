using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IShootable
{
    public Range FireRate { get; set; }
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }
    public IFireable Fireable { get; set; }
    public DanmakuConfig Config { get; set; }
}
