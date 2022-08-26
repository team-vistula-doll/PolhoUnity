using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Dialogue : MonoBehaviour
{
    public GameObject DialoguePanel;
    
    private static DialogueRunner dialRunner;

    public static Action OnDialStart;
    public Action OnDialComplete;
    public Action OnNodeStart;
    public Action OnNodeComplete;
    public Action OnCommand;

    public void DialStart()
    {
        OnDialStart?.Invoke();
    }

    public void DialComplete()
    {
        OnDialComplete?.Invoke();
    }

    public void NodeStart()
    {
        OnNodeStart?.Invoke();
    }

    public void NodeComplete()
    {
        OnNodeComplete?.Invoke();
    }

    public void Command()
    {
        OnCommand?.Invoke();
    }

    private void Start()
    {
        dialRunner = GetComponent<DialogueRunner>();

        OnDialStart += delegate { DialoguePanel.SetActive(true); };
        OnDialComplete += delegate { DialoguePanel.SetActive(false); };

        PlayDialogue("Test2");
    }

    public static void PlayDialogue(string nodeName)
    {
        OnDialStart?.Invoke();
        dialRunner.StartDialogue(nodeName);
    }
}
