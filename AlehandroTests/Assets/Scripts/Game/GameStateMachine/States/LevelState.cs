using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LevelState : IGameState
{
    private GameStateMachine _gameStateMachine;

    AsyncOperationHandle<GameObject> CameraHandle;
    AsyncOperationHandle<GameObject> UICanvasHandle;
    AsyncOperationHandle<GameObject> FieldCanvasHandle;
    AsyncOperationHandle<GameObject> InputControllerGOHandle;
    AsyncOperationHandle<GameObject> VictoryWindowHandle;
    AsyncOperationHandle<GameObject> GameOverWindowHandle;
    AsyncOperationHandle<FieldObjectsPrefabsSO> handleFieldObjectsPrefabsSO;

    private AsyncOperationHandle<SceneInstance> sceneHandle;

    private Camera canvasCamera;
    private Canvas uiCanvas;
    private Canvas fieldCanvas;
    private ObjectPooller _objectPooller;
    private TextAsset _levelDataFile;
    private InputController _inputController;
    private GoalsManager _goalsManager;
    private LevelData _levelData;
    private UIView _uiView;
    private Field _field;

    GameObject _VictoryWindow;
    GameObject _GameOverWindow;

    public LevelState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public void Initialize(TextAsset levelDataFile)
    {
        _levelDataFile = levelDataFile;
    }

    public void EnterState()
    {
        Scene PreviousScene = SceneManager.GetActiveScene();

        sceneHandle = Addressables.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
        sceneHandle.Completed += (asyncHandle) =>
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
        _goalsManager.OnVictoryAchived -= Victory;
        _goalsManager.OnGameOver -= GameOver;

        _inputController.CleanUpSubscriptions();

        if (sceneHandle.Result.Scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneHandle.Result.Scene);
        }

        Addressables.Release(CameraHandle);
        Addressables.Release(UICanvasHandle);
        Addressables.Release(FieldCanvasHandle);
        Addressables.Release(InputControllerGOHandle);
        Addressables.Release(VictoryWindowHandle);
        Addressables.Release(GameOverWindowHandle);
        Addressables.Release(handleFieldObjectsPrefabsSO);

        Addressables.Release(sceneHandle);
    }

    private async void PrepareNewScene()
    {
        FieldObjectsPrefabsSO fieldObjectsPrefabsSO;

        CameraHandle = Addressables.InstantiateAsync("CanvasCamera");
        UICanvasHandle = Addressables.InstantiateAsync("UICanvas");
        FieldCanvasHandle = Addressables.InstantiateAsync("FieldCanvas");
        InputControllerGOHandle = Addressables.InstantiateAsync("InputControllerGO");
        VictoryWindowHandle = Addressables.InstantiateAsync("VictoryWindow");
        GameOverWindowHandle = Addressables.InstantiateAsync("GameOverWindow");
        handleFieldObjectsPrefabsSO = Addressables.LoadAssetAsync<FieldObjectsPrefabsSO>("FieldObjectsPrefabsSO");

        await CameraHandle.Task;
        await UICanvasHandle.Task;
        await FieldCanvasHandle.Task;
        await InputControllerGOHandle.Task;
        await VictoryWindowHandle.Task;
        await GameOverWindowHandle.Task;
        await handleFieldObjectsPrefabsSO.Task;

        GameObject cameraObject = CameraHandle.Result;
        canvasCamera = cameraObject.GetComponent<Camera>();

        GameObject uiCanvasObject = UICanvasHandle.Result;
        uiCanvas = uiCanvasObject.GetComponent<Canvas>();
        _uiView = uiCanvasObject.GetComponent<UIView>();

        GameObject fieldCanvasObject = FieldCanvasHandle.Result;
        GameObject fieldObject = fieldCanvasObject.transform.GetChild(0).gameObject;
        GameObject fadingPanel = fieldObject.transform.GetChild(0).gameObject;
        fieldCanvas = fieldCanvasObject.GetComponent<Canvas>();

        GameObject InputControllerGO = InputControllerGOHandle.Result;
        _inputController = InputControllerGO.GetComponent<InputController>();

        uiCanvas.worldCamera = canvasCamera;
        fieldCanvas.worldCamera = canvasCamera;

        _levelData = new LevelData();
        _levelData.LoadFromTextAsset(_levelDataFile);
        _goalsManager = new GoalsManager(_levelData);
        _uiView.Initialize(_goalsManager);

        if (handleFieldObjectsPrefabsSO.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load ScriptableObject: " + handleFieldObjectsPrefabsSO.OperationException);
            return;
        }

        fieldObjectsPrefabsSO = handleFieldObjectsPrefabsSO.Result;

        FieldObjectFactory fieldObjectFactory = new FieldObjectFactory(fieldObjectsPrefabsSO, _goalsManager);
        _objectPooller = new ObjectPooller(fieldObjectFactory, fieldObject.transform);

        _field = new Field(canvasCamera, _objectPooller, _goalsManager, fieldObject, fadingPanel, _inputController, _levelData.Board, _gameStateMachine._updater);

        _VictoryWindow = VictoryWindowHandle.Result;
        _GameOverWindow = GameOverWindowHandle.Result;

        RestartLevelBTN restartVictory = _VictoryWindow.GetComponent<RestartLevelBTN>();
        restartVictory.Initialize(RestartLevel, _VictoryWindow);

        RestartLevelBTN restartGameOver = _GameOverWindow.GetComponent<RestartLevelBTN>();
        restartGameOver.Initialize(RestartLevel, _GameOverWindow);

        _VictoryWindow.SetActive(false);
        _GameOverWindow.SetActive(false);

        _goalsManager.OnVictoryAchived += Victory;
        _goalsManager.OnGameOver += GameOver;
    }

    private void RestartLevel(GameObject windowToDeactivate)
    {
        _goalsManager.Initialize(_levelData);
        _uiView.SetInitials();
        _field.ResetLevel(_levelData.Board);
        _inputController.UnLock();

        windowToDeactivate.SetActive(false);
    }

    private void Victory()
    {
        _inputController.Lock();
        _VictoryWindow.SetActive(true);
    }

    private void GameOver()
    {
        _inputController.Lock();
        _GameOverWindow.SetActive(true);
    }
}