using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : FieldObject 
{
    protected int _health;
    private GameObject _cracks;

    public ObstacleBase() : base()
    {

    }

    public ObstacleBase(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        _cracks = gameObject.transform.GetChild(1).gameObject;
        _health = 2;
    }

    public override void Reset()
    {
        _health = 2;
        _cracks.SetActive(false);
    }

    public override void TakeDamage(ref FieldObject[,] fieldObjects, ref List<Indexes> emptyCellsIndexes,  ref List<int> indexOfARowForSortInEmptyCells)
    {
        _health--;
        if(_health == 1)
        {
            _cracks.SetActive(true);
        }
        else
        {
            if (FieldObjectsPrefabsSO.GetIDByType[this.GetType()] == _goalsManager._obstacleToDestroy)
            {
                _goalsManager.SubtructAmountOfObstacles(1);
                _goalsManager.AddScore(2);
            }

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
    }
}
