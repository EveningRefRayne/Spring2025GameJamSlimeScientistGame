using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private GameState mode = GameState.NONE;
    //Access To Event manager
    [SerializeField] public EventManager eventMan;
    //Listens: PlayerAte, WaveEnd, WaveEndAgeWipe, StartNextWave
    [SerializeField] private Player player;
    [SerializeField] private List<Eatables> allItems;
    [SerializeField] private List<Eatables> items;
    [SerializeField] private List<int> currentEatableValues;
    [SerializeField] private List<int> achievableValues;
    public int target { get; private set; }
    public int wave = 0;
    [SerializeField] private float timerMax;
    private float timerBase=10f;
    private float lastTime = 0;
    private float timer;
    private int spawnDelta = 0;
    public float startSpawnDistance = 10;
    public float lowTimerValue = 0.33f;
    public int eatableMaxAge = 3;
    


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
        eventMan.PlayerAte.AddListener(onPlayerAte);
        eventMan.WaveEnd.AddListener(onWaveEnd);
        eventMan.WaveEndAgeWipe.AddListener(onWaveEndAgeWipe);
        eventMan.SpawnEatables.AddListener(onSpawnEatables);
        eventMan.StartNextWave.AddListener(onStartNextWave);
    }

    public void OnDisable()
    {
        eventMan.Play.RemoveListener(onPlay);
        eventMan.Pause.RemoveListener(onPause);
        eventMan.PlayerAte.RemoveListener(onPlayerAte);
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
    private void onPlayerAte(Eatables tar)
    {
        items.Remove(tar);
        currentEatableValues.Remove(tar.eatValue);
    }


    private void onWaveEnd(bool success)
    {
        if (!success)
        {
            //game over
            eventMan?.GameOver?.Invoke();
        }
        else
        {
            wave += wave;
            eventMan?.WaveEndAgeWipe?.Invoke();
        }
    }
    private void onWaveEndAgeWipe()
    {
        //Wipe out any items above a certain age
        foreach(Eatables eat in items)
        {
            if(eat.age>=eatableMaxAge)
            {
                eventMan?.ItemExpired?.Invoke(eat);
            }
        }
        //Invoke SpawnEatables at the end to move to the next step
    }
    private void onSpawnEatables()
    {
        //Spawn more Eatables
        //Spawn a number of Eatables equal to wave, up to 3 max.
        int toSpawn = ((wave > 3) ? 3 : wave);
        int tries = 0;
        spawnDelta = 0;//Reset spawnDelta to zero. May or may not make the game too easy? Maybe it should be halved instead. But it needs to reduce somehow or the numbers that spawn will never go down again...
        foreach (int val in currentEatableValues) spawnDelta = ((Mathf.Abs(val) > spawnDelta) ? Mathf.Abs(val) : spawnDelta); //check if any are bigger than the spawn delta and increase the delta if they are.
        spawnDelta = spawnDelta + wave;
        while (toSpawn > 0 && tries < allItems.Count)
        {
            GameObject check = allItems[tries].gameObject;
            if (!check.activeInHierarchy)
            {
                toSpawn -= 1;
                //Do the setup for it to spawn
                check.GetComponent<Renderer>().enabled = false;
                check.SetActive(true);
                check.GetComponent<Eatables>().spawnSetup(Random.Range(-spawnDelta, spawnDelta), Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward * startSpawnDistance);
                items.Add(check.GetComponent<Eatables>());
                currentEatableValues.Add(check.GetComponent<Eatables>().eatValue);
                check.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                tries += 1;
            }
        }
        //finished spawning
        //Need to recalculate viable target values now
        recalculateAchievableValues();
        //Then invoke the event to move to the next wave.
    }
    //Invoke StartNextWave at the end to move to the next step
    private void onStartNextWave()
    {
        //Do stuff to determine the next target value
        setTarget(achievableValues[Random.Range(0, (achievableValues.Count - 1))]);//make it whatever it needs to be
    }

    private void FixedUpdate()
    {
        //Short circuit if game isn't in play mode
        if (mode == GameState.NONE || mode == GameState.PAUSE) return;
        if (timer > 0)
        {
            timer = timer - (Time.time - lastTime);
            lastTime = Time.time;
            if (timer/timerMax<=lowTimerValue)
            {
                eventMan?.TimerLow?.Invoke();
            }
        }
        else if (timer <= 0)
        {
            if (player.eatValue == target) eventMan?.WaveEnd?.Invoke(true);
            else eventMan?.WaveEnd?.Invoke(false);
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
        achievableValues.Clear();
        int i = 0;
        int j = 1;
        int k = 2;
        while (i<currentEatableValues.Count)
        {
            while (j < currentEatableValues.Count)
            {
                while(k<currentEatableValues.Count)
                {
                    achievableValues.Add(player.eatValue+currentEatableValues[i] + currentEatableValues[j] + currentEatableValues[k]);
                    k += 1;
                }
                achievableValues.Add(player.eatValue+currentEatableValues[i] + currentEatableValues[j]);
                j += 1;
                k = j + 1;
            }
            achievableValues.Add(player.eatValue+currentEatableValues[i]);
            i += 1;
            j = i + 1;
            k = j + 1;
        }
    }



}