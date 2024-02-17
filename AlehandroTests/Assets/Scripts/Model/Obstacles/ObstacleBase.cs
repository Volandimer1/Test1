using UnityEngine;

public class ObstacleBase : FieldObject 
{
    protected int _health;
    private GameObject _cracks;

    public ObstacleBase() : base()
    {

    }

    public ObstacleBase(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
        _cracks = gameObject.transform.GetChild(1).gameObject;
        _health = 2;
    }

    public override void Reset()
    {
        _health = 2;
        _cracks.SetActive(false);
    }

    public override void TakeDamage()
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

            _field.AddToEmptyCellsIndexes(Indexes);

            _field.DeleteFromField(Indexes.Row, Indexes.Column);
        }
    }
}
