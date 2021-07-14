using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    PartyScreen,
    BattleOver,
    Busy,
    AboutToUse,
    MoveToForget
}

public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
}


public class BattleSystem : MonoBehaviour
{
    [Header("Player Unit")]
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private Image playerImage;

    [Header("Enemy Unit")]
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private Image trainerImage;

    [Header("Dialog and Party screen")]
    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private GameObject pokeBallSprite;

    private BattleState state;
    private BattleState? previousState;

    [Header("Actions")]
    private int currentAction;
    private int currentMoveAction;
    private int currentPartyMember;
    private bool aboutToUseChoice = true;
    [SerializeField] private MovesSelectionUI moveSelectionUI;

    public event Action<bool> OnBattleOver;

    [Header("Pokemons")]
    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;

    private bool isTrainerBattle = false;
    private int escapeAttempts;
    private MoveBase moveToLearn;

    [Header("Controllers")]
    private PlayerController player;
    private TrainerController trainer;


    #region Starting and Ending Battles

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        isTrainerBattle = false;

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
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
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextTrainerPokemon = trainerParty.GetHealthyPokemon();
                if (nextTrainerPokemon != null)
                {
                    //send out next pokemon
                    StartCoroutine(AboutToUse(nextTrainerPokemon));
                }
                else
                {
                    BattleOver(true);
                }
            }

        }
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    #endregion

    #region Handling Stuff

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
        else if(state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if( state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if(moveIndex  == PokemonBase.MaxNumberOfMoves)
                {
                    //dont learn new move
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} did not learn {moveToLearn.MoveName}!"));
                }
                else
                {
                    //forget the selected and learn new move
                    var selectedMove = playerUnit._Pokemon.Moves[moveIndex].Base;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} forgot {selectedMove.MoveName} and learned {moveToLearn.MoveName}!"));
                    playerUnit._Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                    
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
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
                previousState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 2)
            {
                //bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 3)
            {
                // run
                StartCoroutine(RunTurns(BattleAction.Run));
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
            var move = playerUnit._Pokemon.Moves[currentMoveAction];
            if (move.MovePoints == 0) return;

            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            dialogueBox.EnableActionSelector(false);
            StartCoroutine(RunTurns(BattleAction.Move));
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

            if (previousState == BattleState.ActionSelection)
            { 
                previousState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if(playerUnit._Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must have a pokemon selected to continue");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (previousState == BattleState.AboutToUse)
            {
                previousState = null;
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
            {
                ActionSelection();
            }
            
        }

    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogueBox.UpdateChoiceBoxSelection(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceSelector(false);
            if(aboutToUseChoice == true)
            {
                //yes choice
                previousState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                //or no
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableChoiceSelector(false);
            StartCoroutine(SendNextTrainerPokemon());
        }

    }

    
    #endregion

    public IEnumerator SetupBattle()
    {
        playerUnit.ClearHUD();
        enemyUnit.ClearHUD();

        if (!isTrainerBattle)
        {
            //wild pokemon not trainer
            //setup player pokemon
            playerUnit.Setup(playerParty.GetHealthyPokemon());

            //setup enemy pokemon
            enemyUnit.Setup(wildPokemon);
            
            dialogueBox.SetMoveNames(playerUnit._Pokemon.Moves);

            yield return dialogueBox.TypeDialogue($"A Wild {enemyUnit._Pokemon.Base.PokeName} appeared.");            
        }
        else
        {
            //it is a trainer battle
            //show trainer and player
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            playerImage.sprite = player.PlayerSprite;
            trainerImage.gameObject.SetActive(true);
            trainerImage.sprite = trainer.TrainerSprite;
            //taunght player
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} wants to Battle! Prepare to Die!!!");

            //send out first pokemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} sent out {enemyPokemon.Base.PokeName}! Now go Crush them!!!");

            //send out pokemon of player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogueBox.TypeDialogue($"{player.PlayerName} sent out {playerPokemon.Base.PokeName}! You can do it!!!");

            dialogueBox.SetMoveNames(playerUnit._Pokemon.Moves);

        }
        
        //setup partymember screen
        partyScreen.Init();
        escapeAttempts = 0;
        ActionSelection();
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
        
    private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy /= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    #region Running Turns and such

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        yield return new WaitForSeconds(1f);

        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerUnit._Pokemon.CurrentMove = playerUnit._Pokemon.Moves[currentMoveAction];
            enemyUnit._Pokemon.CurrentMove = enemyUnit._Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit._Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit._Pokemon.CurrentMove.Base.Priority;
            //check who goes first
            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit._Pokemon.Speed >= enemyUnit._Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit._Pokemon; // if the first fainted

            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit._Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if(secondPokemon.HP > 0)
            {
                //second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit._Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            
        }
        else
        {
            if(playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentPartyMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if(playerAction == BattleAction.UseItem)
            {
                dialogueBox.EnableActionSelector(false);
                yield return ThrowPokeBall();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }
            //after switching it is now enemy turn
            var enemyMove = enemyUnit._Pokemon.GetRandomMove();

            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        if(state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit._Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit._Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit._Pokemon);
        move.MovePoints--;

        yield return dialogueBox.TypeDialogue($"{sourceUnit._Pokemon.Base.PokeName} used {move.Base.MoveName}");
        yield return new WaitForSeconds(1f);

        if (CheckIfMoveHits(move, sourceUnit._Pokemon, targetUnit._Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit._Pokemon, targetUnit._Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit._Pokemon.TakeDamage(move, sourceUnit._Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit._Pokemon.HP > 0)
            {
                foreach(var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if(rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit._Pokemon, targetUnit._Pokemon, secondary.Target);
                }
            }

            if (targetUnit._Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue($"{sourceUnit._Pokemon.Base.PokeName}'s move missed!");
            yield return new WaitForSeconds(2f);

        }

    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        //status like burn hurt after the round.

        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        sourceUnit._Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit._Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit._Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }

    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon sourcePokemon, Pokemon targetPokemon, MoveTarget moveTarget)
    {  
        if (effects.Boosts != null)
        {
            //stat boosting / deboosting
            if (moveTarget == MoveTarget.Self)
                sourcePokemon.ApplyBoosts(effects.Boosts);
            else
                targetPokemon.ApplyBoosts(effects.Boosts);
        }

        if(effects.Status != ConditionID.None)
        {
            //status effects like burning
            targetPokemon.SetStatus(effects.Status);
        }

        if (effects.VolatileStatus != ConditionID.None)
        {
            //Volatilestatus effects like burning
            targetPokemon.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(sourcePokemon);
        yield return ShowStatusChanges(targetPokemon);
    }

    #endregion

    #region Switching and sending pokemong

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {        
        if(playerUnit._Pokemon.HP > 0)
        {
            //currentPokemonFainted = false;
            yield return dialogueBox.TypeDialogue($"Come back {playerUnit._Pokemon.Base.PokeName} ");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        //send in the next one
        //setup player pokemon
        playerUnit.Setup(newPokemon);
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.PokeName} You Can do it.");

        if(previousState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if(previousState == BattleState.AboutToUse)
        {
            previousState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
            

    }

    private IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;
        var nextTrainerPokemon = trainerParty.GetHealthyPokemon();

        enemyUnit.Setup(nextTrainerPokemon);
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} sent out {nextTrainerPokemon.Base.PokeName}!");

        state = BattleState.RunningTurn;

    }

    private void OpenPartyScreen()
    {
        print("Party screen");
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);

    }

    #endregion

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialogue("A Critical Hit!!");
            yield return new WaitForSeconds(2f);
        }

        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogueBox.TypeDialogue("A Super Effective Hit!!");
            yield return new WaitForSeconds(2f);
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogue("A Not Effective Hit!!");
            yield return new WaitForSeconds(2f);
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

    private IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName} is about to use {newPokemon.Base.PokeName}. Do you want to change Pokemon ? ");
        state = BattleState.AboutToUse;
        dialogueBox.EnableChoiceSelector(true);
    }

    private IEnumerator ThrowPokeBall()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue("You Can Not Steal a Trainers POKEMON!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogueBox.TypeDialogue($"{player.PlayerName} used a POKEBALL!");

        var pokeBallObj = Instantiate(pokeBallSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeBall = pokeBallObj.GetComponent<SpriteRenderer>();

        //animations
        yield return pokeBall.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1.2f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeBall.transform.DOLocalMoveY(enemyUnit.transform.position.y - 1.3f, 1f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit._Pokemon);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeBall.transform.DOPunchRotation(new Vector3(0, 0, 10f), 1f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            //pokemon is caughted
            yield return dialogueBox.TypeDialogue($"{enemyUnit._Pokemon.Base.PokeName} was Caught!");
            yield return pokeBall.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit._Pokemon);
            yield return dialogueBox.TypeDialogue($"{enemyUnit._Pokemon.Base.PokeName} was added to your Pokemon Party!");

            Destroy(pokeBall);
            BattleOver(true);
        }
        else
        {
            //pokemon broke free
            yield return new WaitForSeconds(1f);
            yield return pokeBall.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakCaptureAnimation();

            if (shakeCount < 2)
                yield return dialogueBox.TypeDialogue($"{enemyUnit._Pokemon.Base.PokeName} broke free, sweet sweet Freedom!");
            else
                yield return dialogueBox.TypeDialogue($"{enemyUnit._Pokemon.Base.PokeName} was almost caught. Better Luck Next Time");

            Destroy(pokeBall);
            state = BattleState.RunningTurn;
        }


    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHP);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }

        return shakeCount;

    }

    private IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"{player.PlayerName}, You can Not run from a Trainer!");
            state = BattleState.RunningTurn;
            yield break;
        }

        escapeAttempts++;

        int playerSpeed = playerUnit._Pokemon.Speed;
        int enemySpeed = enemyUnit._Pokemon.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogueBox.TypeDialogue($"{player.PlayerName} Ran away Safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogueBox.TypeDialogue($"{player.PlayerName} Ran away Safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"{player.PlayerName} Could not Escape!");
                state = BattleState.RunningTurn;
            }
        }


    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        faintedUnit.PlayFaintAnimation();
        yield return dialogueBox.TypeDialogue($"{faintedUnit._Pokemon.Base.PokeName} fainted");
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            //exp gain
            int expYield = faintedUnit._Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit._Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit._Pokemon.Exp += expGain;
            yield return dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} gained {expGain} exp!");
            yield return playerUnit.Hud.SetExpSmoothly();


            //check if leveld up

            while (playerUnit._Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                playerUnit._Pokemon.BoostStatsAfterLevelUp();
                yield return dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} we up a level, and is now {playerUnit._Pokemon.Level}!");
                yield return new WaitForSeconds(1f);

                //try to learn a new move
                var newMoveToLearn = playerUnit._Pokemon.GetLearnableMovesAtCurrentLevel();
                if(newMoveToLearn != null)
                {
                    if(playerUnit._Pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                    {
                        //add move
                        playerUnit._Pokemon.LearnMove(newMoveToLearn);
                        yield return dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} learned the move {newMoveToLearn.Base.MoveName}!");
                        dialogueBox.SetMoveNames(playerUnit._Pokemon.Moves);
                    }
                    else
                    {
                        //optoin to forget a move or skip this move
                        yield return dialogueBox.TypeDialogue($"{playerUnit._Pokemon.Base.PokeName} is trying to learn {newMoveToLearn.Base.MoveName}!");
                        yield return dialogueBox.TypeDialogue($"But you can only learn {PokemonBase.MaxNumberOfMoves} moves!");
                        yield return dialogueBox.TypeDialogue($"You will have to forget a move to learn a new one!");
                        yield return ChooseMoveToForget(playerUnit._Pokemon, newMoveToLearn.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(1f);

                    }
                }

                yield return playerUnit.Hud.SetExpSmoothly(true);
            }
            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"Choose a Move you want to forget!");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;

    }


}
