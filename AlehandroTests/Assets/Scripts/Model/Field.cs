using System;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    private const int BoardRows = 9;
    private const int BoardColumns = 5;

    public FieldObject[,] _fieldObjects = new FieldObject[BoardRows, BoardColumns];
    public List<Indexes> _emptyCellsIndexes = new List<Indexes>();
    public List<int> _indexOfARowForSortInEmptyCells = new List<int>(BoardRows) { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private FieldGravityLogicService _fieldGravityLogicService;
    private LightningController _lightningController;
    private FieldObjectPooller _objectPooller;
    private GoalsManager _goalsManager;
    private GameObject _fieldGameObject;
    private GameObject _fadingPanel;
    private Camera _camera;

    private Vector2 _cursorLocalCoordinates = new Vector2();
    private List<Indexes> _chain = new List<Indexes>();
    private List<Indexes> _obstaclesToDamageIndexes = new List<Indexes>();

    public Field()
    {
        
    }

    public void Initialize(Camera camera, FieldObjectPooller objectPoller, GoalsManager goalsManager, GameObject fieldGameObject, GameObject fadingPanel, LightningController lightningController, InputController inputController, int[] boardData, Updater updater)
    {
        _fieldGravityLogicService = new FieldGravityLogicService(this, updater, objectPoller);

        _objectPooller = objectPoller;
        _goalsManager = goalsManager;
        _fieldGameObject = fieldGameObject;
        _fadingPanel = fadingPanel;
        _lightningController = lightningController;
        _camera = camera;

        inputController.OnDragEvent += HandleDragEvent;
        inputController.OnPointerDownEvent += HandlePointerDownEvent;
        inputController.OnPointerUpEvent += HandlePointerUpEvent;

        GenerateLevel(boardData);
    }

    public void ResetLevel(int[] boardData)
    {
        for (int i = 0; i < BoardRows; i++)
        {
            for (int j = 0; j < BoardColumns; j++)
            {
                if (_fieldObjects[i, j] != null)
                {
                    DeleteFromField(i, j);
                }
            }
        }

        _emptyCellsIndexes.Clear();
        _indexOfARowForSortInEmptyCells = new List<int>(BoardRows) { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        _fieldGravityLogicService.Reset();

        GenerateLevel(boardData);
    }

    public void DeleteFromField(int row, int column)
    {
        _objectPooller.ReturnObjectToPool(_fieldObjects[row, column]);
        _fieldObjects[row, column] = null;
    }

    public static bool InBounds(int i, int j)
    {
        if ((0 <= i) && (i < BoardRows) && (0 <= j) && (j < BoardColumns))
            return true;
        return false;
    }

    public static bool InBounds(Indexes indexes)
    {
        if ((0 <= indexes.Row) && (indexes.Row < BoardRows) && (0 <= indexes.Column) && (indexes.Column < BoardColumns))
            return true;
        return false;
    }

    public void AddIndexesToSortedList(List<Indexes> list, List<int> indexesOfARowsInList, Indexes itemToAdd)
    {
        if (list.Contains(itemToAdd) == false)
        {
            list.Insert(indexesOfARowsInList[itemToAdd.Row], itemToAdd);
            for (int i = itemToAdd.Row - 1; i > -1; i--)
            {
                indexesOfARowsInList[i]++;
            }
        }
    }

    public void AddToEmptyCellsIndexes(Indexes itemToAdd)
    {
        AddIndexesToSortedList(_emptyCellsIndexes, _indexOfARowForSortInEmptyCells, itemToAdd);
    }

    public void RemoveFromListOfSortedIndexes(List<Indexes> list, List<int> indexesOfARowsInList, Indexes itemToRemove)
    {
        list.Remove(itemToRemove);
        for (int i = itemToRemove.Row - 1; i > -1; i--)
        {
            indexesOfARowsInList[i]--;
        }
    }

    public void RemoveFromEmptyCellsIndexes(Indexes itemToRemove)
    {
        RemoveFromListOfSortedIndexes(_emptyCellsIndexes, _indexOfARowForSortInEmptyCells, itemToRemove);
    }

    private Indexes GetIndexesCursorIsOn(Vector3 position)
    {
        _cursorLocalCoordinates = _fieldGameObject.transform.InverseTransformPoint(_camera.ScreenToWorldPoint(position));
        return FieldObject.LocalPositionToIndexes(_cursorLocalCoordinates);
    }

    private void HandlePointerDownEvent(Vector3 position)
    {
        if (_fieldGravityLogicService.FieldIsMoving)
            return;

        Indexes indexesCursorOn = GetIndexesCursorIsOn(position);

        if (!InBounds(indexesCursorOn))
            return;

        if (_chain.Count == 0)
        {
            Type typeOfAnObjectCursorIsOn = _fieldObjects[indexesCursorOn.Row, indexesCursorOn.Column].GetType();

            if(typeof(ISelectable).IsAssignableFrom(typeOfAnObjectCursorIsOn))
            {
                SelectNewObject(indexesCursorOn);

                HighlightTokens(typeOfAnObjectCursorIsOn);
            }
        }
    }

    private void HandleDragEvent(Vector3 position)
    {
        if (_fieldGravityLogicService.FieldIsMoving)
            return;

        Indexes indexesCursorOn = GetIndexesCursorIsOn(position);

        if (!InBounds(indexesCursorOn) || (_chain.Count < 1))
            return;

        if (indexesCursorOn == _chain[_chain.Count - 1])
            return;

        if (IndexesIsNeighborToLastInChain(indexesCursorOn))
        {
            if ((_chain.Count > 1) && (indexesCursorOn == _chain[_chain.Count - 2]))
            {
                DeselectPreviousObject();
                _lightningController.RemovePoint();
            }
            else if (CanAddToChain(indexesCursorOn))
            {
                SelectNewObject(indexesCursorOn);
            }
        }
    }

    private bool CanAddToChain(Indexes indexesCursorOn)
    {
        Type typeOfAnObjectCursorIsOn = _fieldObjects[indexesCursorOn.Row, indexesCursorOn.Column].GetType();

        bool isFirstInChainType = typeOfAnObjectCursorIsOn.Equals(_fieldObjects[_chain[0].Row, _chain[0].Column].GetType());
        bool isBonusType = typeof(BonusBase).IsAssignableFrom(typeOfAnObjectCursorIsOn);
        bool isNotInChain = !_chain.Contains(indexesCursorOn);

        return (isFirstInChainType || isBonusType) && isNotInChain;
    }

    private void DeselectPreviousObject()
    {
        ISelectable objToDeselect = _fieldObjects[_chain[_chain.Count - 1].Row, _chain[_chain.Count - 1].Column] as ISelectable;
        objToDeselect.DeleteFromChain();
        _chain.RemoveAt(_chain.Count - 1);
    }

    private void SelectNewObject(Indexes indexesCursorOn)
    {
        _chain.Add(indexesCursorOn);
        ISelectable objToAddToChain = _fieldObjects[indexesCursorOn.Row, indexesCursorOn.Column] as ISelectable;
        objToAddToChain.AddToChain();

        _lightningController.AddPoint(_fieldObjects[indexesCursorOn.Row, indexesCursorOn.Column].PrefabInstance.transform.position);
    }

    private void HandlePointerUpEvent(Vector3 position)
    {
        _fadingPanel.SetActive(false);
        _fadingPanel.transform.SetAsLastSibling();
        _lightningController.RemoveAll();

        if (_chain.Count == 1)
        {
            if (TryHandleSingleChain())
                return;
        }

        if (_chain.Count < 3)
        {
            DeselectChain();
            return;
        }

        _goalsManager.SubtructAmountOfMoves(1);

        int matchCount = _chain.Count;
        Indexes lastInChainIndexes = _chain[_chain.Count - 1];

        DamageChain();
        DamageObstacles();

        MatchCountCheck(matchCount, lastInChainIndexes);

        _fieldGravityLogicService.StartGravity();
    }

    private bool TryHandleSingleChain()
    {
        Type objectType = _fieldObjects[_chain[0].Row, _chain[0].Column].GetType();

        if (typeof(BonusBase).IsAssignableFrom(objectType))
        {
            _goalsManager.SubtructAmountOfMoves(1);
            _fieldObjects[_chain[0].Row, _chain[0].Column].TakeDamage();
            _chain.Clear();
            _fieldGravityLogicService.StartGravity();
            return true;
        }

        // other cases for single chain

        return false;
    }

    private void MatchCountCheck(int matchCount, Indexes whereToSpawn)
    {

        if ((matchCount >= 5) && (matchCount <= 7))
        {
            _fieldObjects[whereToSpawn.Row, whereToSpawn.Column] =
                _objectPooller.GetObjectOfType(typeof(BonusSideRocket), whereToSpawn.Row, whereToSpawn.Column);

            RemoveFromEmptyCellsIndexes(whereToSpawn);
        }

        if (matchCount > 7)
        {
            _fieldObjects[whereToSpawn.Row, whereToSpawn.Column] =
                _objectPooller.GetObjectOfType(typeof(BonusBomb), whereToSpawn.Row, whereToSpawn.Column);

            RemoveFromEmptyCellsIndexes(whereToSpawn);
        }
    }

    private void DeselectChain()
    {
        foreach (Indexes indexes in _chain)
        {
            ISelectable selectablefromObject = _fieldObjects[indexes.Row, indexes.Column] as ISelectable;
            selectablefromObject.DeleteFromChain();
        }
        _chain.Clear();
    }

    private void DamageChain()
    {
        Indexes lastInChainIndexes;
        while (_chain.Count > 0)
        {
            lastInChainIndexes = _chain[_chain.Count - 1];
            AddNearbyObstaclesIndexesToListForDamaging(lastInChainIndexes);
            if (_fieldObjects[lastInChainIndexes.Row, lastInChainIndexes.Column] != null)
                _fieldObjects[lastInChainIndexes.Row, lastInChainIndexes.Column].TakeDamage();
            _chain.RemoveAt(_chain.Count - 1);
        }
    }

    private void DamageObstacles()
    {
        Indexes lastIndexes;
        while (_obstaclesToDamageIndexes.Count > 0)
        {
            lastIndexes = _obstaclesToDamageIndexes[_obstaclesToDamageIndexes.Count - 1];
            if (_fieldObjects[lastIndexes.Row, lastIndexes.Column] != null)
                _fieldObjects[lastIndexes.Row, lastIndexes.Column].TakeDamage();
            _obstaclesToDamageIndexes.RemoveAt(_obstaclesToDamageIndexes.Count - 1);
        }
    }

    private void GenerateLevel(int[] boardData)
    {
        for (int i = 0; i < BoardRows; i++)
        {
            for (int j = 0; j < BoardColumns; j++)
            {
                Type type = FieldObjectsPrefabsSO.GetTypeByID[boardData[(i*5) + j]];
                _fieldObjects[i, j] = _objectPooller.GetObjectOfType(type, i, j);
            }
        }
    }

    private void AddNearbyObstaclesIndexesToListForDamaging(Indexes indexes)
    {
        Indexes indexesSides;
        indexesSides.Row = indexes.Row - 1; indexesSides.Column = indexes.Column;
        AddIfObstacleToListForDamaging(indexesSides);

        indexesSides.Row = indexes.Row + 1;
        AddIfObstacleToListForDamaging(indexesSides);

        indexesSides.Row = indexes.Row; indexesSides.Column = indexes.Column - 1;
        AddIfObstacleToListForDamaging(indexesSides);

        indexesSides.Column = indexes.Column + 1;
        AddIfObstacleToListForDamaging(indexesSides);
    }

    private void AddIfObstacleToListForDamaging(Indexes indexes)
    {
        if (InBounds(indexes.Row, indexes.Column))
            if (_fieldObjects[indexes.Row, indexes.Column] != null)
            {
                Type objectType = _fieldObjects[indexes.Row, indexes.Column].GetType();

                if (typeof(ObstacleBase).IsAssignableFrom(objectType))
                {
                    if (_obstaclesToDamageIndexes.Contains(indexes) == false) _obstaclesToDamageIndexes.Add(indexes);
                }
            }
    }
    
    private bool IndexesIsNeighborToLastInChain(Indexes indexes)
    {
        if ((Math.Abs(indexes.Row - _chain[_chain.Count - 1].Row) <= 1) && 
            (Math.Abs(indexes.Column - _chain[_chain.Count - 1].Column) <= 1))
            return true;
        return false;        
    }

    private void HighlightTokens(Type type)
    {
        _fadingPanel.SetActive(true);
        _fadingPanel.transform.SetAsLastSibling();

        for (int i = 0; i < BoardRows; i++)
        {
            for (int j = 0; j < BoardColumns; j++)
            {
                if (_fieldObjects[i, j] == null) continue;

                Type ijObjectType = _fieldObjects[i, j].GetType();

                if ((ijObjectType.Equals(type)) || (typeof(BonusBase).IsAssignableFrom(ijObjectType)))
                {
                    _fieldObjects[i, j].PrefabInstance.transform.SetAsLastSibling();
                }
            }
        }
    }
}
