using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    public Dictionary<Type, IGameState> States { get; private set; }
    private IGameState _currentState;
    private Updater _updater;

    public GameStateMachine(Updater updater)
    {
        _updater = updater;
        States = new Dictionary<Type, IGameState>();
        InitializeStates();
    }

    private void InitializeStates()
    {
        States.Add(typeof(InitializationState), new InitializationState(this));
        States.Add(typeof(MenuState), new MenuState(this));
        States.Add(typeof(LevelState), new LevelState(this));
    }

    public void TransitionToState<T>() where T : IGameState
    {
        Type stateType = typeof(T);

        if (!States.ContainsKey(stateType))
        {
            Debug.LogError($"State of type {stateType.Name} is not registered.");
            return;
        }

        _currentState?.ExitState();
        _currentState = States[stateType];
        _currentState?.EnterState();
    }
}