using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen
{
    private float _progress = 0f; // from 0 to 1;

    private Image _progressBarImage;
    private GameObject _canvas;
    private GameObject _camera;

    private AsyncOperationHandle<SceneInstance> sceneHandle;

    public async Task InstantiateLoader()
    {
        sceneHandle = Addressables.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        await sceneHandle.Task;

        if (sceneHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load the LoadingScene.");
            return;
        }

        Scene newScene = sceneHandle.Result.Scene;
        SceneManager.SetActiveScene(newScene);

        GameObject progress = GameObject.Find("ProgressBarForeGround");
        _progressBarImage = progress.GetComponent<Image>();

        _canvas = GameObject.Find("Canvas");
        _camera = GameObject.Find("LoaderCamera");
    }

    public void Show()
    {
        _canvas.SetActive(true);
        _camera.SetActive(true);
    }

    public void Hide()
    {
        _canvas.SetActive(false);
        _camera.SetActive(false);
    }

    public void ResetProgress()
    {
        _progress = 0;
        _progressBarImage.fillAmount = 0;
    }

    public void UpdateProgress(float progress)
    {
        _progress = Mathf.Clamp(progress, 0f, 1f);
        _progressBarImage.fillAmount = _progress;
    }

    public void AddProgress(float value)
    {
        _progress = Mathf.Clamp(_progress + value, 0f, 1f);
        _progressBarImage.fillAmount = _progress;
    }

    public Task ReleaseLoaderResources()
    {
        if (sceneHandle.Result.Scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneHandle.Result.Scene);
        }

        Addressables.Release(sceneHandle);

        return Task.CompletedTask;
    }
}
