using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public float Speed;
    public AudioClip HitSound;
    public AudioClip ShootSound;
    public Rigidbody2D Rigidbody2D { get; set; }
    public PlayerInputController playerInputController;

    private AudioClip hitSound;
    private AudioClip shootSound;
    private AudioSource audioSource;
    private int lives = 3;
    private uint score = 0;

    public Animator animator;

    public static AudioSource _AudioSource;

    public PlayerSprite playerSprite;

    private new PlayerDanmakuCollider collider;

    private bool isDeathResetPosition;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        _AudioSource = GetComponent<AudioSource>();
        collider = GetComponentInChildren<PlayerDanmakuCollider>();
        shootSound = ShootSound;
        isDeathResetPosition = false;
    }

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
        if (input.magnitude > 1)
            input = input.normalized;

        Rigidbody2D.velocity = Speed * Time.deltaTime * input;
    }

    public IEnumerator OnHit()
    {
        collider.IsInvincible = true;
        playerInputController.DisableMap("Gameplay");
        Lives -= 1;
        if(HitSound)
            _AudioSource.PlayOneShot(HitSound);
        playerSprite.ColorPlayerSprite(default, 2);
        animator.SetTrigger("Death");
        transform.localScale = new Vector3(2, 2, 2);
        yield return new WaitUntil(() => isDeathResetPosition); //WaitForSeconds(1.5f);

        transform.localScale = Vector3.one;
        transform.position = new Vector2(0, -4);
        playerInputController.EnableMap("Gameplay");
        isDeathResetPosition = false;
        Color normalColor = new Color(1, 1, 1);
        playerSprite.ColorPlayerSprite(normalColor, 2);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            playerSprite.ColorPlayerSprite();
            yield return new WaitForSeconds(0.1f);
            playerSprite.ColorPlayerSprite(normalColor);
        }

        collider.IsInvincible = false;
    }

    public void DeathResetPosition() => isDeathResetPosition = true;
}
