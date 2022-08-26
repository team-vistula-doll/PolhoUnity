using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class SimpleDialogueView : LineView
{
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueCompleted;
    
    public override void DialogueStarted()
    {
        base.DialogueStarted();
        OnDialogueStarted?.Invoke();
    }

    public override void DialogueComplete()
    {
        base.DialogueComplete();
        OnDialogueStarted?.Invoke();
    }
}

