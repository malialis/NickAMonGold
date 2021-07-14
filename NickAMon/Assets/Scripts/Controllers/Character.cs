using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterAnimator animator;
    public bool IsMoving { get; private set; }
    public float OffsetY { get; private set; } = 0.3f;

    [Range(1f, 15f)]
    [SerializeField] private float moveSpeed;

    public event Action OnEncountered;
    public event Action OnEncounteredLedge;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;
        animator.IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;
        yield return StandStill();

        OnMoveOver?.Invoke();        
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.12f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public CharacterAnimator Animator { get => animator; }

    public void HandleUpdate()
    {
        //animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer))
            return false; // not clear

        return true;

    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if(xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {
            Debug.Log("You can not look diagonally");
        }
    }

    private IEnumerator StandStill()
    {
        Vector3 myPosition = this.transform.position;
        yield return new WaitForSeconds(.1f);

        if (myPosition == transform.position)
        {
            animator.IsMoving = false;
        }
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

}

/*
 * public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null, bool force = false)
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