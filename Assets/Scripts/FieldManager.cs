using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private GameState mode = GameState.NONE;
    //Access To Event manager
    [SerializeField] public EventManager eventMan;
    //Listens: PlayerAte, WaveEnd, WaveEndAgeWipe, StartNextWave

    [SerializeField] private List<FieldItem> items;
    [SerializeField] private List<int> eatableValues;
    [SerializeField] private List<int> achievableValues;
    public int target { get; private set; }
    public int wave = 0;
    [SerializeField] private float timerMax;
    private float timerBase=10f;
    private float lastTime = 0;
    private float timer;
    


    public List<int> getAchievableValues()
    {
        return achievableValues;
    }
    public void setTarget(int tar)
    {
        if (achievableValues.Contains(tar))
        {
            target = tar;
        }
        else
        {
            Debug.LogException(new System.Exception("Somehow selected a target value that cannot be achieved."));
        }
    }

    public void OnEnable()
    {
        eventMan.Play.AddListener(onPlay);
        eventMan.Pause.AddListener(onPause);
        eventMan.WaveEnd.AddListener(onWaveEnd);
        eventMan.WaveEndAgeWipe.AddListener(onWaveEndAgeWipe);
        eventMan.SpawnEatables.AddListener(onSpawnEatables);
        eventMan.StartNextWave.AddListener(onStartNextWave);
    }

    public void OnDisable()
    {
        eventMan.Play.RemoveListener(onPlay);
        eventMan.Pause.RemoveListener(onPause);
        eventMan.WaveEnd.RemoveListener(onWaveEnd);
        eventMan.WaveEndAgeWipe.RemoveListener(onWaveEndAgeWipe);
        eventMan.SpawnEatables.RemoveListener(onSpawnEatables);
        eventMan.StartNextWave.RemoveListener(onStartNextWave);
    }

    private void onPlay()
    {
        mode = GameState.PLAY;
        lastTime = Time.time;
        Time.timeScale = 1;
    }
    private void onPause()
    {
        mode = GameState.PAUSE;
        lastTime = Time.time;
        Time.timeScale = 0;
    }

    private void onWaveEnd(bool success)
    {
        if (!success)
        {
            //game over
        }
    }
    private void onWaveEndAgeWipe()
    {
        //Wipe out any items above a certain age

        //Invoke SpawnEatables at the end to move to the next step
    }
    private void onSpawnEatables()
    {
        //Spawn more Eatables
        
        //Invoke StartNextWave at the end to move to the next step
    }
    private void onStartNextWave()
    {
        //Do stuff to determine the next target value
        setTarget(0);//make it whatever it needs to be

    }

    private void FixedUpdate()
    {
        //Short circuit if game isn't in play mode
        if (mode == GameState.NONE || mode == GameState.PAUSE) return;
        if (timer > 0)
        {
            timer = timer - (Time.time - lastTime);
            lastTime = Time.time;
        }
        else if (timer <= 0)
        {
            wave += wave;
        }
    }
    private float resetTimer()
    {
        timerMax = timerBase + (wave / 4);
        timer = timerMax;
        return timer;
    }

    private void recalculateAchievableValues ()
    {
        //Do whatever it needs to do to accomplish this
    }



}