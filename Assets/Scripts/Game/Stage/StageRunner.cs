using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRunner : MonoBehaviour
{
    public Stage StageToRun;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(StageToRun.StageScript(new StageArgs() {EnemyManager = GetComponent<EnemyManager>()}));
    }

}
