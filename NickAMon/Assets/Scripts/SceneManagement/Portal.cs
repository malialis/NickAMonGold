using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private string sceneToLoad;
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
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        Debug.Log("Portal is pausing...");
        
        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destinationPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        GameController.Instance.PauseGame(false);
        Debug.Log("I am not paused no more");

        yield return fader.FadeOut(0.5f);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;


}

public enum DestinationIdentifier
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I
}
