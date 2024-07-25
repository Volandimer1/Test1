using UnityEngine;
using UnityEngine.UI;

public class RestartLevelBTN : MonoBehaviour
{
    [SerializeField] private Button RestartLevelButton;
    private GameObject _windowToDeactivate;
    public delegate void DelegateTypeToCall(GameObject windowToDeactivate); 
    public DelegateTypeToCall _methodToCall;

    public void Initialize(DelegateTypeToCall RestartMethod, GameObject windowToDeactivate)
    {
        _methodToCall = RestartMethod;
        _windowToDeactivate = windowToDeactivate;
        RestartLevelButton.onClick.AddListener(delegate () { _methodToCall(_windowToDeactivate); });
    }
}
