using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    PartyScreen,
    BattleOver,
    Busy
}


public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;    
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    private BattleState state;

    private int currentAction;
    private int currentMoveAction;
    private int currentPartyMember;

    public event Action<bool> OnBattleOver;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;



    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentAction++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentAction--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 2)
            {
                //bag
            }
            else if (currentAction == 3)
            {
                // run
            }
        }

    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMoveAction++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMoveAction--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMoveAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMoveAction -= 2;

        currentMoveAction = Mathf.Clamp(currentMoveAction, 0, playerUnit._Pokemon.Moves.Count - 1);

        dialogueBox.UpdateMovesSelection(currentMoveAction, playerUnit._Pokemon.Moves[currentMoveAction]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            dialogueBox.EnableActionSelector(false);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //cancel move selection go back to choice menu
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            dialogueBox.EnableActionSelector(true);
            ActionSelection();
        }


    }

    private void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentPartyMember++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentPartyMember--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentPartyMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentPartyMember -= 2;

        currentPartyMember = Mathf.Clamp(currentPartyMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentPartyMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentPartyMember];

            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("Current Pokemon is fainted, please select an other one");
                return;
            }
            if (selectedMember == playerUnit._Pokemon)
            {
                partyScreen.SetMessageText("Pokemon is already in battle");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    public IEnumerator SetupBattle()
    {
        //setup player pokemon
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        
        //setup enemy pokemon
        enemyUnit.Setup(wildPokemon);

        //setup partymember screen
        partyScreen.Init();

        dialogueBox.SetMoveNames(playerUnit._Pokemon.Moves);

        yield return dialogueBox.TypeDialogue($"A Wild {enemyUnit._Pokemon.Base.PokeName} appeared.");
        yield return new WaitForSeconds(1f);

        ChooseFirstTurn();
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;

        StartCoroutine(dialogueBox.TypeDialogue("Choose an Action"));

        dialogueBox.EnableActionSelector(true);
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    private IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit._Pokemon.Moves[currentMoveAction];

        yield return RunMove(playerUnit, enemyUnit, move);

        //if battle state was not changed by the run move, go to the next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
        
    }

    private IEnumerator EnemyMove()
    {
        yield return new WaitForSeconds(1f);

        state = BattleState.PerformMove;
        var move = enemyUnit._Pokemon.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        //if battle state was not changed by the run move, go to the next step
        if (state == BattleState.PerformMove)
            ActionSelection();
        
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit._Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit._Pokemon);
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit._Pokemon);
        move.MovePoints--;

        yield return dialogueBox.TypeDialogue($"{sourceUnit._Pokemon.Base.PokeName} used {move.Base.MoveName}");
        
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();

        if(move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit._Pokemon, targetUnit._Pokemon);
        }
        else
        {
            var damageDetails = targetUnit._Pokemon.TakeDamage(move, sourceUnit._Pokemon);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        

        if (targetUnit._Pokemon.HP <= 0)
        {
            targetUnit.PlayFaintAnimation();
            yield return dialogueBox.TypeDialogue($"{targetUnit._Pokemon.Base.PokeName} fainted");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }

        //status like burn hurt after the round.
        sourceUnit._Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit._Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit._Pokemon.HP <= 0)
        {
            sourceUnit.PlayFaintAnimation();
            yield return dialogueBox.TypeDialogue($"{sourceUnit._Pokemon.Base.PokeName} fainted");
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }

    }

    private IEnumerator RunMoveEffects(Move move, Pokemon sourcePokemon, Pokemon targetPokemon)
    {
        var effects = move.Base.Effects;

        if (effects.Boosts != null)
        {
            //stat boosting / deboosting
            if (move.Base.Target == MoveTarget.Self)
                sourcePokemon.ApplyBoosts(effects.Boosts);
            else
                targetPokemon.ApplyBoosts(effects.Boosts);
        }

        if(effects.Status != ConditionID.None)
        {
            //status effects like burning
            targetPokemon.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(sourcePokemon);
        yield return ShowStatusChanges(targetPokemon);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
                //choose your next pokemon
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
        }
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    private void ChooseFirstTurn()
    {
        if(playerUnit._Pokemon.Speed >= enemyUnit._Pokemon.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonFainted = true;

        if(playerUnit._Pokemon.HP > 0)
        {
            currentPokemonFainted = false;
            yield return dialogueBox.TypeDialogue($"Come back {playerUnit._Pokemon.Base.PokeName} ");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        //send in the next one
        //setup player pokemon
        playerUnit.Setup(newPokemon);
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.PokeName} You Can do it.");

        if (currentPokemonFainted)
        {
            ChooseFirstTurn();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
        
    }

    private void OpenPartyScreen()
    {
        print("Party screen");
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);

    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if(damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialogue("A Critical Hit!!");
        }

        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogueBox.TypeDialogue("A Super Effective Hit!!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogue("A Not Effective Hit!!");
        }

    }

    private IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while(pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
        }
    }

}
