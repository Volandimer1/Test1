using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class InitializationState : IGameState
{
    private GameStateMachine _gameStateMachine;
    private Scene _currentScene;

    public InitializationState(GameStateMachine gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public async Task EnterState()
    {
        _currentScene = SceneManager.GetActiveScene();
        await Addressables.InitializeAsync().Task;

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            _gameStateMachine.AudioManager.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }
        else
        {
            _gameStateMachine.AudioManager.SetSFXVolume(0.5f);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _gameStateMachine.AudioManager.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }
        else
        {
            _gameStateMachine.AudioManager.SetMusicVolume(0.5f);
        }

        _ = _gameStateMachine.TransitionToState<MenuState>();
    }

    public Task ExitState()
    {
        SceneManager.UnloadSceneAsync(_currentScene);

        return Task.CompletedTask;
    }
}