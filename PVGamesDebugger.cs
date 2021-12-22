using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PVGamesDebugger : MonoBehaviour
{
    [Header("Tracked Components")]
    [SerializeField] private DirectionFinder finder;
    [SerializeField] private PVGamesAnimator animator;
    
    
    [SerializeField] private TextMeshProUGUI debugOutput;

    private void Update()
    {
        if(animator.animation!=null)
            DisplayDebugInfo();
    }

    void DisplayDebugInfo()
    {
        debugOutput.text = "";
        debugOutput.text += "X: " +                 finder.debug_direction.x + "\n";
        debugOutput.text += "Y: " +                 finder.debug_direction.y + "\n";
        debugOutput.text += "Angle: " +             finder.debug_direction.z.ToString("0") + "\n";
        
        debugOutput.text += "Mouse Position: "   +  finder.mousePos             + "\n";
        debugOutput.text += "Facing Direction: " +  finder.facingDirection      + "\n";
        debugOutput.text += "Animation Offset: " +  animator.directionOffset    + "\n";
        debugOutput.text += "Animation Name: "   +  animator.animation.name     + "\n";
        debugOutput.text += "Animation Frame: "  +  animator.frameIndex         + "\n";
        
    }
}
