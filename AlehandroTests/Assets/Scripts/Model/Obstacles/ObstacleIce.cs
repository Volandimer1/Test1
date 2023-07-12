using UnityEngine;

public class ObstacleIce : ObstacleBase
{
    public ObstacleIce() : base()
    {

    }

    public ObstacleIce(GameObject gameObject, int indexI, int indexJ)
    {
        Constructor(gameObject, indexI, indexJ);
        Moovable = false;
    }
}