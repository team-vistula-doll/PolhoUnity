using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class DialogueContinueOnInput : MonoBehaviour
{
    private float timeWaited = 0;
    private int timeToWait = 1;
    private DialogueRunner _dialogueRunner;
    private DialogueUI _dialogueUI;
    private LineView _dialogueLineView;

    // Start is called before the first frame update
    void Start()
    {
        _dialogueRunner = GetComponent<DialogueRunner>();
        _dialogueUI = GetComponent<DialogueUI>();
        _dialogueLineView = GetComponent<LineView>();
    }

    public void Update()
    {
        if(timeWaited < timeToWait)
            timeWaited += Time.unscaledDeltaTime;
    }


    public void ResetCooldown()
    {
        timeWaited = 0;
    }

    public void ReadyToSkip()
    {
        timeWaited = timeToWait;
    }

    private void OnGUI()
    {
        if (_dialogueRunner.IsDialogueRunning)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyUp)
            {
                _dialogueLineView.ReadyForNextLine();
                //_dialogueUI.MarkLineComplete();
                //_dialogueRunner.Dialogue.Continue();
                ResetCooldown();
            }
        }
    }
}
