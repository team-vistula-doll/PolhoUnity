using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesText : MonoBehaviour
{
    private static TMP_Text livesText;

    void Start()
    {
        livesText = GetComponent<TMP_Text>();
    }

    public static string Text
    {
        get => livesText.text;
        set => livesText.text = value;
    }
}
