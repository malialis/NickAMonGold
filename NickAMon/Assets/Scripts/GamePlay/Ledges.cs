using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ledges : MonoBehaviour, Interactable
{    
    [SerializeField] private Vector3 playerChange;
    //[SerializeField] private PlayerController playerController;
    [SerializeField] private float durationOfFall;
    
    private void Start()
    {
       // playerController.OnEncounteredLedge += JumpDirection;
    }

    public void Interact(Transform initiator)
    {
        StartCoroutine(SmoothJumpDown(initiator));

    }



    public void JumpDirection()
    {
       // playerController.transform.position += playerChange;
    }

    IEnumerator SmoothJumpDown(Transform initiator)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < durationOfFall)
        {
            initiator.position = Vector3.Lerp(startPosition, playerChange, time / durationOfFall);
            time += Time.deltaTime;
            yield return null;
        }
        initiator.position = playerChange;
    }



}
