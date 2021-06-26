using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NickAMon.Controller;

public class Ledges : MonoBehaviour
{    
    public Vector3 playerChange;
    [SerializeField] private PlayerController playerController;
    /*
    public bool needText;
    public string placeName;
    public GameObject text;
    public Text placeText;

    */

    private void Start()
    {
        playerController.OnEncounteredLedge += JumpDirection;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player is entering me");
        if (other.CompareTag("Player"))
        {    
            other.transform.position += playerChange;
            Debug.Log("Player is entering me");
           /*
            if (needText)
            {
                StartCoroutine(PlaceNameCoroutine());
            }
           */
        }
    }

    public void JumpDirection()
    {
        playerController.transform.position += playerChange;
    }

/*
    private IEnumerator PlaceNameCoroutine()
    {
        text.SetActive(true);
        placeText.text = placeName;
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
    }
   */     
}
