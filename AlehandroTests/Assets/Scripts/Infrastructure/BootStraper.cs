using UnityEngine;

public class BootStraper : MonoBehaviour
{
    [SerializeField] private Updater _updater;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        GameStateMachine gameStateMachine = new GameStateMachine(_updater);
        gameStateMachine.TransitionToState<InitializationState>();
    }
}