using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class MyCustomEditor : EditorWindow
{
    [SerializeField] private int _SelectedIndex = -1;
    [SerializeField] private VisualElement _RightPane;
    [SerializeField] private FieldObjectsPrefabsSO _fieldObjectsSO;
    [SerializeField] private List<System.Type> _typesList;
    [SerializeField] private Dictionary<VisualElement, Image> _containerImageDict = new Dictionary<VisualElement, Image>();
    [SerializeField] private List<VisualElement> _gridOfContainers = new List<VisualElement>();
    [SerializeField] private LevelData _levelData = new LevelData();
    [SerializeField] private TextField _levelNametextField;
    [SerializeField] private PopupField<int> popupTokens;
    [SerializeField] private IntegerField amountToDestroyField;
    [SerializeField] private PopupField<int> popupObstacles;
    [SerializeField] private IntegerField scoreToGetField;
    [SerializeField] private IntegerField movesLeftField;

    [MenuItem("Tools/My Custom Editor")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("Level Editor");

        wnd.minSize = new Vector2(350, 500);
        wnd.maxSize = new Vector2(700, 700);
    }

    public void CreateGUI()
    {
        string[] assetGuids = AssetDatabase.FindAssets("t:FieldObjectsPrefabsSO");

        if (assetGuids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            _fieldObjectsSO = AssetDatabase.LoadAssetAtPath<FieldObjectsPrefabsSO>(assetPath);
        }

        _typesList = new List<System.Type>(FieldObjectsPrefabsSO.GetTypeByID);

        VisualElement root = rootVisualElement;

        _levelNametextField = new TextField("Enter Level Name here - ");
        _levelNametextField.style.minWidth = 300;

        root.Add(_levelNametextField);

        VisualElement horizontalLeveCOntainer = new VisualElement();
        horizontalLeveCOntainer.style.flexDirection = FlexDirection.Row;

        popupTokens = new PopupField<int>("Destroy Tokens", new List<int> { 0, 1, 2, 3, 4, -1 }, 0,
            (int arg) => {
                if (arg == -1) return "Null";
                return FieldObjectsPrefabsSO.GetTypeByID[arg].Name; },
            (int arg) => {
                if (arg == -1) return "Null";
                return FieldObjectsPrefabsSO.GetTypeByID[arg].Name;});
        popupTokens.RegisterValueChangedCallback(evnt => {
            _levelData.TokenToDestroy = evnt.newValue;
        });

        horizontalLeveCOntainer.Add(popupTokens);

        amountToDestroyField = new IntegerField("Amount of");
        amountToDestroyField.value = 10;
        amountToDestroyField.RegisterValueChangedCallback(evnt => {
            if (evnt.newValue<0)
            {
                amountToDestroyField.value = 0;
                return;
            }

            _levelData.AmountOfTokensToDestroy = evnt.newValue;
        });

        horizontalLeveCOntainer.Add(amountToDestroyField);

        root.Add(horizontalLeveCOntainer);

        popupObstacles = new PopupField<int>("Destroy Obstacles", new List<int> { 5, 6, -1}, 0,
            (int arg) => {
                if (arg == -1) return "Null";
                return FieldObjectsPrefabsSO.GetTypeByID[arg].Name;
            },
            (int arg) => {
                if (arg == -1) return "Null";
                return FieldObjectsPrefabsSO.GetTypeByID[arg].Name;
            });
        popupObstacles.RegisterValueChangedCallback(evnt => {
            _levelData.ObstacleToDestroy = evnt.newValue;
        });

        root.Add(popupObstacles);

        scoreToGetField = new IntegerField("Score to get");
        scoreToGetField.value = 20;
        scoreToGetField.RegisterValueChangedCallback(evnt => {
            if (evnt.newValue < 0)
            {
                scoreToGetField.value = 0;
                return;
            }

            _levelData.ScoreToGet = evnt.newValue;
        });

        root.Add(scoreToGetField);

        movesLeftField = new IntegerField("Moves untill Game Over");
        movesLeftField.value = 10;
        movesLeftField.RegisterValueChangedCallback(evnt => {
            if (evnt.newValue < 0)
            {
                movesLeftField.value = 0;
                return;
            }

            _levelData.MovesLeft = evnt.newValue;
        });

        root.Add(movesLeftField);

        VisualElement horizontalButtonsContainer = new VisualElement();
        horizontalButtonsContainer.style.flexDirection = FlexDirection.Row;

        Button loadButton = new Button(() => LoadLevelWithName(_levelNametextField.text));
        loadButton.text = "LoadLevel";
        horizontalButtonsContainer.Add(loadButton);

        Button saveButton = new Button(() => SaveLevelWithName(_levelNametextField.text));
        saveButton.text = "SaveLevel";
        horizontalButtonsContainer.Add(saveButton);

        Button generateRandomButton = new Button(() => GenerateRandom());
        generateRandomButton.text = "Generate Random Level";
        horizontalButtonsContainer.Add(generateRandomButton);

        root.Add(horizontalButtonsContainer);

        TwoPaneSplitView splitView = new TwoPaneSplitView(0, 80, TwoPaneSplitViewOrientation.Horizontal);

        root.Add(splitView);

        VisualElement gridContainer = new VisualElement();
        gridContainer.style.flexDirection = FlexDirection.Row;
        gridContainer.style.flexWrap = Wrap.Wrap;
        gridContainer.style.width = 250;
        gridContainer.style.height = 450;

        for (int i = 0; i < 45; i++)
        {
            IMGUIContainer container = new IMGUIContainer();
            container.style.width = 50;
            container.style.height = 50;
            int thisIndex = new int();
            thisIndex = i;
            container.RegisterCallback<MouseUpEvent>((evt) => OnContainerClick(container, thisIndex));
            _gridOfContainers.Add(container);
            AddImageToContainer(container);
            _levelData.Board[i] = 0;

            gridContainer.Add(container);
        }

        ListView leftPane = new ListView();
        splitView.Add(leftPane);
        _RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(_RightPane);
        _RightPane.Add(gridContainer);

        leftPane.makeItem = () => new Image();
        leftPane.bindItem = (item, index) => { (item as Image).sprite = _fieldObjectsSO.SpritesDictionaryByType[_typesList[index]]; };
        leftPane.itemsSource = _typesList;

        leftPane.selectedIndex = _SelectedIndex;

        leftPane.onSelectionChange += (items) => { _SelectedIndex = leftPane.selectedIndex; };
    }

    private void OnContainerClick(VisualElement container, int conteinerIndex)
    {
        if (_containerImageDict.ContainsKey(container))
        {
            _levelData.Board[conteinerIndex] = _SelectedIndex;

            _containerImageDict[container].sprite = 
                _fieldObjectsSO.SpritesDictionaryByType[_typesList[_SelectedIndex]];
            return;
        }
    }

    private void LoadLevelWithName(string levelName)
    {
        _levelData.LoadFromFile(levelName);

        for (int i = 0; i < 45; i++)
        {
            _containerImageDict[_gridOfContainers[i]].sprite =
                _fieldObjectsSO.SpritesDictionaryByType[_typesList[_levelData.Board[i]]];
        }

        popupTokens.value = _levelData.TokenToDestroy;
        amountToDestroyField.value = _levelData.AmountOfTokensToDestroy;
        popupObstacles.value = _levelData.ObstacleToDestroy;
        scoreToGetField.value = _levelData.ScoreToGet;
        movesLeftField.value = _levelData.MovesLeft;
    }

    private void SaveLevelWithName(string levelName)
    {
        _levelData.WriteToFile(levelName);
    }

    private void GenerateRandom()
    {
        for (int i = 0; i < 45; i++)
        {
            int randomindex = UnityEngine.Random.Range(0, _typesList.Count);

            _levelData.Board[i] = randomindex;

            _containerImageDict[_gridOfContainers[i]].sprite =
                _fieldObjectsSO.SpritesDictionaryByType[_typesList[randomindex]];
        }
    }

    private void AddImageToContainer(VisualElement container)
    {
        Image image = new Image();
        image.sprite = _fieldObjectsSO.SpritesDictionaryByType[_typesList[0]];
        image.style.width = container.contentRect.width;
        image.style.height = container.contentRect.height;
        image.style.flexGrow = 1;

        container.Add(image);
        _containerImageDict.Add(container, image);
    }
}