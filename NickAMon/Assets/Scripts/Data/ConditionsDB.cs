using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.PSN,
            new Condition()
            {
                Name = "Poision",
                StartMessage = "has been Poisoned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} hurt itself due to Poison!");
                }
            }
        },

        {
            ConditionID.BRN,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been Burned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} hurt itself due to Burning!");
                }
            }
        },

        {
            ConditionID.PAR,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been Paralyzed!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} can not move due to being Paralyzed!");
                        return false;
                    }
                    return true;

                }
            }
        },

        {
            ConditionID.FRZ,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been Frozen!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} is not Frozen anymore!");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} is Frozen and can not move!");
                    return false;

                }
            }
        },

        {
            ConditionID.SLP,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen Asleep!",
                OnStart = (Pokemon pokemon) =>
                {
                    //sleep for 1 - 4 turns
                    pokemon.StatusTime = Random.Range(1, 5);
                    Debug.Log($"Will sleep for {pokemon.StatusTime} moves");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} is startled Awake, and can move!");
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.PokeName} is Asleep and can not move!");
                    return false;

                }
            }
        }

    };



}

public enum ConditionID
{
    None, //no condition
    PSN, //Poison
    BRN, //Burn
    SLP, //Sleep
    PAR, //Paralized
    FRZ //Frozen
}