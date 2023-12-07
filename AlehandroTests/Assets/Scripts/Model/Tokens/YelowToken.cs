using UnityEngine;

public class YelowToken : TokenBase
{
    public YelowToken() : base()
    {

    }

    public YelowToken(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }
}