using UnityEngine;

public class GreenToken : TokenBase
{
    public GreenToken() : base()
    {

    }

    public GreenToken(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }
}