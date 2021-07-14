using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject exclamation;
    [SerializeField] private Sprite sprite; //trainer image
    [SerializeField] private string trainerName; //trainers name
    [SerializeField] private GameObject fov;
    [SerializeField] private Dialog dialog;
    [SerializeField] private Dialog dialogAfterLostBattle;

    private bool battleLost = false;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }


    public void Interact(Transform initiator)
    {
        exclamation.SetActive(true);
        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            //speak some smack talk to player
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                Debug.Log("Start battle");
                GameController.Instance.StartTrainerBattle(this);
            }));
            exclamation.SetActive(false);
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterLostBattle));
            exclamation.SetActive(false);
        }
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //show !
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //walk towards player
        var diff = player.transform.position - transform.position;
        var moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.Move(moveVector);

        //speak some smack talk to player
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Start battle");
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
        {
            angle = 90f;
        }
        else if(dir == FacingDirection.Up)
        {
            angle = 180f;
        }
        else if (dir == FacingDirection.Left)
        {
            angle = 270f;
        }
        else if (dir == FacingDirection.Down)
        {
            angle = 0f;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public string TrainerName
    {
        get => trainerName;
    }

    public Sprite TrainerSprite
    {
        get => sprite;
    }

}
