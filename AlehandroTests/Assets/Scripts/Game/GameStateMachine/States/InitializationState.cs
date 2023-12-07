using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
public class InitializationState : IGameState
{
    private GameStateMachine _gameStateMachine;
    private int _levelID;

    public InitializationState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public void Initialize(int levelID)
    {
        _levelID = levelID;
    }

    public void EnterState()
    {
        Addressables.InitializeAsync().Completed += OnAddressablesInitComplited;

    }

    public void ExitState()
    {
        
    }

    private async void OnAddressablesInitComplited(AsyncOperationHandle<IResourceLocator> obj)
    {
        LevelsContainerSO levelsContainerSO;

        AsyncOperationHandle<LevelsContainerSO> handleLevelsContainerSO = Addressables.LoadAssetAsync<LevelsContainerSO>("LevelsContainerSO");

        await handleLevelsContainerSO.Task;

        if (handleLevelsContainerSO.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load ScriptableObject: " + handleLevelsContainerSO.OperationException);
            return;
        }

        levelsContainerSO = handleLevelsContainerSO.Result;

        LevelState levelState = _gameStateMachine.States[typeof(LevelState)] as LevelState;

        if (levelState != null)
        {
            levelState.Initialize(levelsContainerSO.Levels[_levelID]);
        }
        Addressables.Release(handleLevelsContainerSO);

        _gameStateMachine.TransitionToState<LevelState>();
    }
}