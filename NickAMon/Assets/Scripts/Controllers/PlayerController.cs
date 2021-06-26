using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NickAMon.Controller
{
    public class PlayerController: MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private LayerMask solidObjectsLayer;
        [SerializeField] private LayerMask grassLayer;
        [SerializeField] private LayerMask ledgeLayer;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private int encounterRate = 25;

        public event Action OnEncountered;
        public event Action OnEncounteredLedge;

        private bool isMoving;
        private Vector2 input;
        private CharacterAnimator animator;
        public Vector3 playerChange;

        private void Awake()
        {
            animator = GetComponent<CharacterAnimator>();
        }

        public void HandleUpdate()
        {
            if (!isMoving)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");

                if (input.x != 0) input.y = 0; // removes diagonal movement

                if(input != Vector2.zero)
                {
                    animator.MoveX = input.x;
                    animator.MoveY = input.y;

                    var targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;

                    if (IsWalkable(targetPos))
                    {
                        StartCoroutine(Move(targetPos));
                    }                    
                }
            }
            animator.IsMoving = isMoving;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Interact();
            }
        }

        IEnumerator Move(Vector3 targetPos)
        {
            isMoving = true;

            while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;

            isMoving = false;

            CheckForEncounters();
            CheckForLedges();
        }            

        private bool IsWalkable(Vector3 targetPos)
        {
            if (Physics2D.OverlapCircle(targetPos, 0.12f, solidObjectsLayer | interactableLayer) != null)
            {
                return false;
            }
            return true;
        }

        private void CheckForEncounters()
        {
            if (Physics2D.OverlapCircle(transform.position, 0.12f, grassLayer) != null)
            {
               if(UnityEngine.Random.Range(1, 101) <= encounterRate)
                {
                    Debug.Log("Encountered a pokemon");
                    animator.IsMoving = false;
                    OnEncountered();
                }
            }
        }

        private void CheckForLedges()
        {
            var facingDir = new Vector3(animator.MoveX, animator.MoveY);
            var interactPos = transform.position + facingDir;
            Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, ledgeLayer);
            if (collider != null)
            {
                collider.GetComponent<Ledges>()?.JumpDirection();
            }
        }

        private void Interact()
        {
            var facingDir = new Vector3(animator.MoveX, animator.MoveY);
            var interactPos = transform.position + facingDir;
            Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
            if(collider != null)
            {
                collider.GetComponent<Interactable>()?.Interact();
            }
        }





    }
}

//Do this for a buddy
/*
public void HandleUpdate()
{
    if (!character.IsMoving)
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // remove diagonal movement
        if (input.x != 0) input.y = 0;

        if (input != Vector2.zero)
        {
            GameController.Instance.Buddy.Follow(GameController.Instance.PlayerController.transform.position);
            StartCoroutine(character.Move(input, OnMoveOver));
        }
    }

    character.HandleUpdate();

    if (Input.GetKeyDown(KeyCode.Z))
        Interact();
}


public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null, bool force = false)
        {
            animator.moveX = Mathf.Clamp(moveVec.x, -1f, 1f);
            animator.moveY = Mathf.Clamp(moveVec.y, -1f, 1f);

            var targetPos = transform.position;
            targetPos.x += moveVec.x;
            targetPos.y += moveVec.y;

            if (!IsPathClear(targetPos) && !force)
                yield break;

            IsMoving = true;

            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;

            IsMoving = false;

            OnMoveOver?.Invoke();
        }



*/