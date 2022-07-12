using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 1f;

    public void Update()
    {
        Move();
    }
    
    public virtual void Move()
    {
        transform.position += transform.up * Speed * Time.deltaTime;
    }
}
