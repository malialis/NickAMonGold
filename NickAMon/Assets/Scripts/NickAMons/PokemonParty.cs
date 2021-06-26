using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;
    public List<Pokemon> Pokemons { get { return pokemons; } }


    private void Start()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void HealParty()
    {
        foreach (Pokemon mon in pokemons)
        {
            mon.CureStatus();
            mon.CureVolatileStatus();
            mon.RestoreFullHP();
            //mon.ResetStatBoost();

            foreach (Move mov in mon.Moves)
            {
                mov.MovePoints = mov.Base.MovePoints;
            }
        }
    }


}
