using UnityEngine;

public class GreenToken : TokenBase
{
    public GreenToken() : base()
    {

    }

    public GreenToken(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
    }
}