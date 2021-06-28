using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    private Character character;

    private float idleTimer = 0f;
    [SerializeField] float idleWaitTime;

    [SerializeField] List<Vector2> movementPattern;
    private int currentPattern = 0;

    private NPCState state;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > idleWaitTime)
            {
                idleTimer = 0f;
                if(movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
                
            }
        }
        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            Debug.Log("I am interacting");
            state = NPCState.Dialog;

            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                state = NPCState.Idle;
                idleTimer = 0f;
            }
            ));
        }

    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;


        state = NPCState.Idle;
    }

}

public enum NPCState
{
    Idle,
    Walking,
    Dialog
}
