using System.Collections.Generic;
using UnityEngine;

public class BonusSideRocket : BonusBase
{
    public BonusSideRocket() : base()
    {

    }

    public BonusSideRocket(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }

    public override void TakeDamage(ref FieldObject[,] fieldObjects, ref List<Indexes> emptyCellsIndexes,  ref List<int> indexOfARowForSortInEmptyCells)
    {
        if (emptyCellsIndexes.Contains(Indexes) == false)
        {
            emptyCellsIndexes.Insert(indexOfARowForSortInEmptyCells[Indexes.Row], Indexes);
            for (int i = Indexes.Row - 1; i > -1; i--)
            {
                indexOfARowForSortInEmptyCells[i]++;
            }
            _goalsManager.AddScore(3);
        }
        fieldObjects[Indexes.Row, Indexes.Column] = null;

        int width = fieldObjects.GetLength(1);

        for (int j = 0; j < width; j++)
        {
            if (fieldObjects[Indexes.Row, j] != null) 
                fieldObjects[Indexes.Row, j].TakeDamage(ref fieldObjects, ref emptyCellsIndexes, ref indexOfARowForSortInEmptyCells);
        }

        _objectPoller.ReturnObjectToPool(this);
    }
}