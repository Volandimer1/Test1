using UnityEngine;

public class BlueToken : TokenBase
{
    public BlueToken() : base()
    {

    }

    public BlueToken(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
    }
}