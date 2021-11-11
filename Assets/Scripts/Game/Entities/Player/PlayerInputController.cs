using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMoveable))]
public class PlayerInputController : MonoBehaviour
{
    private IMoveable _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IMoveable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _player.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }
}
