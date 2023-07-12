using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
public class InitializationState : IGameState
{
    private GameStateMachine _gameStateMachine;

    public InitializationState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
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
        FieldObjectsPrefabsSO fieldObjectsPrefabsSO;
        LevelsContainerSO levelsContainerSO;

        AsyncOperationHandle<FieldObjectsPrefabsSO> handleFieldObjectsPrefabsSO = Addressables.LoadAssetAsync<FieldObjectsPrefabsSO>("FieldObjectsPrefabsSO");
        AsyncOperationHandle<LevelsContainerSO> handleLevelsContainerSO = Addressables.LoadAssetAsync<LevelsContainerSO>("LevelsContainerSO");

        await handleFieldObjectsPrefabsSO.Task;
        await handleLevelsContainerSO.Task;

        if (handleFieldObjectsPrefabsSO.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load ScriptableObject: " + handleFieldObjectsPrefabsSO.OperationException);
            return;
        }

        if (handleLevelsContainerSO.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load ScriptableObject: " + handleLevelsContainerSO.OperationException);
            return;
        }

        fieldObjectsPrefabsSO = handleFieldObjectsPrefabsSO.Result;
        levelsContainerSO = handleLevelsContainerSO.Result;

        FieldObjectFactory fieldObjectFactory = new FieldObjectFactory(fieldObjectsPrefabsSO);
        ObjectPooller objectPooller = new ObjectPooller(fieldObjectFactory);

        LevelState levelState = _gameStateMachine.States[typeof(LevelState)] as LevelState;

        if (levelState != null)
        {
            levelState.Initialize(objectPooller, levelsContainerSO.Levels[0]);
        }

        Addressables.Release(handleFieldObjectsPrefabsSO);
        Addressables.Release(handleLevelsContainerSO);

        _gameStateMachine.TransitionToState<LevelState>();
    }
}