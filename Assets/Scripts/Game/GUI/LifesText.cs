using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesText : MonoBehaviour
{
    private TMPro.TMP_Text _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMPro.TMP_Text>();
        GameStateManager.Instance.LifesChanged += UpdateText;
    }

    private void UpdateText(int obj)
    {
        _text.text = "Lifes: " + obj;
    }

}
