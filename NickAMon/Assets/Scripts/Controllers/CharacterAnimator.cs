using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    [SerializeField] List<Sprite> fallingDownSprites;
    [SerializeField] List<Sprite> fallingRightSprites;
    [SerializeField] List<Sprite> fallingLeftSprites;
    
    //parameter
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    bool wasPreviouslyMoving;
    public bool IsJumping { get; set; }

    //states
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;

    //falling anims
    SpriteAnimator fallingDownAnim;
    SpriteAnimator fallingLeftAnim;
    SpriteAnimator fallingRightAnim;

    SpriteAnimator currentAnim;

    //references
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);

        fallingDownAnim = new SpriteAnimator(fallingDownSprites, spriteRenderer);
        fallingLeftAnim = new SpriteAnimator(fallingLeftSprites, spriteRenderer);
        fallingRightAnim = new SpriteAnimator(fallingRightSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var previousAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        if (currentAnim != previousAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
    }

    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }

}

public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
}
