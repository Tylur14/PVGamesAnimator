using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionFinder : MonoBehaviour
{
    
    public enum PossibleDirections
    {
        WEST        = 0,    // PVGames Spritesheet Offset = 1
        SOUTH_WEST  = 1,    // PVGames Spritesheet Offset = 4
        SOUTH       = 2,    // PVGames Spritesheet Offset = 0
        SOUTH_EAST  = 3,    // PVGames Spritesheet Offset = 6
        EAST        = 4,    // PVGames Spritesheet Offset = 2
        NORTH_EAST  = 5,    // PVGames Spritesheet Offset = 7
        NORTH       = 6,    // PVGames Spritesheet Offset = 3
        NORTH_WEST  = 7     // PVGames Spritesheet Offset = 5
    }
    
    [SerializeField] private float originOffset = 0.5f; // ? Needs to be reworked, doesn't scale
   
    // Status
    [HideInInspector] public PossibleDirections facingDirection;
    [HideInInspector] public Vector3 mousePos;
    [HideInInspector] public Vector3 debug_direction;
    
    private Camera _cam;
    private LineRenderer _lineRenderer;
    
    
    
    private void Start()
    {
        _cam = Camera.main;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if(!IsPointerOverUIObject())
                GetDirection();
        }
            
    }
    
    // solution by SkylinR
    // https://answers.unity.com/questions/967170/detect-if-pointer-is-over-any-ui-element.html
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void GetDirection()
    {
        mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y += originOffset;
        
        var direction = mousePos - transform.position;
        direction.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        debug_direction = direction;
        
        GetFacingDirection(direction.z);
        if(_lineRenderer) 
            DisplayLine(direction);
    }

    void GetFacingDirection(float angle)
    {
        angle -= 22.5f;
        for (int i = 0; i < 8; i++)
        {
            if (i == 0) // special case because it needs the range of 337.5 - 22.5
            {
                var tempAngle = angle + 22.5;
                if (tempAngle >= 337.5 || tempAngle <= 22.5)
                {
                    facingDirection = (PossibleDirections) i;
                    break;
                }
            }
            if (angle >= i && angle <= i * 45)
            {
                facingDirection = (PossibleDirections) i;
                break;
            }
        }
    }
    void DisplayLine(Vector3 rawDir)
    {
        var offset = new Vector3();
        offset.y -= originOffset;
        
        rawDir.z = Mathf.Sqrt(rawDir.x*rawDir.x + rawDir.y * rawDir.y)/2;
        
        var lineDir = rawDir;
        lineDir.z = 0;
        lineDir.x /= rawDir.z;
        lineDir.y /= rawDir.z;
        _lineRenderer.SetPosition(1,offset+lineDir);
    }
}
