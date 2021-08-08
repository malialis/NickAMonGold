using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    public bool IsLoaded { get; private set; }
    [SerializeField] private List<SceneDetails> connectedScenes;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");
            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            //load all connected scenes
            foreach(var scene in connectedScenes)
            {
                scene.LoadScene();
            }
            //unload scenes that are not needed
            if(GameController.Instance.PreviousScene != null)
            {
                var previoslyLoadedScenes = GameController.Instance.PreviousScene.connectedScenes;
                foreach (var scene in previoslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                    {

                    }
                }
            }

        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

}
