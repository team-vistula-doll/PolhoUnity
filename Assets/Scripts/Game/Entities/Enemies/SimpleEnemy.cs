using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class SimpleEnemy : MonoBehaviour, IEntity
{
    public float Speed;
    public int HealthPoints = 1;
    public uint ScoreValue = 10;
    public AudioClip HitSound;
    public AudioClip DeathSound;

    public Rigidbody2D Rigidbody2D { get; set; }

    private Player player;
    private AudioSource audioSource;

    public void Move(Vector2 input)
    {

        if (input.magnitude > 1)
            input = input.normalized;

        Rigidbody2D.velocity = Speed * Time.deltaTime * input;
    }
    
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnHit()
    {
        if (--HealthPoints <= 0) OnDeath();
        else
        {
            Debug.Log("Enemy hit");
            if (HitSound)
                audioSource.PlayOneShot(HitSound);
        }
    }

    public void OnDeath()
    {
        Debug.Log("Enemy killed!");
        player.Score += ScoreValue;
        if(DeathSound)
            AudioSource.PlayClipAtPoint(DeathSound, Camera.main.transform.position);
        DelayedActions.AddDelayedAction(delegate { gameObject.SetActive(false); });
    }
}
