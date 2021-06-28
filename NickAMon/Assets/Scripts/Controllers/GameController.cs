using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NickAMon.Controller;

public enum GameState
{
    FreeRoam,
    Battle,
    Interacting,
    Dialog
}

public class GameController : MonoBehaviour
{
    private GameState state;

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
        buddy = FindObjectOfType<BuddyController>().GetComponent<BuddyController>();
    }

    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    private BuddyController buddy;

    private GameState stateBeforePause;

   // private TrainerController trainer;
    public static GameController Instance { get; private set; }
    public BuddyController Buddy { get => buddy; set => buddy = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }

        if (Input.GetKeyDown(KeyCode.H))
            playerController.GetComponent<PokemonParty>().HealParty();
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    private void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

}
