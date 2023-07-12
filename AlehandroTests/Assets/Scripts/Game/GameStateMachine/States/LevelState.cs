using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelState : IGameState
{
    private GameStateMachine _gameStateMachine;

    private Camera canvasCamera;
    private Canvas uiCanvas;
    private Canvas fieldCanvas;
    private ObjectPooller _objectPooller;
    private TextAsset _levelDataFile;

    public LevelState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public void Initialize(ObjectPooller objectPooller, TextAsset levelDataFile)
    {
        _objectPooller = objectPooller;
        _levelDataFile = levelDataFile;
    }

    public void EnterState()
    {
        Scene PreviousScene = SceneManager.GetActiveScene();

        Addressables.LoadSceneAsync("LevelScene", LoadSceneMode.Additive).Completed += (asyncHandle) =>
        {
            if (asyncHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load the LevelScene.");
                return;
            }

            SceneManager.UnloadSceneAsync(PreviousScene);
            PrepareNewScene();
        };
    }

    public void ExitState()
    {

    }

    private async void PrepareNewScene()
    {
        AsyncOperationHandle<GameObject> CameraHandle = Addressables.InstantiateAsync("CanvasCamera");
        AsyncOperationHandle<GameObject> UICanvasHandle = Addressables.InstantiateAsync("UICanvas");
        AsyncOperationHandle<GameObject> FieldCanvasHandle = Addressables.InstantiateAsync("FieldCanvas");

        await CameraHandle.Task;
        await UICanvasHandle.Task;
        await FieldCanvasHandle.Task;

        GameObject cameraObject = CameraHandle.Result;
        canvasCamera = cameraObject.GetComponent<Camera>();

        GameObject uiCanvasObject = UICanvasHandle.Result;
        uiCanvas = uiCanvasObject.GetComponent<Canvas>();
        UIView uiView = uiCanvasObject.GetComponent<UIView>();

        GameObject fieldCanvasObject = FieldCanvasHandle.Result;
        fieldCanvas = fieldCanvasObject.GetComponent<Canvas>();

        uiCanvas.worldCamera = canvasCamera;
        fieldCanvas.worldCamera = canvasCamera;

        LevelData levelData = new LevelData();
        levelData.LoadFromTextAsset(_levelDataFile);
        GoalsManager goalsManager = new GoalsManager(levelData);
        uiView.Initialize(goalsManager);
    }
}