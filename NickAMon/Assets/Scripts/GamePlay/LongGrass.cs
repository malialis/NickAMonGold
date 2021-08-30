using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int encounterRate = 25;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= encounterRate)
        {
            Debug.Log("Encountered a pokemon");
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();
        }
    }


}
