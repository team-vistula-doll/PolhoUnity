﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : MonoBehaviour, IMoveable, IHitable
{
    public float Speed;
    public int HealthPoints = 1;

    [HideInInspector]
    public Rigidbody2D Rigidbody2D { get; set; }

    public void Move(Vector2 input)
    {

        if (input.magnitude > 1)
            input = input.normalized;

        Rigidbody2D.velocity = new Vector2(input.x, input.y) * Speed * Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHit()
    {
        // Future hit functionality
        Debug.Log("Enemy hit");
        if (--HealthPoints <= 0) OnDeath();
    }

    public void OnDeath()
    {
        //Future death functionality
        Debug.Log("Enemy killed!");
        gameObject.SetActive(false);
    }
}
