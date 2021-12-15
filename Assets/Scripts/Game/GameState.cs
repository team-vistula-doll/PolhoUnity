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
    public int Difficulty { get; set; }
    
    public static event Action<GameState> GameStateChanged;
    private static GameStateManager instance = null;
    private static GameState _currentState;

    public static GameState gameState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            GameStateChanged?.Invoke(_currentState);
        }
    }

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameStateManager();
            return instance;
        }
    }
}
