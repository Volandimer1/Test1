using System.Collections.Generic;
using UnityEngine;

public class BonusBase : FieldObject, ISelectable
{
    private GameObject _particalSystem;

    public BonusBase() : base()
    {

    }

    public BonusBase(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
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

    }

    public override void Reset()
    {
        _particalSystem.SetActive(false);
    }
}
