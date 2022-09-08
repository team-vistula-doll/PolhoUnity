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

    private IEntity player;
    private PlayerDanmakuEmitter playerEmitter;
    
    void Start()
    {
        player = GetComponent<IEntity>();
        playerEmitter = GetComponentInChildren<PlayerDanmakuEmitter>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        MoveVal = context.ReadValue<Vector2>();
        player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Focus(InputAction.CallbackContext context)
    {
        FocusMultiplier = context.canceled ? 1f : 0.5f;
        player.Move(new Vector2(MoveVal.x * FocusMultiplier, MoveVal.y * FocusMultiplier));
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled) // If button released
        {
            playerEmitter.CanShoot = false;
            
            // This lets the player fire twice as fast if they spam the button
            if (playerEmitter.Timer > 0.5f/playerEmitter.FireRate.GetValue())
                playerEmitter.Timer = 0.5f / playerEmitter.FireRate.GetValue();
        }
        else playerEmitter.CanShoot = true;
    }
}
