using System.Collections.Generic;
using UnityEngine;

public class BonusBomb : BonusBase
{
    public BonusBomb() : base()
    {

    }

    public BonusBomb(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
    }

    public override void TakeDamage()
    {
        _field.AddToEmptyCellsIndexes(Indexes);
        _goalsManager.AddScore(5);
        
        _field._fieldObjects[Indexes.Row, Indexes.Column] = null;

        int IrangeFrom = Indexes.Row - 2;
        int IrangeTill = Indexes.Row + 3;
        int JrangeFrom = Indexes.Column - 2;
        int JrangeTill = Indexes.Column + 3;

        for (int i = IrangeFrom; i < IrangeTill; i++)
        {
            for (int j = JrangeFrom; j < JrangeTill; j++)
            {
                if (Field.InBounds(i, j) == false) continue;

                if (_field._fieldObjects[i, j] != null)
                    _field._fieldObjects[i, j].TakeDamage();
            }
        }

        _objectPoller.ReturnObjectToPool(this);
    }
}