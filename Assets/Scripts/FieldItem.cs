using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parent Class to things on the Field, including the Player and Eatables. Contains the eventmanager and EatValue for all child classes.
public class FieldItem : MonoBehaviour
{
    //Access To Event manager
    [SerializeField] public EventManager eventMan;
    [SerializeField] public int eatValue { get; private set; } = 0;

    public void setEatValue(int num)
    {
        eatValue = num;
    }
}
