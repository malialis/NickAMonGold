using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] private Text dialogueText;
    [SerializeField] private int lettersPerSecond;

    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;

    [SerializeField] private List<Text> actionText;
    [SerializeField] private List<Text> moveText;
    [SerializeField] private Text mpText;
    [SerializeField] private Text moveTypeText;

    [SerializeField] private Color highlightColor;


    public void SetDialogue(string dialog)
    {
        dialogueText.text = dialog;
    }

    public IEnumerator TypeDialogue(string dialog)
    {
        dialogueText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;

    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);

    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);

    }

    public void UpdateActionSelection(int selectionAction)
    {
        for (int i = 0; i < actionText.Count; i++)
        {
            if (i == selectionAction)
            {
                actionText[i].color = highlightColor;
            }
            else
            {
                actionText[i].color = Color.black;
            }
        }
    }

    public void UpdateMovesSelection(int selectedAction, Move move)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if( i == selectedAction)
            {
                moveText[i].color = highlightColor;
            }
            else
            {
                moveText[i].color = Color.black;
            }
        }
        mpText.text = $"MP {move.MovePoints} / {move.Base.MovePoints}";
        moveTypeText.text = move.Base.Type.ToString();
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if(i < moves.Count)
            {
                moveText[i].text = moves[i].Base.MoveName;                
            }
            else
            {
                moveText[i].text = "-";
            }
                
        }
    }


}
