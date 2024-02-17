using UnityEngine;

public class ObstacleIce : ObstacleBase
{
    public ObstacleIce() : base()
    {

    }

    public ObstacleIce(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
        Movable = false;
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
        Movable = false;
    }
}