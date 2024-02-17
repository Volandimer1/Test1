using UnityEngine;

public class YelowToken : TokenBase
{
    public YelowToken() : base()
    {

    }

    public YelowToken(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
    }
}