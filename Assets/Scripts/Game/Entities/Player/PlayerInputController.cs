using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IMoveable))]
[RequireComponent(typeof(IShootable))]
public class PlayerInputController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveVal;
    [HideInInspector]
    public float focus = 1f;

    private IMoveable _player;
    private IShootable _playerEmitter;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
        _playerEmitter = GetComponentInChildren<IShootable>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
        _player.Move(new Vector2(moveVal.x * focus, moveVal.y * focus));
    }

    public void Focus(InputAction.CallbackContext context)
    {
        if (context.canceled) focus = 1f;
        else focus = 0.5f;

        _player.Move(new Vector2(moveVal.x * focus, moveVal.y * focus));
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled) //if button released
        {
            _playerEmitter.canShoot = false;
            
            //This lets the player fire twice as fast if they spam the button
            if(_playerEmitter.timer > 0.5f/_playerEmitter.FireRate.GetValue())
                _playerEmitter.timer = 0.5f / _playerEmitter.FireRate.GetValue();
        }
        else _playerEmitter.canShoot = true;
    }
}
