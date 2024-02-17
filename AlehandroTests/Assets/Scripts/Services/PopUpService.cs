using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.UI;

public class PopUpService
{
    public void ShowSingleButtonPopUp(string messageText, string buttonText, Action onButtonPressed)
    {
        AsyncOperationHandle<GameObject> _popUpCanvasHandle;
        _popUpCanvasHandle = Addressables.InstantiateAsync("OneButtonPopUpCanvas");
        _popUpCanvasHandle.Completed += (go) =>
        {
            GameObject popup = _popUpCanvasHandle.Result;
            popup.GetComponentInChildren<TextMeshProUGUI>().text = messageText;

            Button button1 = popup.GetComponentInChildren<Button>();
            button1.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            button1.onClick.AddListener(() =>
            {
                onButtonPressed?.Invoke();

                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });
        };
    }

    public void ShowTwoButtonsPopUp(string messageText, string button1Text, string button2Text, Action onButton1Pressed, Action onButton2Pressed)
    {
        AsyncOperationHandle<GameObject> _popUpCanvasHandle;
        _popUpCanvasHandle = Addressables.InstantiateAsync("TwoButtonsPopUpCanvas");
        _popUpCanvasHandle.Completed += (go) =>
        {
            GameObject popup = _popUpCanvasHandle.Result;
            popup.GetComponentInChildren<TextMeshProUGUI>().text = messageText;

            Button[] buttons = popup.GetComponentsInChildren<Button>();

            buttons[0].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = button1Text;
            buttons[0].onClick.AddListener(() =>
            {
                onButton1Pressed?.Invoke();

                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });

            buttons[1].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = button2Text;
            buttons[1].onClick.AddListener(() =>
            {
                onButton2Pressed?.Invoke();

                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });
        };
    }

    public void ShowOptionsPopUp(GameStateMachine gameStateMachine, InputController inputController)
    {
        inputController.Lock();

        AsyncOperationHandle<GameObject> _popUpCanvasHandle;
        _popUpCanvasHandle = Addressables.InstantiateAsync("OptionsCanvas");
        _popUpCanvasHandle.Completed += (go) =>
        {
            GameObject popup = _popUpCanvasHandle.Result;

            Button[] buttons = popup.GetComponentsInChildren<Button>();
            Slider[] sliders = popup.GetComponentsInChildren<Slider>();

            if (PlayerPrefs.HasKey("SFXVolume")) 
                sliders[0].value = PlayerPrefs.GetFloat("SFXVolume");
            
            if (PlayerPrefs.HasKey("MusicVolume"))
                sliders[1].value = PlayerPrefs.GetFloat("MusicVolume");

            buttons[0].onClick.AddListener(() =>
            {
                inputController.UnLock();
                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });

            buttons[1].onClick.AddListener(() =>
            {
                inputController.UnLock();

                _ = gameStateMachine.TransitionToState<MenuState>();

                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });

            buttons[2].onClick.AddListener(() =>
            {
                Application.Quit();

                UnityEngine.Object.Destroy(popup);
                Addressables.Release(_popUpCanvasHandle);
            });

            AudioManager audioManager = gameStateMachine.AudioManager;

            sliders[0].onValueChanged.AddListener((float value) =>
            {
                audioManager.SetSFXVolume(value);
                PlayerPrefs.SetFloat("SFXVolume", value);
            });

            sliders[1].onValueChanged.AddListener((float value) =>
            {
                audioManager.SetMusicVolume(value);
                PlayerPrefs.SetFloat("MusicVolume", value);
            });
        };
    }
}
