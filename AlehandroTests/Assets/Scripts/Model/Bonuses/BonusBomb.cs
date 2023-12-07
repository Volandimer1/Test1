using System.Collections.Generic;
using UnityEngine;

public class BonusBomb : BonusBase
{
    public BonusBomb() : base()
    {

    }

    public BonusBomb(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
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
            _goalsManager.AddScore(5);
        }
        fieldObjects[Indexes.Row, Indexes.Column] = null;

        int IrangeFrom = Indexes.Row - 2;
        int IrangeTill = Indexes.Row + 3;
        int JrangeFrom = Indexes.Column - 2;
        int JrangeTill = Indexes.Column + 3;

        for (int i = IrangeFrom; i < IrangeTill; i++)
        {
            for (int j = JrangeFrom; j < JrangeTill; j++)
            {
                if (Field.InBounds(i, j) == false) continue;

                if (fieldObjects[i, j] != null)
                    fieldObjects[i, j].TakeDamage(ref fieldObjects, ref emptyCellsIndexes, ref indexOfARowForSortInEmptyCells);
            }
        }

        _objectPoller.ReturnObjectToPool(this);
    }
}