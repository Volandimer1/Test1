using UnityEngine;

public class OrangeToken : TokenBase
{
    public OrangeToken() : base()
    {

    }

    public OrangeToken(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
    }
}