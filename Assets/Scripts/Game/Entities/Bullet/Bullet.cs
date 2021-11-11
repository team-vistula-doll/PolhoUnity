using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 1f;
    //protected Rigidbody2D _rigidbody2D;
    public void Start()
    {
        //_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        Move();
    }
    public virtual void Move()
    {
        transform.position += transform.up * Speed * Time.deltaTime;
    }
}
