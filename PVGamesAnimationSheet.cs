using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PVGames Animation Sheet", menuName = "ScriptableObjects/PVGames Animation Sheet", order = 1)]
public class PVGamesAnimationSheet : ScriptableObject
{
    public string ID;
    public int startIndex;
    public int stopIndex;
    public int frameCount;
}