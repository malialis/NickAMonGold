using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovesSelectionUI : MonoBehaviour
{

    [SerializeField] List<Text> moveTexts;
    [SerializeField] private Color highlightColor;
    private int currentMoveSelection = 0;


    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMoveSelection++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMoveSelection--;
        }

        currentMoveSelection = Mathf.Clamp(currentMoveSelection, 0, PokemonBase.MaxNumberOfMoves);
        UpdateMoveSelection(currentMoveSelection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentMoveSelection);
        }

    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < PokemonBase.MaxNumberOfMoves +1; i++)
        {
            if(i == selection)
            {
                moveTexts[i].color = highlightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }

}
