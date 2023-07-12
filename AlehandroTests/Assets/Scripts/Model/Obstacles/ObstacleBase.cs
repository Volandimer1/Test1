using UnityEngine;

public class ObstacleBase : FieldObject, IDamagable 
{
    private int _health;
    private GameObject _cracks;

    public ObstacleBase() : base()
    {
        _health = 0;
        _cracks = new();
    }

    public ObstacleBase(GameObject gameObject, int indexI, int indexJ)
    {
        Constructor(gameObject, indexI, indexJ);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ)
    {
        base.Constructor(gameObject, indexI, indexJ);
        _cracks = gameObject.transform.GetChild(1).gameObject;
        _health = 2;
    }

    public override void Reset()
    {
        _health = 2;
        _cracks.SetActive(false);
    }

    public void TakeDamage()
    {
        _health--;
        if(_health == 1)
        {
            _cracks.SetActive(true);
        }
        else
        {

        }
    }
}
