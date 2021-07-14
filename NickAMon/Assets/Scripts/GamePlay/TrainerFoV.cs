using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFoV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        Debug.Log("In trainers view");
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }

    
}
