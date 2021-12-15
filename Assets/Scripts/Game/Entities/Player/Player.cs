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

    private static int lives = 3;
    private static uint score = 0;

    private static TMP_Text scoreText;
    private static TMP_Text livesText;
    
    public static int Lives
    {
        get => lives;

        set
        {
            if (value <= 0)
            {
                lives = value;
                livesText.text = "Dead";
                GameStateManager.gameState = GameState.DEAD;
            }
            else
            {
                lives = value;
                livesText.text = "Lives: " + lives; // Update life indicator
            }
        }
    }

    public static uint Score
    {
        get => score;
        set
        {
            score = value;
            scoreText.text = "Score: " + score; // Update score indicator
        }
    }

    public Rigidbody2D Rigidbody2D { get; set; }
    public void Move(Vector2 input)
    {
        if(input.magnitude > 1)
            input = input.normalized;
        
        Rigidbody2D.velocity = new Vector2(input.x, input.y) * Speed * Time.deltaTime;
    }

    public static void Hit()
    {
        Lives -= 1;
        // Future hit functionality
    }

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        scoreText = GameObject.FindWithTag("ScoreText").GetComponent<TMP_Text>();
        livesText = GameObject.FindWithTag("LivesText").GetComponent<TMP_Text>();
    }
}
