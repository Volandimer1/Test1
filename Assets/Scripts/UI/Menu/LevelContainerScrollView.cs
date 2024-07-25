using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelContainerScrollView : MonoBehaviour
{
    [SerializeField] private LevelsContainerSO _levelsContainerSO;
    [SerializeField] private GameObject _contentPrefab;
    [SerializeField] private GameObject _selectionBox;

    private GameStateMachine _gameStateMachine;
    private int _selectedLevel = 0;
    private List<GameObject> _levels = new List<GameObject>();

    public void Initialize(GameStateMachine gameStateMachine)
    {
        _gameStateMachine =  gameStateMachine;
    }

    void Start()
    {
        for(int i = 0; i < _levelsContainerSO.LevelFileNames.Count; i++)
        {
            GameObject content = Instantiate(_contentPrefab, transform);

            TextMeshProUGUI contetnTPM = content.GetComponent<TextMeshProUGUI>();
            contetnTPM.text = (i + 1).ToString();

            SelectLevel selector = content.GetComponent<SelectLevel>();
            selector.Initialize(this);
            
            _levels.Add(content);
        }

        _selectionBox.transform.SetParent(_levels[0].transform);
        RectTransform _selectionBoxRectTransform = _selectionBox.GetComponent<RectTransform>();
        _selectionBoxRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _selectionBoxRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _selectionBoxRectTransform.pivot = new Vector2(0.5f, 0.5f);
        _selectionBoxRectTransform.sizeDelta = new Vector2(100, 100);
        _selectionBox.transform.localPosition = Vector3.zero;

    }

    public void StartTheLevel()
    {
        LevelState levelState = _gameStateMachine.StateFactories[typeof(LevelState)].Invoke() as LevelState;

        
        string levelName = _levelsContainerSO.LevelFileNames[_selectedLevel];
        TextAsset levelData = Resources.Load<TextAsset>("Levels/" + levelName);

        if (levelState != null)
        {
            levelState.Initialize(levelData);
        }

        _ = _gameStateMachine.TransitionToState(levelState);
    }

    public void ItemClicked(GameObject item)
    {
        for (int i = 0; i < _levels.Count; i++)
        {
            if (item == _levels[i])
            {
                _selectedLevel = i;
                _selectionBox.transform.SetParent(_levels[i].transform);
                _selectionBox.transform.localPosition = Vector3.zero;
                return;
            }
        }
    }
}
