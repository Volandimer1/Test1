using System.Collections.Generic;
using UnityEngine;

public class BonusSideRocket : BonusBase
{
    public BonusSideRocket() : base()
    {

    }

    public BonusSideRocket(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
    }

    public override void TakeDamage()
    {
        _field.AddToEmptyCellsIndexes(Indexes);
        _goalsManager.AddScore(3);

        _field._fieldObjects[Indexes.Row, Indexes.Column] = null;

        int width = _field._fieldObjects.GetLength(1);

        for (int j = 0; j < width; j++)
        {
            if (_field._fieldObjects[Indexes.Row, j] != null)
                _field._fieldObjects[Indexes.Row, j].TakeDamage();
        }

        _objectPoller.ReturnObjectToPool(this);
    }
}