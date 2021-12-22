using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationAction
{
    public string animationName;
    public PVGamesAnimationSheet animationSheet;
}



public class PVGamesAnimator : MonoBehaviour
{
    [SerializeField] private float frameRate;
    public PVGamesAnimationSheet animation;
    public Sprite[] sheet;
    public int frameIndex;
    public int directionOffset;
    public int startOffset;
    public int frameCount;

    private int _currentDirection;
    private DirectionFinder dir;
    private SpriteRenderer _spriteRenderer;
    private float _timer;
    
    private void Start()
    {
        dir = GetComponent<DirectionFinder>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        LoadAnimation(animation);
    }

    private void Update()
    {
        if (_currentDirection != (int) dir.facingDirection)
        {
            _currentDirection = (int) dir.facingDirection;
            GetDirectionOffset();
        }
        TryAnimate();
    }

    void SetSprite()
    {
        int index = startOffset+(directionOffset * frameCount) + frameIndex;
        _spriteRenderer.sprite = sheet[index];
    }

    void TryAnimate()
    {
        if (animation == null)
            return;
        if (_timer > 0)
            _timer -= Time.deltaTime;
        else Animate();
    }
    
    void Animate()
    {
        _timer = frameRate;
        frameIndex++;
        if (frameIndex > frameCount-1)
            frameIndex = 0;
        SetSprite();
    }
    
    void GetDirectionOffset()
    {
        // WEST
        if (dir.facingDirection         == DirectionFinder.PossibleDirections.WEST)
            directionOffset = 1;
        
        // SOUTH
        else if (dir.facingDirection    == DirectionFinder.PossibleDirections.SOUTH)
            directionOffset = 0;
        
        // EAST and NORTH
        else if (dir.facingDirection    == DirectionFinder.PossibleDirections.EAST ||
                 dir.facingDirection    == DirectionFinder.PossibleDirections.NORTH)
            directionOffset = (int) dir.facingDirection / 2;
        
        // SOUTH_WEST and SOUTH_EAST
        else if (dir.facingDirection    == DirectionFinder.PossibleDirections.SOUTH_WEST ||
                 dir.facingDirection    == DirectionFinder.PossibleDirections.SOUTH_EAST)
            directionOffset = (int) dir.facingDirection + 3;
        
        // NORTH_EAST
        else if(dir.facingDirection     == DirectionFinder.PossibleDirections.NORTH_EAST)
            directionOffset = 7;
        
        // NORTH_WEST
        else if(dir.facingDirection     == DirectionFinder.PossibleDirections.NORTH_WEST)
            directionOffset = 5;
    }

    public void LoadAnimation(PVGamesAnimationSheet incomingAnimation)
    {
        animation = incomingAnimation;
        sheet = Resources.LoadAll<Sprite>("Mythos/"+animation.ID); // Need to add function to check if we already have it loaded
        startOffset = animation.startIndex;
        frameCount  = animation.frameCount;
        if (frameCount <= 0)
            frameCount = 8;
        SetSprite();
    }
}
