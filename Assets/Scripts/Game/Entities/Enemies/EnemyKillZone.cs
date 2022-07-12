using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillZone : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.transform.parent.gameObject.tag != "Player")
            collider.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
