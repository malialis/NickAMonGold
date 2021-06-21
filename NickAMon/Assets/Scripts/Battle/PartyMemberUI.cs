using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private Image pokeImage;
    [SerializeField] private Color highlightedColor;

    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.PokeName;
        levelText.text = "Lvl:" + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);
        pokeImage.sprite = pokemon.Base.FrontSprite;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }

}
