using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonMove", menuName = "Pokemon/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int movePoints;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int MovePoints
    {
        get { return movePoints; }
    }

    public string MoveName
    {
        get { return moveName; }
    }

    public string Description
    {
        get { return description; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }

    /*
    public bool IsSpecial
    {
        get
        {
            if(type == PokemonType.Fire || type == PokemonType.Water || type == PokemonType.Grass || type == PokemonType.Ice || type == PokemonType.Electric || type == PokemonType.Dragon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    */


}
[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;

    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}


public enum MoveCategory
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe, Self
}