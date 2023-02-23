using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IEntity), typeof(IShootable))]
public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveVal { get; set; }
    public float FocusMultiplier { get; set; } = 1f;

    public PlayerInput input;
    public Animator animator;
        
    private IMoveable _player;
    private PlayerDanmakuEmitter _playerEmitter;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
        _playerEmitter = GetComponentInChildren<PlayerDanmakuEmitter>();
        EnableMap("Pause Menu");
    }

    public void Move(InputAction.CallbackContext context)
    {
        MoveVal = context.ReadValue<Vector2>();
        _player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Focus(InputAction.CallbackContext context)
    {
        FocusMultiplier = context.canceled ? 1f : 0.5f;
        _player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled) // If button released
        {
            _playerEmitter.CanShoot = false;
            
            // This lets the player fire twice as fast if they spam the button
            if (_playerEmitter.Timer > 0.5f/_playerEmitter.FireRate.GetValue())
                _playerEmitter.Timer = 0.5f / _playerEmitter.FireRate.GetValue();
        }
        else _playerEmitter.CanShoot = true;
    }

    public void Update()
    {
        animator.SetFloat("X speed", MoveVal.x);
    }

    public void Restart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisableMap(string mapName) => input.actions.FindActionMap(mapName).Disable();
    public void EnableMap(string mapName) => input.actions.FindActionMap(mapName).Enable();
}
