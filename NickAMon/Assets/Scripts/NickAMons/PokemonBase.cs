using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon", menuName ="Pokemon/Create New Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string pokeName;

    [TextArea]
    [SerializeField] private string description;

    [Header("Pokemon Sprites")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite iconSprite;

    [Header("Pokemon Type")]
    [SerializeField] PokemonType type01;
    [SerializeField] PokemonType type02;

    //base states
    [Header("Base Stats")]
    [Tooltip("This is the pokemons base stats")]
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [Header("Exp Stats and rates")]
    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;
    [SerializeField] int catchRate = 255;
    [SerializeField] int statusBonus;

    [Header("Moves")]
    [SerializeField] List<LearnableMove> learnableMoves;

    public static int MaxNumberOfMoves { get; set; } = 4;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level) / 4;
        }
        else if (growthRate == GrowthRate.Erractic)
        {
            return GetErractic(level);
        }
        return -1;
    }

    public int GetErractic(int level)
    {
        if (level <= 15)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
        }
        else if (level >= 15 && level <= 36)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50));
        }
        else
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
        }
    }


    #region properties

    public string PokeName
    {
        get { return pokeName;  }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public Sprite IconSprite
    {
        get { return iconSprite; }
    }

    public PokemonType Type01
    {
        get { return type01; }
    }

    public PokemonType Type02
    {
        get { return type02; }
    }

    public int MaxHP
    {
        get { return maxHP; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public int CatchRate
    {
        get { return catchRate; }
    }

    public int ExpYield => expYield;

    public GrowthRate GrowthRate => growthRate;

    #endregion


}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}


public enum PokemonType
{
    None,
    Normal,
    Bug,
    Dragon,
    Electric,
    Fire,
    Fighting,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,    
    Poison,
    Psychic,
    Rock,
    Water,
    Dark,
    Steel,
    Legendary,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //these are for boosts for MoveAccuracy
    Accuracy,
    Evasion
}

public enum GrowthRate
{
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Erractic

}

public class TypeChart
{
    static float[][] chart =
    {
        //                  Nor Bug Drg Ele Fir  Fgt fly ghs grs grd ice poi psy rok wtr drk stl leg
        /*nor*/ new float[]{1f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, 1f, .5f, 1f, 1f,.5f, 1f },
        /*bug*/ new float[]{1f, 1f, 1f, 1f, .5f, .5f, .5f, .5f, 2f, 1f, 1f, .5f, 2f, 1f, 1f, 2f, .5f, 1f },
        /*drg*/ new float[]{1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 1f },
        /*ele*/ new float[]{1f, 1f, 1f, .5f, 1f, 1f, 2f, 1f, .5f, 0f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f },
        /*fir*/ new float[]{1f, 2f, .5f, 1f, .5f, 1f, 1f, 1f, 2f, 1f, 2f, 1f, 1f, .5f, .5f, 1f, 2f, 1f },
        /*fgt*/ new float[]{2f, .5f, 1f, 1f, 1f, 1f, .5f, 0f, 1f, 1f, 2f, .5f, 1f, 2f, 1f, 2f, 2f, 1f},
        /*fly*/ new float[]{1f, 2f, 1f, .5f, 1f, 2f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, .5f, 1f, 1f, .5f, 1f },
        /*ghs*/ new float[]{0f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, .5f, 1f, 1f },
        /*grs*/ new float[]{1f, .5f, .5f, 1f, .5f, 1f, .5f, 1f, .5f, 2f, 1f, .5f, 1f, 2f, 2f, 1f, .5f, 1f },        
        /*grd*/ new float[]{1f, .5f, 1f, 2f, 2f, 1f, 0f, 1f, .5f, 1f, 1f, 2f, 1f, 2f, 1f, 1f, 2f, 1f },
        /*ice*/ new float[]{1f, 1f, 2f, 1f, .5f, 1f, 2f, 1f, 2f, 2f, .5f, 1f, 1f, 1f, .5f, 1f, .5f, 1f },
        /*poi*/ new float[]{1f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 2f, .5f, 1f, .5f, 1f, .5f, 1f, 1f, 0f, 1f },
        /*psy*/ new float[]{1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, .5f, 1f, 1f, 0f, .5f, 1f },
        /*rok*/ new float[]{1f, 2f, 1f, 1f, 2f, .5f, 2f, 1f, 1f, .5f, 2f, 1f, 1f, 1f, 1f, 1f, .5f, 1f },
        /*wtr*/ new float[]{1f, 1f, .5f, 1f, 2f, 1f, 1f, 1f, .5f, 2f, 1f, 1f, 1f, 2f, .5f, 1f, 1f, 1f },
        /*drk*/ new float[]{1f, 2f, 1f, 1f, 1f, .5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, .5f, 1f, 1f },
        /*stl*/ new float[]{1f, 1f, 1f, .5f, .5f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, .5f, 1f, .5f, 1f },
        /*leg*/ new float[]{1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }

    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }



}
