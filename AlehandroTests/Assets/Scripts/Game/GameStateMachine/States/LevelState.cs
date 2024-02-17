using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Threading.Tasks;
using UnityEngine.UI;

public class LevelState : IGameState
{
    private const float _numberOfAssetsToLoad = 8f;

    private GameStateMachine _gameStateMachine;
    private LoadingScreen _loadingScreen;
    private Scene _newScene;
    private PopUpService _popUpService;

    private AsyncOperationHandle<GameObject> _cameraHandle;
    private AsyncOperationHandle<GameObject> _uICanvasHandle;
    private AsyncOperationHandle<GameObject> _fieldCanvasHandle;
    private AsyncOperationHandle<GameObject> _lightningRendererHandle;
    private AsyncOperationHandle<GameObject> _inputControllerGOHandle;
    private AsyncOperationHandle<FieldObjectsPrefabsSO> _fieldObjectsPrefabsSOHandle;

    private AsyncOperationHandle<SceneInstance> sceneHandle;

    private Camera _canvasCamera;
    private Canvas _uiCanvas;
    private Canvas _fieldCanvas;
    private FieldObjectPooller _objectPooller;
    private TextAsset _levelDataFile;
    private InputController _inputController;
    private LightningController _lightningController;
    private GoalsManager _goalsManager;
    private LevelData _levelData;
    private UIView _uiView;
    private Field _field;

    public LevelState(GameStateMachine gameStateMachine)
    {
        _popUpService = new PopUpService();
        _gameStateMachine = gameStateMachine;
    }

    public void Initialize(TextAsset levelDataFile)
    {
        _levelDataFile = levelDataFile;
    }

    public async Task EnterState()
    {
        _loadingScreen = new LoadingScreen();
        await _loadingScreen.InstantiateLoader();

        sceneHandle = Addressables.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
        await sceneHandle.Task;

        if (sceneHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load the LevelScene.");
            return;
        }

        _newScene = sceneHandle.Result.Scene;
        SceneManager.SetActiveScene(_newScene);

        await PrepareNewScene();
    }

    public Task ExitState()
    {
        _goalsManager.OnVictoryAchived -= VictoryPopUp;
        _goalsManager.OnGameOver -= GameOverPopUp;

        _inputController.CleanUpSubscriptions();
        
        SceneManager.UnloadSceneAsync(_newScene);

        ReleaseHandles();

        Addressables.Release(sceneHandle);

        return Task.CompletedTask;
    }

    private async Task PrepareNewScene()
    {
        LoadAssets();

        await Task.WhenAll(_cameraHandle.Task, _uICanvasHandle.Task, _fieldCanvasHandle.Task, _lightningRendererHandle.Task, 
            _inputControllerGOHandle.Task, _fieldObjectsPrefabsSOHandle.Task);

        CheckForErrors();

        InitializationProcess();

        _loadingScreen.ResetProgress();
        _ = _loadingScreen.ReleaseLoaderResources();
    }

    private void LoadAssets()
    {
        _ = LoadAssetViaAdressables("CanvasCamera", ref _cameraHandle);
        _ = LoadAssetViaAdressables("UICanvas", ref _uICanvasHandle);
        _ = LoadAssetViaAdressables("FieldCanvas", ref _fieldCanvasHandle);
        _ = LoadAssetViaAdressables("LightningRenderer", ref _lightningRendererHandle);
        _ = LoadAssetViaAdressables("InputControllerGO", ref _inputControllerGOHandle);

        _fieldObjectsPrefabsSOHandle = Addressables.LoadAssetAsync<FieldObjectsPrefabsSO>("FieldObjectsPrefabsSO");
        _fieldObjectsPrefabsSOHandle.Completed += (go) =>
        {
            _loadingScreen.AddProgress(1f / _numberOfAssetsToLoad);
        };
    }

    private Task LoadAssetViaAdressables(string assetName, ref AsyncOperationHandle<GameObject> handle)
    {
        handle = Addressables.InstantiateAsync(assetName);
        handle.Completed += (go) =>
        {
            _loadingScreen.AddProgress(1f / _numberOfAssetsToLoad);
        };

        return Task.CompletedTask;

        /*_cameraHandle.GetDownloadStatus().Percent;
        _cameraHandle.PercentComplete; */
    }

    private void CheckForErrors()
    {
        if (_cameraHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load CanvasCamera: " + _cameraHandle.OperationException);
            return;
        }

        if (_uICanvasHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load UICanvas: " + _uICanvasHandle.OperationException);
            return;
        }

        if (_fieldCanvasHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load FieldCanvas: " + _fieldCanvasHandle.OperationException);
            return;
        }

        if (_lightningRendererHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load LightningRenderer: " + _lightningRendererHandle.OperationException);
            return;
        }

        if (_inputControllerGOHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load InputControllerGO: " + _inputControllerGOHandle.OperationException);
            return;
        }

        if (_fieldObjectsPrefabsSOHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load ScriptableObject: " + _fieldObjectsPrefabsSOHandle.OperationException);
            return;
        }
    }

    private void InitializationProcess()
    {
        FieldObjectsPrefabsSO fieldObjectsPrefabsSO;
        fieldObjectsPrefabsSO = _fieldObjectsPrefabsSOHandle.Result;
        fieldObjectsPrefabsSO.Initialize();

        GameObject cameraObject = _cameraHandle.Result;
        _canvasCamera = cameraObject.GetComponent<Camera>();

        GameObject uiCanvasObject = _uICanvasHandle.Result;
        _uiCanvas = uiCanvasObject.GetComponent<Canvas>();
        Button optionButton = _uiCanvas.GetComponentInChildren<Button>();
        optionButton.onClick.AddListener(() => 
        {
            _popUpService.ShowOptionsPopUp(_gameStateMachine, _inputController);
        });
        _uiView = uiCanvasObject.GetComponent<UIView>();

        GameObject fieldCanvasObject = _fieldCanvasHandle.Result;
        GameObject fieldObject = fieldCanvasObject.transform.GetChild(0).gameObject;
        GameObject fadingPanel = fieldObject.transform.GetChild(0).gameObject;
        _fieldCanvas = fieldCanvasObject.GetComponent<Canvas>();

        GameObject InputControllerGO = _inputControllerGOHandle.Result;
        _inputController = InputControllerGO.GetComponent<InputController>();

        _uiCanvas.worldCamera = _canvasCamera;
        _fieldCanvas.worldCamera = _canvasCamera;

        _levelData = new LevelData();
        _levelData.LoadFromTextAsset(_levelDataFile);
        _goalsManager = new GoalsManager(_levelData);
        _uiView.Initialize(_goalsManager);

        GameObject lightningObject = _lightningRendererHandle.Result;
        LineRenderer lineRenderer = lightningObject.GetComponent<LineRenderer>();
        _lightningController = new LightningController(lineRenderer);

        _field = new Field();
        FieldObjectFactory fieldObjectFactory = new FieldObjectFactory(fieldObjectsPrefabsSO, _goalsManager, _field);
        _objectPooller = new FieldObjectPooller(fieldObjectFactory, fieldObject.transform);
        _field.Initialize(_canvasCamera, _objectPooller, _goalsManager, fieldObject, fadingPanel, _lightningController, _inputController, _levelData.Board, _gameStateMachine.Updater);

        _goalsManager.OnVictoryAchived += VictoryPopUp;
        _goalsManager.OnGameOver += GameOverPopUp;
    }

    private void ReleaseHandles()
    {
        Addressables.Release(_cameraHandle);
        Addressables.Release(_uICanvasHandle);
        Addressables.Release(_fieldCanvasHandle);
        Addressables.Release(_inputControllerGOHandle);
        Addressables.Release(_fieldObjectsPrefabsSOHandle);
    }

    private void VictoryPopUp()
    {
        _inputController.Lock();
        _popUpService.ShowSingleButtonPopUp("GG u won ^_^ u da best!!!", "Restart", RestartLevel);
    }

    private void GameOverPopUp()
    {
        _inputController.Lock();
        _popUpService.ShowSingleButtonPopUp("U Lost :-(", "Restart", RestartLevel);
    }

    private void RestartLevel()
    {
        _goalsManager.Initialize(_levelData);
        _uiView.SetInitials();
        _field.ResetLevel(_levelData.Board);
        _inputController.UnLock();
    }
}