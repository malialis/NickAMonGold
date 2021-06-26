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
    [SerializeField] bool alwaysHits;
    [SerializeField] int movePoints;
    [SerializeField] int priority;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;
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

    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }

    public int MovePoints
    {
        get { return movePoints; }
    }

    public int Priority { get { return priority; } }

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

    public List<SecondaryEffects> Secondaries { get { return secondaries; } }

    public MoveTarget Target
    {
        get { return target; }
    }

 
}
[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;
    [SerializeField] private ConditionID volatileStatus;

    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }
}

[System.Serializable]
public class SecondaryEffects: MoveEffects
{
    [SerializeField] private int chance;
    [SerializeField] private MoveTarget target;

    public int Chance { get { return chance; } }
    public MoveTarget Target { get { return target; } }

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
