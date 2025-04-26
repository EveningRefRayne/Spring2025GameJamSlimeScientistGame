using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eatables : FieldItem
{
    private GameState mode = GameState.NONE;
    private int age = 0;
    //Listens: Play, Pause, ItemExpired
    //Invokes: PlayerAte

    public void OnEnable()
    {
        eventMan?.Play?.AddListener(onPlay);
        eventMan?.Pause?.AddListener(onPause);
        eventMan?.PlayerAte?.AddListener(onPlayerAte);
        eventMan?.ItemExpired?.AddListener(onItemExpired);
    }
    public void OnDisable()
    {
        eventMan?.Play?.RemoveListener(onPlay);
        eventMan?.Pause?.RemoveListener(onPause);
        eventMan?.PlayerAte?.RemoveListener(onPlayerAte);
        eventMan?.ItemExpired?.RemoveListener(onItemExpired);
    }
    private void onPlay()
    {
        //Do stuff again
        mode = GameState.PLAY;
    }
    private void onPause()
    {
        //Stop doing everything
        mode = GameState.PAUSE;
    }
    private void onPlayerAte(Eatables target)
    {
        if(target==this)
        {
            //It got eaten. Delete itself (or maybe just disable because that's less expensive?
        }
    }

    private void onItemExpired(Eatables target)
    {
        if(target==this)
        {
            //Delete itself, and maybe some other stuff?
        }
    }
}
