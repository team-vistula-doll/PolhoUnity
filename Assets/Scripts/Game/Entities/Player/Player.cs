using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IMoveable
{
    public float Speed;

    [HideInInspector]
    public Rigidbody2D Rigidbody2D { get; set; }

    public void Move(Vector2 input)
    {
       
        if(input.magnitude>1)
            input = input.normalized;

        Rigidbody2D.velocity = new Vector2(input.x, input.y) * Speed; //TODO: Yes, without Time.deltaTime because with it the speed varies between framerates, but maybe there's a solution
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
}
