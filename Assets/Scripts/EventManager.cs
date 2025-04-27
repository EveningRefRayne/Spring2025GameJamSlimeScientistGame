using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "EventManagerAsset", menuName = "EventManager")]
public class EventManager : ScriptableObject
{
    public UnityEvent Play;
    public UnityEvent Pause;
    public UnityEvent GameStateNone;
    public UnityEvent<Eatables> PlayerAte;
    public UnityEvent<Eatables> ItemExpired;
    public UnityEvent<bool> WaveEnd;
    public UnityEvent WaveEndAgeWipe;
    public UnityEvent SpawnEatables;
    public UnityEvent StartNextWave;
    public UnityEvent TimerLow;
    public UnityEvent GameOver;
}