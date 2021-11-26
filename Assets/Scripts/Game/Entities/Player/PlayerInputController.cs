using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IMoveable))]
public class PlayerInputController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveVal;

    private IMoveable _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
    }

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    _player.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    //}

    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
        //Debug.Log("X: " + moveVal.x + ", Y: " + moveVal.y);
        _player.Move(new Vector2(moveVal.x, moveVal.y));
    }

}
