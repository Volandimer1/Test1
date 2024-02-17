using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class MenuState : IGameState
{
    private GameStateMachine _gameStateMachine;

    AsyncOperationHandle<GameObject> CameraHandle;
    AsyncOperationHandle<GameObject> UICanvasHandle;

    private AsyncOperationHandle<SceneInstance> sceneHandle;

    private Camera canvasCamera;
    private Canvas uiCanvas;

    public MenuState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public async Task EnterState()
    {
        sceneHandle = Addressables.LoadSceneAsync("MenuScene", LoadSceneMode.Additive);
        await sceneHandle.Task;

        if (sceneHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load the MenuScene.");
            return;
        }

        Scene newScene = sceneHandle.Result.Scene;
        SceneManager.SetActiveScene(newScene);
        await PrepareNewScene();
    }

    public Task ExitState()
    {
        if (sceneHandle.Result.Scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneHandle.Result.Scene);
        }

        Addressables.Release(CameraHandle);
        Addressables.Release(UICanvasHandle);

        Addressables.Release(sceneHandle);

        return Task.CompletedTask;
    }

    private async Task PrepareNewScene()
    {
        CameraHandle = Addressables.InstantiateAsync("CanvasCamera");
        UICanvasHandle = Addressables.InstantiateAsync("MainMenuCanvas");

        await CameraHandle.Task;
        await UICanvasHandle.Task;

        GameObject cameraObject = CameraHandle.Result;
        canvasCamera = cameraObject.GetComponent<Camera>();

        GameObject uiCanvasObject = UICanvasHandle.Result;
        uiCanvas = uiCanvasObject.GetComponent<Canvas>();

        uiCanvas.worldCamera = canvasCamera;

        LevelContainerScrollView container = uiCanvas.GetComponentInChildren<LevelContainerScrollView>();
        container.Initialize(_gameStateMachine);
    }
}