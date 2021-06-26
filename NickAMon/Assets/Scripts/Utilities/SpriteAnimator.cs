using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator 
{
    SpriteRenderer spriteRenderer;
    public List<Sprite> Frames { get { return frames; } }
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;


    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f)
    {
        this.frames = frames;
        this.frameRate = frameRate;
        this.spriteRenderer = spriteRenderer;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if(timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }


}
