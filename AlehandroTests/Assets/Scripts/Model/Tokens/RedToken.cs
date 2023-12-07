using UnityEngine;

public class RedToken : TokenBase
{
    public RedToken() : base()
    {

    }

    public RedToken(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }
}