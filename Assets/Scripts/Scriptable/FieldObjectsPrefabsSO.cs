using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FieldObjectsPrefabsSO : ScriptableObject
{
    public GameObject BlueTokenPrefab;
    public Sprite BlueTokenSprite;
    public GameObject GreenTokenPrefab;
    public Sprite GreenTokenSprite;
    public GameObject OrangeTokenPrefab;
    public Sprite OrangeTokenSprite;
    public GameObject RedTokenPrefab;
    public Sprite RedTokenSprite;
    public GameObject YelowTokenPrefab;
    public Sprite YelowTokenSprite;

    public GameObject IceObstaclePrefab;
    public Sprite IceObstacleSprite;
    public GameObject RockObstaclePrefab;
    public Sprite RockObstacleSprite;

    public GameObject BombBonusPrefab;
    public Sprite BombBonusSprite;
    public GameObject SideRocketBonusPrefab;
    public Sprite SideRocketBonusSprite;

    public static System.Type[] GetTypeByID = new System.Type[9] 
    {
        typeof(BlueToken),
        typeof(GreenToken),
        typeof(OrangeToken),
        typeof(RedToken),
        typeof(YelowToken),
        typeof(ObstacleIce),
        typeof(ObstacleRock),
        typeof(BonusBomb),
        typeof(BonusSideRocket)
    };

    public static Dictionary<System.Type, int> GetIDByType = new Dictionary<System.Type, int>()
    {
        {typeof(BlueToken), 0 },
        {typeof(GreenToken), 1 },
        {typeof(OrangeToken), 2 },
        {typeof(RedToken), 3 },
        {typeof(YelowToken), 4 },

        {typeof(ObstacleIce), 5 },
        {typeof(ObstacleRock), 6 },

        {typeof(BonusBomb), 7 },
        {typeof(BonusSideRocket), 8 }
    };

    public Dictionary<System.Type, GameObject> PrefabsDictionary = new Dictionary<System.Type, GameObject>() 
    {
        {typeof(BlueToken), null },
        {typeof(GreenToken), null },
        {typeof(OrangeToken), null },
        {typeof(RedToken), null },
        {typeof(YelowToken), null },

        {typeof(ObstacleIce), null },
        {typeof(ObstacleRock), null },

        {typeof(BonusBomb), null },
        {typeof(BonusSideRocket), null }
    };

    public Dictionary<System.Type, Sprite> SpritesDictionaryByType = new Dictionary<System.Type, Sprite>()
    {
        {typeof(BlueToken), null },
        {typeof(GreenToken), null },
        {typeof(OrangeToken), null },
        {typeof(RedToken), null },
        {typeof(YelowToken), null },

        {typeof(ObstacleIce), null },
        {typeof(ObstacleRock), null },

        {typeof(BonusBomb), null },
        {typeof(BonusSideRocket), null }
    };

    public Dictionary<int, Sprite> SpritesDictionaryByID = new Dictionary<int, Sprite>()
    {
        {0, null },
        {1, null },
        {2, null },
        {3, null },
        {4, null },

        {5, null },
        {6, null },

        {7, null },
        {8, null }
    };

    public void Initialize()
    {
        PrefabsDictionary[typeof(BlueToken)] = BlueTokenPrefab;
        PrefabsDictionary[typeof(GreenToken)] = GreenTokenPrefab;
        PrefabsDictionary[typeof(OrangeToken)] = OrangeTokenPrefab;
        PrefabsDictionary[typeof(RedToken)] = RedTokenPrefab;
        PrefabsDictionary[typeof(YelowToken)] = YelowTokenPrefab;

        PrefabsDictionary[typeof(ObstacleIce)] = IceObstaclePrefab;
        PrefabsDictionary[typeof(ObstacleRock)] = RockObstaclePrefab;

        PrefabsDictionary[typeof(BonusBomb)] = BombBonusPrefab;
        PrefabsDictionary[typeof(BonusSideRocket)] = SideRocketBonusPrefab;

        SpritesDictionaryByType[typeof(BlueToken)] = BlueTokenSprite;
        SpritesDictionaryByID[0] = BlueTokenSprite;
        SpritesDictionaryByType[typeof(GreenToken)] = GreenTokenSprite;
        SpritesDictionaryByID[1] = GreenTokenSprite;
        SpritesDictionaryByType[typeof(OrangeToken)] = OrangeTokenSprite;
        SpritesDictionaryByID[2] = OrangeTokenSprite;
        SpritesDictionaryByType[typeof(RedToken)] = RedTokenSprite;
        SpritesDictionaryByID[3] = RedTokenSprite;
        SpritesDictionaryByType[typeof(YelowToken)] = YelowTokenSprite;
        SpritesDictionaryByID[4] = YelowTokenSprite;

        SpritesDictionaryByType[typeof(ObstacleIce)] = IceObstacleSprite;
        SpritesDictionaryByID[5] = IceObstacleSprite;
        SpritesDictionaryByType[typeof(ObstacleRock)] = RockObstacleSprite;
        SpritesDictionaryByID[6] = RockObstacleSprite;

        SpritesDictionaryByType[typeof(BonusBomb)] = BombBonusSprite;
        SpritesDictionaryByID[7] = BombBonusSprite;
        SpritesDictionaryByType[typeof(BonusSideRocket)] = SideRocketBonusSprite;
        SpritesDictionaryByID[8] = SideRocketBonusSprite;
    }

    public static System.Type GetRandomTokenType()
    {
        int randomind = Random.Range(0, 5);
        return GetTypeByID[randomind];
    }
}