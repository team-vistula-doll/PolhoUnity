using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemy : MonoBehaviour, IMoveable, IHitable
{
    public float Speed;
    public int HealthPoints = 1;
    public uint ScoreValue = 10;
    public AudioClip HitSound;
    public AudioClip DeathSound;

    [HideInInspector]
    public Rigidbody2D Rigidbody2D { get; set; }

    public void Move(Vector2 input)
    {
        if (input.magnitude > 1)
            input = input.normalized;

        Rigidbody2D.velocity = input * Speed * Time.deltaTime;
    }
    
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
    }

    public void OnHit()
    {
        // Future hit functionality
        if (--HealthPoints <= 0) OnDeath();
        else
        {
            Debug.Log("Enemy hit");
            if(HitSound)
                AudioSource.PlayClipAtPoint(HitSound, Camera.main.transform.position);
        }
    }

    public void OnDeath()
    {
        //Future death functionality
        Debug.Log("Enemy killed!");
        Player.Score += ScoreValue;
        if(DeathSound)
            AudioSource.PlayClipAtPoint(DeathSound, Camera.main.transform.position);
        gameObject.SetActive(false);
    }
}
