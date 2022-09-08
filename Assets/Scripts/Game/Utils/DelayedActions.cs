using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedActions : MonoBehaviour
{
    private static Queue<Action> actions;

    public static void AddDelayedAction(Action action)
    {
        actions.Enqueue(action);
    }

    private void Start()
    {
        actions = new Queue<Action>();
    }

    void LateUpdate()
    {
        while (actions.Count > 0)
            actions.Dequeue()?.Invoke();
    }
}
