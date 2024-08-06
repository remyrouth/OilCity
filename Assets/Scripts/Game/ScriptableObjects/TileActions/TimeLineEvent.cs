using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimedEvent", menuName ="TimeLine/NewTimeLineEvent")]
public class TimeLineEvent : ScriptableObject
{
    [Range(0,1)] public float GamePercentage;
    public int triggerYear;
    public Sprite newspaperSprite;
    public LanguageItem title;
    public LanguageItem paragraph;
}
