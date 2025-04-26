using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FieldItem
{
    private GameState mode = GameState.NONE;
    //Listens: Play, Pause, PlayerAte

    public void OnEnable()
    {
        eventMan?.Play?.AddListener(onPlay);
        eventMan?.Pause?.AddListener(onPause);
        eventMan?.PlayerAte?.AddListener(onPlayerAte);
    }
    public void OnDisable()
    {
        eventMan?.Play?.RemoveListener(onPlay);
        eventMan?.Pause?.RemoveListener(onPause);
        eventMan?.PlayerAte?.RemoveListener(onPlayerAte);
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
        //modify player's eatValue based on the value of its target
    }

    public void FixedUpdate()
    {

        //Example invoke code for event system
            /*
            eventMan.TestEvent.Invoke();
            eventMan.TestEventWParams.Invoke(true, TestState.abc);
            eventMan.TestEventNestParams.Invoke(TestState.phase.PHASE1);
            */
    }
}
