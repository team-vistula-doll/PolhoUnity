using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eGameState
{
    RUNNING,
    DEAD,
    PAUSED,
    DIALOGUE
}

public static class GameState
{
    public static int Difficulty { get; set; }
    public static event Action<eGameState> OnGameStateChange;

    private static eGameState _state;
    
    public static eGameState State
    {
        get => _state;
        set
        {
            _state = value;
            OnGameStateChange?.Invoke(_state);
        }
    }
}
