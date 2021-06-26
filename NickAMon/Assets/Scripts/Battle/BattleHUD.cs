using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statusText;
    [SerializeField] private HPBar hpBar;

    [SerializeField] private Color psnColor;
    [SerializeField] private Color brnColor;
    [SerializeField] private Color slpColor;
    [SerializeField] private Color parColor;
    [SerializeField] private Color frzColor;

    private Dictionary<ConditionID, Color> statusColors;


    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.PokeName;
        levelText.text = "Lvl:" + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.BRN, brnColor },
            {ConditionID.FRZ, frzColor },
            {ConditionID.PAR, parColor },
            {ConditionID.PSN, psnColor },
            {ConditionID.SLP, slpColor },
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText; 
    }

    public void SetStatusText()
    {
        if(_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);
            _pokemon.HpChanged = false;
        }
        
    }

}
