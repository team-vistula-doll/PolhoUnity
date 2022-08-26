using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Player : MonoBehaviour, IEntity
{
    public float Speed;
    public AudioClip HitSound;
    public AudioClip ShootSound;

    private AudioClip hitSound;
    private AudioSource audioSource;
    private int lives = 3;
    private uint score = 0;

    public AudioClip shootSound { get; set; }
    public Rigidbody2D Rigidbody2D { get; set; }
    public int Lives
    {
        get => lives;

        set
        {
            lives = value;
            if (value <= 0)
            {
                LivesText.Text = "Dead";
                GameState.State = eGameState.DEAD;
            }
            else
                LivesText.Text = "Lives: " + lives;
        }
    }
    public uint Score
    {
        get => score;
        set
        {
            score = value;
            ScoreText.Text = "Score: " + score;
        }
    }

    public void Move(Vector2 input)
    {
        if(input.magnitude > 1)
            input = input.normalized;
        
        Rigidbody2D.velocity = new Vector2(input.x, input.y) * Speed;
    }

    public void OnHit()
    {
        Lives -= 1;
        
        if(hitSound)
            audioSource.PlayOneShot(hitSound);
    }

    public void OnDeath()
    {
        // TODO: Death
    }

    public void PlayShootSound()
    {
        if(shootSound)
            audioSource.PlayOneShot(ShootSound);
    }
    
    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        hitSound = HitSound;
        shootSound = ShootSound;
    }
}
