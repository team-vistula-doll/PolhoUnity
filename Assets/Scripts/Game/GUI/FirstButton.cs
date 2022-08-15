using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//gdzieś w środku wierzę że jest łatwiejszy sposób niż dorabianie oddzielnego skryptu do tego ale chuj

    public class FirstButton : MonoBehaviour
    {
     
        Button button;
        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            button.Select();
        }
     
     
    }
