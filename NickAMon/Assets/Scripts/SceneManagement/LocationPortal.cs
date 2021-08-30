using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    //telelports the player to a different position without swaping scenes.
    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DestinationIdentifier destinationPortal;

    private Fader fader;

    PlayerController player;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        Debug.Log("Player entered the portal " + gameObject.name);
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleporting());
    }

    private IEnumerator Teleporting()
    {
        GameController.Instance.PauseGame(true);
        Debug.Log("Portal is pausing...");
        yield return fader.FadeIn(0.5f);
        
        var destinationPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);       
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        GameController.Instance.PauseGame(false);
        Debug.Log("I am not paused no more");
        yield return fader.FadeOut(0.5f);

    }

    public Transform SpawnPoint => spawnPoint;

}
