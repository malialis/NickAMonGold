using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private int encounterRate = 25;

    public event Action OnEncountered;
    public event Action OnEncounteredLedge;
    public event Action<Collider2D> OnEnterTrainersView;

    private Vector2 input;
    private Character character;

    [SerializeField] private Sprite sprite; //player image
    [SerializeField] private string playerName; //player name

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0; // removes diagonal movement

            if (input != Vector2.zero)
            {
                GameController.Instance.Buddy.Follow(GameController.Instance.PlayerController.transform.position);
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
            CheckForLedges();
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.Instance.TriggerableLayers);

        foreach(var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }

    }

    private void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void CheckForLedges()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.5f, GameLayers.Instance.LedgeLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);

        }
    }

    public string PlayerName
    {
        get => playerName;
    }

    public Sprite PlayerSprite
    {
        get => sprite;
    }
    public Character Character => character;

}


