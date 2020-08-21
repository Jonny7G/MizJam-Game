using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="game progress",menuName ="progress data",order =1)]
public class GameProgressData : ScriptableObject
{
    public int farthestLevel;
    public int CurrentLevel;
}
