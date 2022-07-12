using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

public interface IHitable
{
    public void OnHit();
    public void OnDeath();
}