using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IMoveable), typeof(IShootable))]
public class PlayerInputController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 MoveVal;
    [HideInInspector]
    public float FocusMultiplier = 1f;

    private IMoveable _player;
    private PlayerDanmakuEmitter _playerEmitter;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
        _playerEmitter = GetComponentInChildren<PlayerDanmakuEmitter>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        MoveVal = context.ReadValue<Vector2>();
        _player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Focus(InputAction.CallbackContext context)
    {
        if (context.canceled) FocusMultiplier = 1f;
        else FocusMultiplier = 0.5f;

        _player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled) //if button released
        {
            _playerEmitter.CanShoot = false;
            
            //This lets the player fire twice as fast if they spam the button
            if(_playerEmitter.Timer > 0.5f/_playerEmitter.FireRate.GetValue())
                _playerEmitter.Timer = 0.5f / _playerEmitter.FireRate.GetValue();
        }
        else _playerEmitter.CanShoot = true;
    }
    //pozostałości po resecie gry pod escapem

    //public void Restart(InputAction.CallbackContext context)
    //{
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}
}
