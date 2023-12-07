using System.Collections.Generic;
using UnityEngine;

public class TokenBase : FieldObject, ISelectable
{
    private GameObject _particalSystem;

    public TokenBase() : base()
    {

    }

    public TokenBase(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        _particalSystem = gameObject.transform.GetChild(1).gameObject;
        Movable = true;
    }

    public void AddToChain()
    {
        _particalSystem.SetActive(true);
    }

    public void DeleteFromChain()
    {
        _particalSystem.SetActive(false);
    }

    public override void TakeDamage(ref FieldObject[,] fieldObjects, ref List<Indexes> emptyCellsIndexes, ref List<int> indexOfARowForSortInEmptyCells)
    {
        _goalsManager.AddScore(1);

        if (FieldObjectsPrefabsSO.GetIDByType[this.GetType()] == _goalsManager._tokenToDestroy)
            _goalsManager.SubtructAmountOfTokens(1);

        if (emptyCellsIndexes.Contains(Indexes) == false)
        {
            emptyCellsIndexes.Insert(indexOfARowForSortInEmptyCells[Indexes.Row], Indexes);
            for (int i = Indexes.Row - 1; i > -1; i--)
            {
                indexOfARowForSortInEmptyCells[i]++;
            }
        }
        _objectPoller.ReturnObjectToPool(this);
        fieldObjects[Indexes.Row, Indexes.Column] = null;
    }

    public override void Reset()
    {
        _particalSystem.SetActive(false);
    }
}