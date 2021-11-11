using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    RUNNING,
    DEAD,
    PAUSED,
    DIALOGUE
}
public class GameStateManager
{
    private GameState _currentState;

    private int _lifes;

    public int Difficulty { get; set; }

    public int Lifes
    { 
        get { return _lifes; }

        set
        {
            if (value < 0)
            {
                SetState(GameState.DEAD);
                _lifes = 0;
            }
            else
                _lifes = Mathf.Min(value, 5);

            LifesChanged.Invoke(_lifes);
        }
    }
    
    public event Action<GameState> GameStateChanged;
    public event Action<int> LifesChanged;

    private static GameStateManager instance = null;

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameStateManager();
            return instance;
        }
    }

    private GameStateManager()
    {
        _lifes = 3;
    }

    public void SetState(GameState state)
    {
        _currentState = state;
        GameStateChanged?.Invoke(_currentState);
    }
}
