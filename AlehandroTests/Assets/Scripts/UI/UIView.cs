using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreTMP;
    [SerializeField] private Image _scoreIMG;
    [SerializeField] private TextMeshProUGUI _movesTMP;
    [SerializeField] private Image _tokenIMG;
    [SerializeField] private TextMeshProUGUI _tokenTMP;
    [SerializeField] private Image _obstacleIMG;
    [SerializeField] private TextMeshProUGUI _obstacleTMP;
    [SerializeField] private FieldObjectsPrefabsSO _fieldObjectsPrefabsSO;

    private GoalsManager _goalsManager;

    public void Initialize(GoalsManager goalsManager)
    {
        _goalsManager = goalsManager;

        _goalsManager.OnScoreValueChanged += UpdateScoreValue;
        _goalsManager.OnMovesLeftValueChanged += UpdateMovesLeftValue;
        _goalsManager.OnAmaountOfTokensToDestroyValueChanged += UpdateTokensToDestroyValue;
        _goalsManager.OnAmountOfObstaclesLeftValueChanged += UpdateObstaclesToDestroyValue;

        SetInitials();
    }

    private void UpdateScoreValue(int value)
    {
        _scoreTMP.text = "Score : " + value.ToString();
        _scoreIMG.fillAmount = value / _goalsManager._targetScore;
    }

    private void UpdateMovesLeftValue(int value)
    {
        _movesTMP.text = "Moves left : " + value.ToString();
    }

    private void UpdateTokensToDestroyValue(int value)
    {
        _tokenTMP.text = value.ToString();
    }

    private void UpdateObstaclesToDestroyValue(int value)
    {
        _obstacleTMP.text = value.ToString();
    }

    private void OnDestroy()
    {
        _goalsManager.OnScoreValueChanged -= UpdateScoreValue;
        _goalsManager.OnMovesLeftValueChanged -= UpdateMovesLeftValue;
        _goalsManager.OnAmaountOfTokensToDestroyValueChanged -= UpdateTokensToDestroyValue;
        _goalsManager.OnAmountOfObstaclesLeftValueChanged -= UpdateObstaclesToDestroyValue;
    }

    private void SetInitials()
    {
        _scoreIMG.transform.parent.gameObject.SetActive(false);
        _movesTMP.gameObject.SetActive(false);
        _tokenIMG.gameObject.SetActive(false);
        _tokenTMP.gameObject.SetActive(false);
        _obstacleIMG.gameObject.SetActive(false);
        _obstacleTMP.gameObject.SetActive(false);

        _scoreTMP.text = "Score : 0";

        if (_goalsManager._targetScore > 0)
        {
            _scoreIMG.transform.parent.gameObject.SetActive(true);
            _scoreIMG.fillAmount = 0;
        }

        if (_goalsManager._movesLeft > 0)
        {
            _movesTMP.gameObject.SetActive(true);
            _movesTMP.text = "Moves left : " + _goalsManager._movesLeft;
        }

        if ((_goalsManager._tokenToDestroy > -1)&&(_goalsManager._amountOfTokensToDestroy > 0))
        {
            _tokenIMG.gameObject.SetActive(true);
            _tokenIMG.sprite = _fieldObjectsPrefabsSO.SpritesDictionary[
                _fieldObjectsPrefabsSO.GetTypeByID[_goalsManager._tokenToDestroy]];
            _tokenTMP.gameObject.SetActive(true);
            _tokenTMP.text = _goalsManager._amountOfTokensToDestroy.ToString();
        }

        if ((_goalsManager._obstacleToDestroy > -1) && (_goalsManager._amountOfObstaclesToDestroy > 0))
        {
            _obstacleIMG.gameObject.SetActive(true);
            _obstacleIMG.sprite = _fieldObjectsPrefabsSO.SpritesDictionary[
                _fieldObjectsPrefabsSO.GetTypeByID[_goalsManager._obstacleToDestroy]];
            _obstacleTMP.gameObject.SetActive(true);
            _obstacleTMP.text = _goalsManager._amountOfObstaclesToDestroy.ToString();
        }
    }
}