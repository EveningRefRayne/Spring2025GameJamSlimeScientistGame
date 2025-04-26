using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState { NONE, PLAY, PAUSE }
public class GameStateManager
{
    //Access To Event manager
    [SerializeField] private EventManager eventMan;

    //The Forms for the UnityEvents I'll be setting up in the event manager
    //public UnityEvent TestEvent;
    //public UnityEvent<bool,someEnums> TestEventWParams;
    //public UnityEvent<TestState.phase> TestEventNestParams;
}
