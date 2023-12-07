using UnityEngine;

public class ObstacleRock : ObstacleBase
{
    public ObstacleRock() : base()
    {

    }

    public ObstacleRock(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        Movable = true;
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        Movable = true;
    }
}