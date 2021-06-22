using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    //[SerializeField] private PokemonBase _base;
    ///[SerializeField] private int level;
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHUD hud;
    [SerializeField] private float enterBattleTime;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color hitColor;

    public Pokemon _Pokemon { get; set; }
    public bool IsPlayerUnit { get { return isPlayerUnit; } }
    public BattleHUD Hud { get { return hud; } }

    private Image image;
    private Vector3 originalPosition;


    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
    }

    private void Start()
    {
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }


    public void Setup(Pokemon pokemon)
    {
        _Pokemon = pokemon;
        if (isPlayerUnit)
            image.sprite = _Pokemon.Base.BackSprite;
        else
            image.sprite = _Pokemon.Base.FrontSprite;
        hud.SetData(pokemon);
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPosition.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPosition.y);
        }

        image.transform.DOLocalMoveX(originalPosition.x, enterBattleTime);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 150f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 150f, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
    }
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 250f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }


}
