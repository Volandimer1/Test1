using UnityEngine;

public class ObstacleRock : ObstacleBase
{
    public ObstacleRock() : base()
    {

    }

    public ObstacleRock(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
        Movable = true;
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
        Movable = true;
    }
}