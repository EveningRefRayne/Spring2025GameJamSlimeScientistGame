using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    //Access To Event manager
    [SerializeField] public EventManager eventMan;
    [SerializeField] public int eatValue {get; private set;}

    public void setEatValue(int num)
    {
        eatValue = num;
    }
}
