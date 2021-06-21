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
        [SerializeField] private int encounterRate = 25;

        public event Action OnEncountered;

        private bool isMoving;
        private Vector2 input;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
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
                    animator.SetFloat("moveX", input.x);
                    animator.SetFloat("moveY", input.y);

                    var targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;

                    if (IsWalkable(targetPos))
                    {
                        StartCoroutine(Move(targetPos));
                    }                    
                }
            }
            animator.SetBool("isMoving", isMoving);
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
        }

        private bool IsWalkable(Vector3 targetPos)
        {
            if (Physics2D.OverlapCircle(targetPos, 0.12f, solidObjectsLayer) != null)
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
                    animator.SetBool("isMoving", false);
                    OnEncountered();
                }
            }
        }



    }
}
