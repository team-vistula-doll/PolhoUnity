using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{

    public float delay = 0.1f;
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int step = 100;
        int count = 0;
        while (count<4000)
        {
            for(int i = 0; i<step;i++)
                Instantiate(prefab, transform.position, Quaternion.Euler(0,0,Random.value*360.0f),transform);
            count += step;
            Debug.Log(count);
            yield return new WaitForSeconds(delay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
