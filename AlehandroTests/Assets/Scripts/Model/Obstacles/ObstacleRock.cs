using UnityEngine;

public class ObstacleRock : ObstacleBase
{
    public ObstacleRock() : base()
    {

    }

    public ObstacleRock(GameObject gameObject, int indexI, int indexJ)
    {
        Constructor(gameObject, indexI, indexJ);
        Moovable = true;
    }
}