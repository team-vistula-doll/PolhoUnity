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

    private IMoveable _player;
    private IShootable _playerEmitter;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
        _playerEmitter = GetComponentInChildren<IShootable>();
    }

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    _player.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    //}

    public void Move(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
        //Debug.Log("X: " + moveVal.x + ", Y: " + moveVal.y);
        _player.Move(new Vector2(moveVal.x, moveVal.y));
    }

    public void Shoot(InputAction.CallbackContext context)
    {

        if (context.canceled)
        {
            _playerEmitter.canShoot = false;
            _playerEmitter.timer = 0;
        }
        else _playerEmitter.canShoot = true;
    }
}
