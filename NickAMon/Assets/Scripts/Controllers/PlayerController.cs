using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NickAMon.Controller
{
    public class PlayerController: MonoBehaviour
    {        
        [SerializeField] private int encounterRate = 25;

        public event Action OnEncountered;
        public event Action OnEncounteredLedge;

        private Vector2 input;        
        private Character character;
        public Vector3 playerChange;

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

                if(input != Vector2.zero)
                {
                    GameController.Instance.Buddy.Follow(GameController.Instance.PlayerController.transform.position);
                    StartCoroutine(character.Move(input, CheckForEncounters));                  
                }
            }

            character.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Interact();
            }
        }

        private void Interact()
        {
            var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
            var interactPos = transform.position + facingDir;
            Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.InteractableLayer);
            if(collider != null)
            {
                collider.GetComponent<Interactable>()?.Interact(transform);
            }
        }

        private void CheckForEncounters()
        {
            if (Physics2D.OverlapCircle(transform.position, 0.12f, GameLayers.Instance.GrassLayer) != null)
            {
                if (UnityEngine.Random.Range(1, 101) <= encounterRate)
                {
                    Debug.Log("Encountered a pokemon");
                    character.Animator.IsMoving = false;
                    OnEncountered();
                }
            }
        }

        private void CheckForLedges()
        {
            var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
            var interactPos = transform.position + facingDir;
            Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.LedgeLayer);
            if (collider != null)
            {
                collider.GetComponent<Ledges>()?.JumpDirection();
            }
        }

    }
}

