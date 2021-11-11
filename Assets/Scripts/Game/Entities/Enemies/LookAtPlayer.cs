using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public float Delay = 0.1f;
    private Transform _trackedPlayerTransform;

    public void Start()
    {
        _trackedPlayerTransform = FindObjectOfType<Player>().transform;
        StartCoroutine(LookCoroutine());
    }

    IEnumerator LookCoroutine()
    {
        while (true)
        {
            Vector3 difference = _trackedPlayerTransform.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            yield return new WaitForSeconds(Delay);
        }
    }

}
