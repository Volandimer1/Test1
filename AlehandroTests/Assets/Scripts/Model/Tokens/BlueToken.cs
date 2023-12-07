using UnityEngine;

public class BlueToken : TokenBase
{
    public BlueToken() : base()
    {

    }

    public BlueToken(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }
}