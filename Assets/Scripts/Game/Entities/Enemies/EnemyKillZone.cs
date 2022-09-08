using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillZone : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D col)
    {
        var parent = col.transform.parent;
        if(!parent.CompareTag("Player"))
            parent.gameObject.SetActive(false);
    }
}
