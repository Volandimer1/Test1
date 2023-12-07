using UnityEngine;

public class BootStraper : MonoBehaviour
{
    [SerializeField] private Updater _updater;
    [SerializeField] private BootConfigSO _config;
    private GameStateMachine gameStateMachine;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        gameStateMachine = new GameStateMachine(_updater);

        InitializationState initializationState = gameStateMachine.States[typeof(InitializationState)] as InitializationState;
        initializationState.Initialize(_config.LevelID);

        gameStateMachine.TransitionToState<InitializationState>();
    }
}