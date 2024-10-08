using UnityEngine;
using WaypointPath;

[RequireComponent(typeof(AudioSource))]
public class SimpleEnemy : MonoBehaviour, IEntity
{
    public float Speed;
    public int HealthPoints = 1;
    public uint ScoreValue = 10;
    public AudioClip HitSound;
    public AudioClip DeathSound;

    private Player player;
    private AudioSource audioSource;
    
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Move(Waypoint waypoint)
    {
        if (waypoint.Speed.HasValue) Speed = waypoint.Speed.Value;
        if (waypoint.Acceleration.HasValue) Speed += waypoint.Acceleration.Value;
        transform.position = Vector2.MoveTowards(transform.position, waypoint.Position, Speed * Time.deltaTime);
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
