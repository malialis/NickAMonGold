using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon", menuName ="Pokemon/Create New Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string pokeName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type01;
    [SerializeField] PokemonType type02;

    //base states
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

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
    Legendary
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
