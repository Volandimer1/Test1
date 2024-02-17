using System.Collections.Generic;
using UnityEngine;

public class FieldGravityLogicService : IUpdatable
{
    public bool FieldIsMoving { get; private set; }

    private Field _field;
    private Updater _updater;
    private FieldObjectPooller _objectPooller;

    private float movingSpeed = 1.5f;
    private float _currentMovingProgress = 0f;

    private List<FieldObject> _movingObjects = new List<FieldObject>();
    private List<Indexes> emptyUnChecked;
    private List<int> indexOfARowsInEmptyUnChecked;

    public FieldGravityLogicService(Field field, Updater updater, FieldObjectPooller objectPooller)
    {
        _field = field;
        _updater = updater;
        _objectPooller = objectPooller;
    }

    public void StartGravity()
    {
        FieldIsMoving = true;
        GetNewMovables();
        _updater.AddUpdatable(this);
    }

    public void Reset()
    {
        _currentMovingProgress = 0f;
        FieldIsMoving = false;

        if (_movingObjects.Count > 0)
        {
            foreach(FieldObject movingObjects in _movingObjects)
            {
                _objectPooller.ReturnObjectToPool(movingObjects);
            }
        }

        _movingObjects.Clear();
    }

    public void Tick()
    {
        _currentMovingProgress += Time.deltaTime * movingSpeed;
        if (_currentMovingProgress > 1f) _currentMovingProgress = 1f;

        foreach (FieldObject movingObject in _movingObjects)
        {
            movingObject.MoveToInterpolated(_currentMovingProgress);
        }

        if (_currentMovingProgress >= 1f)
        {
            _currentMovingProgress = 0f;
            FieldIsMoving = false;

            AssignNewIndexesAndReferences();
            _movingObjects.Clear();

            if (_field._emptyCellsIndexes.Count > 0)
                GetNewMovables();

            if (_movingObjects.Count > 0)
                FieldIsMoving = true;
        }

        if (FieldIsMoving == false)
            _updater.RemoveUpdatable(this);
    }

    private void RemoveFromEmptyUnChecked(Indexes itemToRemove)
    {
        _field.RemoveFromListOfSortedIndexes(emptyUnChecked, indexOfARowsInEmptyUnChecked, itemToRemove);
    }

    private void AddToEmptyUnChecked(Indexes itemToAdd)
    {
        _field.AddIndexesToSortedList(emptyUnChecked, indexOfARowsInEmptyUnChecked, itemToAdd);
    }

    private bool TryGetMovablesAboveTillCan(Indexes indexesOfEmptyCellToCheckFrom)
    {
        Indexes indexesOfCurentEmptyCell = indexesOfEmptyCellToCheckFrom;
        bool result = false;

        while (true)
        {
            Indexes indexesAbove = new Indexes(indexesOfCurentEmptyCell.Row - 1, indexesOfCurentEmptyCell.Column);

            if (indexesOfCurentEmptyCell.Row == 0)
            {
                _movingObjects.Add(_objectPooller.GetRandomToken(indexesOfCurentEmptyCell.Row - 1, indexesOfCurentEmptyCell.Column));
                _movingObjects[_movingObjects.Count - 1].SetTarget(indexesOfCurentEmptyCell);
                RemoveFromEmptyUnChecked(indexesOfEmptyCellToCheckFrom);
                return true;
            }

            if ((_field._fieldObjects[indexesAbove.Row, indexesAbove.Column] == null) || (_field._fieldObjects[indexesAbove.Row, indexesAbove.Column].Movable == false))
            {
                if (result == true)
                {
                    AddToEmptyUnChecked(indexesOfCurentEmptyCell);
                    RemoveFromEmptyUnChecked(indexesOfEmptyCellToCheckFrom);
                }
                return result;
            }

            AddObjectToMovables(indexesAbove, indexesOfCurentEmptyCell);
            result = true;

            indexesOfCurentEmptyCell = indexesAbove;
        }
    }

    private void GetNewMovables()
    {
        emptyUnChecked = new List<Indexes>(_field._emptyCellsIndexes);
        indexOfARowsInEmptyUnChecked = new List<int>(_field._indexOfARowForSortInEmptyCells);
        Indexes indexesOfEmptyCell;

        for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
        {
            indexesOfEmptyCell = emptyUnChecked[itemCount];

            if (TryGetMovablesAboveTillCan(indexesOfEmptyCell)) itemCount--;
        }

        for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
        {
            indexesOfEmptyCell = emptyUnChecked[itemCount];

            Indexes indexesAboveRight = new Indexes(indexesOfEmptyCell.Row - 1, indexesOfEmptyCell.Column + 1);
            Indexes indexesAboveLeft = new Indexes(indexesOfEmptyCell.Row - 1, indexesOfEmptyCell.Column - 1);

            if (TryGetValidDiagonalMovable(indexesAboveRight, indexesOfEmptyCell))
            {
                AddToEmptyUnChecked(indexesAboveRight);
                RemoveFromEmptyUnChecked(indexesOfEmptyCell);
                itemCount--;
                TryGetMovablesAboveTillCan(indexesAboveRight);
                continue;
            }

            if (TryGetValidDiagonalMovable(indexesAboveLeft, indexesOfEmptyCell))
            {
                AddToEmptyUnChecked(indexesAboveLeft);
                RemoveFromEmptyUnChecked(indexesOfEmptyCell);
                itemCount--;
                TryGetMovablesAboveTillCan(indexesAboveLeft);
                continue;
            }

            RemoveFromEmptyUnChecked(indexesOfEmptyCell);
            itemCount--;
        }
    }

    private bool TryGetValidDiagonalMovable(Indexes IndexesToCheck, Indexes indexesToMoveTo)
    {
        bool result = false;
        if (Field.InBounds(IndexesToCheck))
        {
            if (_field._fieldObjects[IndexesToCheck.Row, IndexesToCheck.Column] != null)
            {
                if ((_field._fieldObjects[IndexesToCheck.Row, IndexesToCheck.Column].Movable) && (_field._fieldObjects[IndexesToCheck.Row + 1, IndexesToCheck.Column] != null))
                {
                    AddObjectToMovables(IndexesToCheck, indexesToMoveTo);
                    result = true;
                }
            }
        }
        return result;
    }

    private void AddObjectToMovables(Indexes indexesOfObjectToAdd, Indexes MoveTo)
    {
        _field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column].SetTarget(MoveTo);
        _movingObjects.Add(_field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column]);
        
        _field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column] = null;
        _field.AddToEmptyCellsIndexes(indexesOfObjectToAdd);
    }

    private void AssignNewIndexesAndReferences()
    {
        foreach (FieldObject movable in _movingObjects)
        {
            Indexes newindexes = FieldObject.LocalPositionToIndexes(movable.PrefabInstance.transform.localPosition);
            movable.Indexes = newindexes;

            _field._fieldObjects[newindexes.Row, newindexes.Column] = movable;
            _field.RemoveFromEmptyCellsIndexes(newindexes);
        }
    }
}