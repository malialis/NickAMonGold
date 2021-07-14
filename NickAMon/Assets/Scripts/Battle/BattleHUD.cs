using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [SerializeField] private GameObject expBar;

    private Dictionary<ConditionID, Color> statusColors;


    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.PokeName;
        SetLevel();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHP);
        SetExp();

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

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);

    }

    public IEnumerator SetExpSmoothly(bool reset = false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();

    }

    private float GetNormalizedExp()
    {
        int currentLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float normalizedExp = (float)(_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);

    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);
            _pokemon.HpChanged = false;
        }
        
    }

    public void SetLevel()
    {
        levelText.text = "Lvl: " + _pokemon.Level;
    }

}
