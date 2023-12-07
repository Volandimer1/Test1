using UnityEngine;

public class ObstacleIce : ObstacleBase
{
    public ObstacleIce() : base()
    {

    }

    public ObstacleIce(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        Movable = false;
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
        Movable = false;
    }
}