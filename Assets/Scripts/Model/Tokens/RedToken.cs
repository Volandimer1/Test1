using UnityEngine;

public class RedToken : TokenBase
{
    public RedToken() : base()
    {

    }

    public RedToken(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
    }
}