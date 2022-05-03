using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * 0.33f;
    }
}
