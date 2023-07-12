using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Field : IUpdatable, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private ObjectPooller _objectPoller;
    private GoalsManager _goalsManager;
    private GameObject _fieldGameObject;
    private GameObject _fadingPanel;
    private FieldObject[,] _fieldObjects = new FieldObject[9,5];
    private float movingSpeed = 100f;
    private Camera _camera;

    private Vector2 cursorLocalCoordinates = new Vector2();
    private List<FieldObject> _chain = new List<FieldObject>();
    private List<indexes> _emptyCellsIndexes = new List<indexes>();
    private List<FieldObject> _movingObjects = new List<FieldObject>();
    private bool _fieldIsMoving = false;
    private float _interpolatingValue = 0f;
    
    public Field(ObjectPooller objectPoller, GoalsManager goalsManager, GameObject fieldGameObject, GameObject fadingPanel)
    {
        _objectPoller = objectPoller;
        _goalsManager = goalsManager;
        _fieldGameObject = fieldGameObject;
        _fadingPanel = fadingPanel;
    }

    public void Tick()
    {
        if (_fieldIsMoving)
        {
            _interpolatingValue += 0.01f*Time.deltaTime*movingSpeed;
            if (_interpolatingValue > 1f) _interpolatingValue = 1f;

            foreach(FieldObject movingObject in _movingObjects)
            {
                movingObject.MoveToInterpolated(_interpolatingValue);
            }

            if (_interpolatingValue >= 1f){
                _interpolatingValue = 0f;
                _fieldIsMoving = false;

                AssignNewIndexesAndReferences();
                _movingObjects.Clear();

                if (_emptyCellsIndexes.Count > 0) GetNewMovables();
                if (_movingObjects.Count > 0) _fieldIsMoving = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_fieldIsMoving)
            return;

        cursorLocalCoordinates = _fieldGameObject.transform.InverseTransformPoint(_camera.ScreenToWorldPoint(eventData.position));
        indexes indexesCursorOn = FieldObject.LocalPositionToIndexes(cursorLocalCoordinates);

        if (!InBounds(indexesCursorOn.i, indexesCursorOn.j))
            return;

        if (_chain.Count == 0)
        {
            if (_fieldObjects[indexesCursorOn.i, indexesCursorOn.j] is ISelectable myObj)
            {
                _chain.Add(_fieldObjects[indexesCursorOn.i, indexesCursorOn.j]);
                myObj.AddToChain();

                HighlightTokens(_chain[0].GetType());
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_fieldIsMoving)
            return;

        cursorLocalCoordinates = _fieldGameObject.transform.InverseTransformPoint(_camera.ScreenToWorldPoint(eventData.position));
        indexes indexesCursorOn = FieldObject.LocalPositionToIndexes(cursorLocalCoordinates);

        if (!InBounds(indexesCursorOn.i, indexesCursorOn.j))
            return;

        if (indexesCursorOn == _chain[_chain.Count - 1].Indexes)
            return;

        if (IfIndexesIsNeighborToLastInChain(indexesCursorOn))
        {
            Type typeOfAnObjectCursorIsOn = _fieldObjects[indexesCursorOn.i, indexesCursorOn.j].GetType();

            if ((_chain.Count > 1) && (indexesCursorOn == _chain[_chain.Count - 2].Indexes))
            {
                ISelectable objToDeselect = _chain[_chain.Count - 1] as ISelectable;
                objToDeselect.DeleteFromChain();
                _chain.RemoveAt(_chain.Count - 1);
            }
            else if ((typeOfAnObjectCursorIsOn.Equals(_chain[0].GetType()) || (typeof(BonusBase).IsAssignableFrom(typeOfAnObjectCursorIsOn))) && (!_chain.Contains(_fieldObjects[indexesCursorOn.i,indexesCursorOn.j])))
            {
                _chain.Add(_fieldObjects[indexesCursorOn.i, indexesCursorOn.j]);
                ISelectable objToAddToChain = _fieldObjects[indexesCursorOn.i, indexesCursorOn.j] as ISelectable;
                objToAddToChain.AddToChain();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _fadingPanel.SetActive(false);
        _fadingPanel.transform.SetAsLastSibling();

        if (_chain.Count < 3)
        {
            foreach (FieldObject fieldObject in _chain)
            {
                ISelectable selectablefromObject = fieldObject as ISelectable;
                selectablefromObject.DeleteFromChain();
            }
            _chain.Clear();
            return;
        }

        while (_chain.Count > 0)
        {
            int matchCount = 0;
            indexes lastInChainIndexes = _chain[_chain.Count - 1].Indexes;

            _emptyCellsIndexes.Add(lastInChainIndexes);
            Destroy(_chain[_chain.Count - 1].gameObject);
            _chain.RemoveAt(_chain.Count - 1);
        }

        _fieldIsMoving = true;
    }

    private bool IfIndexesIsNeighborToLastInChain(indexes indexes)
    {
        if ((Math.Abs(indexes.i - _chain[_chain.Count - 1].Indexes.i) <= 1) && 
            (Math.Abs(indexes.j - _chain[_chain.Count - 1].Indexes.j) <= 1))
            return true;
        return false;        
    }

    private void HighlightTokens(Type type)
    {
        _fadingPanel.SetActive(true);
        _fadingPanel.transform.SetAsLastSibling();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Type ijobjectType = _fieldObjects[i, j].GetType();

                if ((ijobjectType.Equals(type)) || (typeof(BonusBase).IsAssignableFrom(ijobjectType)))
                {
                    _fieldObjects[i, j].PrefabInstance.transform.SetAsLastSibling();
                }
            }
        }
    }

    private void GetNewMovables()
    {
        List<indexes> emptyUnChecked = new List<indexes>(_emptyCellsIndexes);
        indexes indexesOfEmptyCell;
        while (emptyUnChecked.Count > 0)
        {
            for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
            {
                indexesOfEmptyCell = emptyUnChecked[itemCount];

                indexes indexesAbove = new indexes(indexesOfEmptyCell.i, indexesOfEmptyCell.j - 1);

                if (indexesOfEmptyCell.j == 0)
                {
                    _movingObjects.Add(_objectPoller.GetRandomToken(indexesOfEmptyCell.i, indexesOfEmptyCell.j - 1, _fieldGameObject.transform));
                    _movingObjects[_movingObjects.Count - 1].SetTarget(indexesOfEmptyCell);
                    emptyUnChecked.Remove(indexesOfEmptyCell);
                    itemCount--;
                    break;
                }

                if (_fieldObjects[indexesAbove.j, indexesAbove.i] != null)
                {
                    if (_fieldObjects[indexesAbove.j, indexesAbove.i].Moovable)
                    {
                        AddObjectToMovables(indexesAbove, indexesOfEmptyCell);
                        emptyUnChecked.Add(indexesAbove);
                        emptyUnChecked.Remove(indexesOfEmptyCell);
                        itemCount--;
                        break;
                    }
                }
            }

            for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
            {
                indexesOfEmptyCell = emptyUnChecked[itemCount];

                indexes indexesAboveRight = new indexes(indexesOfEmptyCell.i + 1, indexesOfEmptyCell.j - 1);
                indexes indexesAboveLeft = new indexes(indexesOfEmptyCell.i - 1, indexesOfEmptyCell.j - 1);

                if (TryGetValidMovable(indexesAboveRight, indexesOfEmptyCell)) 
                {
                    emptyUnChecked.Add(indexesAboveRight);
                    emptyUnChecked.Remove(indexesOfEmptyCell);
                    break; 
                }

                if (TryGetValidMovable(indexesAboveLeft, indexesOfEmptyCell))
                {
                    emptyUnChecked.Add(indexesAboveLeft);
                    emptyUnChecked.Remove(indexesOfEmptyCell);
                    break;
                }

                emptyUnChecked.RemoveAt(itemCount);
                itemCount--;
            }
        }
    }

    private bool TryGetValidMovable(indexes IndexesToCheck, indexes indexesToMoveTo)
    {
        bool result = false;
        if (InBounds(IndexesToCheck))
        {
            if (_fieldObjects[IndexesToCheck.j, IndexesToCheck.i] != null)
            {
                if ((_fieldObjects[IndexesToCheck.j, IndexesToCheck.i].Moovable) && (_fieldObjects[IndexesToCheck.j + 1, IndexesToCheck.i] != null))
                {
                    AddObjectToMovables(IndexesToCheck, indexesToMoveTo);
                    result = true;
                }
            }
        }
        return result;
    }

    private void AddObjectToMovables(indexes indexesOfObjectToAdd, indexes targetIndexes)
    {
        _movingObjects.Add(_fieldObjects[indexesOfObjectToAdd.j, indexesOfObjectToAdd.i]);
        _fieldObjects[indexesOfObjectToAdd.j, indexesOfObjectToAdd.i].SetTarget(targetIndexes);

        _fieldObjects[indexesOfObjectToAdd.j, indexesOfObjectToAdd.i] = null;
        _emptyCellsIndexes.Add(indexesOfObjectToAdd);
    }

    private void AssignNewIndexesAndReferences()
    {
        foreach (FieldObject movable in _movingObjects)
        {
            indexes newindexes = FieldObject.LocalPositionToIndexes(movable.PrefabInstance.transform.localPosition);
            movable.Indexes = newindexes;

            _fieldObjects[newindexes.j, newindexes.i] = movable;
            _emptyCellsIndexes.Remove(newindexes);
        }
    }

    private bool InBounds(int i, int j)
    {
        if ((0 <= i) && (i < 9) && (0 <= j) && (j < 5))
            return true;
        return false;
    }

    private bool InBounds(indexes indexes)
    {
        if ((0 <= indexes.i) && (indexes.i < 9) && (0 <= indexes.j) && (indexes.j < 5))
            return true;
        return false;
    }
}
