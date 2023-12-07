using System.Collections.Generic;
using UnityEngine;

public class FieldGravityLogicService : IUpdatable
{
    public bool FieldIsMoving { get; private set; }

    private Field _field;
    private Updater _updater;
    private ObjectPooller _objectPooller;

    private float movingSpeed = 1.5f;
    private float _currentAnimationProgress = 0f;

    private List<FieldObject> _movingObjects = new List<FieldObject>();

    public FieldGravityLogicService(Field field, Updater updater, ObjectPooller objectPooller)
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
        _currentAnimationProgress = 0f;
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
        _currentAnimationProgress += Time.deltaTime * movingSpeed;
        if (_currentAnimationProgress > 1f) _currentAnimationProgress = 1f;

        foreach (FieldObject movingObject in _movingObjects)
        {
            movingObject.MoveToInterpolated(_currentAnimationProgress);
        }

        if (_currentAnimationProgress >= 1f)
        {
            _currentAnimationProgress = 0f;
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

    private bool TryGetMovablesAboveTillCan(Indexes indexesOfEmptyCellToCheckFrom, ref List<Indexes> emptyUnChecked, ref List<int> indexOfARowsInEmptyUnChecked)
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
                _field.RemoveIndexesFromSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfEmptyCellToCheckFrom);
                return true;
            }

            if ((_field._fieldObjects[indexesAbove.Row, indexesAbove.Column] == null) || (_field._fieldObjects[indexesAbove.Row, indexesAbove.Column].Movable == false))
            {
                if (result == true)
                {
                    _field.AddIndexesToSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfCurentEmptyCell);
                    _field.RemoveIndexesFromSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfEmptyCellToCheckFrom);
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
        List<Indexes> emptyUnChecked = new List<Indexes>(_field._emptyCellsIndexes);
        List<int> indexOfARowsInEmptyUnChecked = new List<int>(_field._indexOfARowForSortInEmptyCells);
        Indexes indexesOfEmptyCell;

        for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
        {
            indexesOfEmptyCell = emptyUnChecked[itemCount];

            if (TryGetMovablesAboveTillCan(indexesOfEmptyCell, ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked)) itemCount--;
        }

        for (int itemCount = 0; itemCount < emptyUnChecked.Count; itemCount++)
        {
            indexesOfEmptyCell = emptyUnChecked[itemCount];

            Indexes indexesAboveRight = new Indexes(indexesOfEmptyCell.Row - 1, indexesOfEmptyCell.Column + 1);
            Indexes indexesAboveLeft = new Indexes(indexesOfEmptyCell.Row - 1, indexesOfEmptyCell.Column - 1);

            if (TryGetValidDiagonalMovable(indexesAboveRight, indexesOfEmptyCell))
            {
                _field.AddIndexesToSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesAboveRight);
                _field.RemoveIndexesFromSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfEmptyCell);
                itemCount--;
                TryGetMovablesAboveTillCan(indexesAboveRight, ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked);
                continue;
            }

            if (TryGetValidDiagonalMovable(indexesAboveLeft, indexesOfEmptyCell))
            {
                _field.AddIndexesToSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesAboveLeft);
                _field.RemoveIndexesFromSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfEmptyCell);
                itemCount--;
                TryGetMovablesAboveTillCan(indexesAboveLeft, ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked);
                continue;
            }

            _field.RemoveIndexesFromSortedList(ref emptyUnChecked, ref indexOfARowsInEmptyUnChecked, indexesOfEmptyCell);
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

    private void AddObjectToMovables(Indexes indexesOfObjectToAdd, Indexes targetIndexes)
    {
        _movingObjects.Add(_field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column]);
        _field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column].SetTarget(targetIndexes);

        _field._fieldObjects[indexesOfObjectToAdd.Row, indexesOfObjectToAdd.Column] = null;

        _field.AddIndexesToSortedList(ref _field._emptyCellsIndexes, ref _field._indexOfARowForSortInEmptyCells, indexesOfObjectToAdd);
    }

    private void AssignNewIndexesAndReferences()
    {
        foreach (FieldObject movable in _movingObjects)
        {
            Indexes newindexes = FieldObject.LocalPositionToIndexes(movable.PrefabInstance.transform.localPosition);
            movable.Indexes = newindexes;

            _field._fieldObjects[newindexes.Row, newindexes.Column] = movable;
            _field.RemoveIndexesFromSortedList(ref _field._emptyCellsIndexes, ref _field._indexOfARowForSortInEmptyCells, newindexes);
        }
    }
}