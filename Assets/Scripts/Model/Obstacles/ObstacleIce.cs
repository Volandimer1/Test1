using UnityEngine;

public class ObstacleIce : ObstacleBase
{
    public ObstacleIce() : base()
    {

    }

    public ObstacleIce(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
        Movable = false;
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
        Movable = false;
    }
}