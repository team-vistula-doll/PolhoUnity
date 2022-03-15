using System.Collections;
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

        Rigidbody2D.velocity = Speed * Time.deltaTime * input;
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
        if (--HealthPoints <= 0) OnDeath();
        else Debug.Log("Enemy hit");
    }

    public void OnDeath()
    {
        //Future death functionality
        Debug.Log("Enemy killed!");
        gameObject.SetActive(false);
    }
}
