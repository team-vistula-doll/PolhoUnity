using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillZone : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag!="Player")
            collider.gameObject.SetActive(false);
    }
}
