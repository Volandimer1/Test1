using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameStateMachine
{
    public Updater Updater;
    public AudioManager AudioManager;
    public Dictionary<Type, Func<IGameState>> StateFactories { get; private set; }

    private IGameState _currentState;

    public GameStateMachine(Updater updater, AudioManager audioManager)
    {
        Updater = updater;
        AudioManager = audioManager;
        StateFactories = new Dictionary<Type, Func<IGameState>>();
        InitializeStateFactories();
    }

    private void InitializeStateFactories()
    {
        StateFactories.Add(typeof(InitializationState), () => new InitializationState(this));
        StateFactories.Add(typeof(MenuState), () => new MenuState(this));
        StateFactories.Add(typeof(LevelState), () => new LevelState(this));
    }

    public async Task TransitionToState<T>() where T : IGameState
    {
        Type stateType = typeof(T);

        if (!StateFactories.ContainsKey(stateType))
        {
            Debug.LogError($"State of type {stateType.Name} is not registered.");
            return;
        }

        IGameState previousState = _currentState;
        _currentState = StateFactories[stateType].Invoke();

        if (_currentState != null)
        {
            await _currentState.EnterState();
            previousState?.ExitState();
        }
    }

    public async Task TransitionToState(IGameState state)
    {
        IGameState previousState = _currentState;
        _currentState = state;

        if (_currentState != null)
        {
            await _currentState.EnterState();
            previousState?.ExitState();
        }
    }
}