using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IMoveable
{
    public float Speed;
    public AudioClip HitSound;
    public AudioClip ShootSound;
    
    private static AudioClip hitSound;
    public static AudioClip shootSound;

    [HideInInspector]
    public Rigidbody2D Rigidbody2D { get; set; }

    private static int lives = 3;
    private static uint score = 0;

    public static int Lives
    {
        get => lives;

        set
        {
            if (value <= 0)
            {
                lives = value;
                LivesText.Text = "Dead";
                GameStateManager.gameState = GameState.DEAD;
            }
            else
            {
                lives = value;
                LivesText.Text = "Lives: " + lives; // Update life indicator
            }
        }
    }

    public static uint Score
    {
        get => score;
        set
        {
            score = value;
            ScoreText.Text = "Score: " + score; // Update score indicator
        }
    }

    public void Move(Vector2 input)
    {
        if(input.magnitude > 1)
            input = input.normalized;

        Rigidbody2D.velocity = new Vector2(input.x, input.y) * Speed;
    }

    public static void OnHit()
    {
        Lives -= 1;
        if(hitSound)
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
        // Future hit functionality
    }

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        hitSound = HitSound;
        shootSound = ShootSound;
    }
}
