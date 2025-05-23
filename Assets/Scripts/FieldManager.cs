using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldManager : MonoBehaviour
{
    private GameState mode = GameState.PLAY;
    public List<GameObject> moveBeacons;
    public GameObject timerLink;
    public GameObject goalLink;
    public GameObject EatablePrefab;
    //Access To Event manager
    [SerializeField] public EventManager eventMan;
    //Listens: PlayerAte, WaveEnd, WaveEndAgeWipe, StartNextWave
    [SerializeField] private Player player;
    [SerializeField] private List<Eatables> allItems;
    [SerializeField] private List<Eatables> items;
    [SerializeField] private List<int> currentEatableValues;
    [SerializeField] private List<int> achievableValues;
    public int target { get; private set; } = 0;
    public int wave = 0;
    [SerializeField] private float timerMax;
    private float timerBase=10f;
    private float lastTime = 0;
    private bool starting = true;
    private float startupTimer = 1.5f;
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

    public void Awake()
    {
        GameObject making;
        for(int i=0;i<9;i++)
        {
            making = Instantiate<GameObject>(EatablePrefab);
            making.GetComponent<Eatables>().setupNav(moveBeacons);
            allItems.Add(making.GetComponent<Eatables>());
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
        player.setEatValue(player.eatValue + tar.eatValue);
        player.gameObject.GetComponentInChildren<TMP_Text>().text = "" + player.eatValue;
        Debug.Log("player at: " + player.eatValue);
        items.Remove(tar);
        currentEatableValues.Remove(tar.eatValue);
    }


    private void onWaveEnd(bool success)
    {
        Debug.Log("Called Wave End");
        if (!success)
        {
            Debug.Log("Player Didn't match Target, game over.");
            //game over
            eventMan?.GameOver?.Invoke();
        }
        else
        {
            
            wave = wave + 1;
            eventMan?.WaveEndAgeWipe?.Invoke();
        }
    }
    private void onWaveEndAgeWipe()
    {
        Debug.Log("Called Age Wipe");
        //Wipe out any items above a certain age
        foreach(Eatables eat in items)
        {
            if(eat.age>=eatableMaxAge)
            {
                eventMan?.ItemExpired?.Invoke(eat);
            }
        }
        //Invoke SpawnEatables at the end to move to the next step
        eventMan?.SpawnEatables?.Invoke();
    }
    private void onSpawnEatables()
    {
        //Debug.Log("Called Spawn Eatables");
        //Spawn more Eatables
        //Spawn a number of Eatables equal to wave, up to 3 max.
        Debug.Log("Wave is still: " + wave);
        int toSpawn = ((wave > 3) ? 3 : wave);
        Debug.Log("Need to spawn: " + toSpawn);
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
                check.GetComponentInChildren<SpriteRenderer>().enabled = false;
                check.SetActive(true);
                if(wave==1)check.GetComponent<Eatables>().spawnSetup(1, Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward * startSpawnDistance);
                else check.GetComponent<Eatables>().spawnSetup(Random.Range(-spawnDelta, spawnDelta), Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward * startSpawnDistance);
                items.Add(check.GetComponent<Eatables>());
                currentEatableValues.Add(check.GetComponent<Eatables>().eatValue);
                if(check.GetComponent<Eatables>().eatValue>=0)
                {
                    check.GetComponentInChildren<Animator>().Play("GreenEatable");
                    check.GetComponentInChildren<TMP_Text>().text = "+" + check.GetComponent<Eatables>().eatValue;

                }
                else if(check.GetComponent<Eatables>().eatValue<0){
                    check.GetComponentInChildren<Animator>().Play("NotGreenEatable");
                    check.GetComponentInChildren<TMP_Text>().text = "" + check.GetComponent<Eatables>().eatValue;
                }
                
                check.GetComponentInChildren<SpriteRenderer>().enabled = true;
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
        eventMan?.StartNextWave?.Invoke();
    }
    //Invoke StartNextWave at the end to move to the next step
    private void onStartNextWave()
    {
        //Do stuff to determine the next target value
        
        setTarget(achievableValues[Random.Range(0, (achievableValues.Count))]);//make it whatever it needs to be
        goalLink.GetComponent<TMP_Text>().text = "" + target;
    }

    private void FixedUpdate()
    {
        //Short circuit if game isn't in play mode
        if (mode == GameState.NONE || mode == GameState.PAUSE) return;
        if (starting)
        {
            Debug.Log("Starting");
            if(startupTimer>0)
            {
                Debug.Log("Starting Timer countdown");
                startupTimer = startupTimer - (Time.time - lastTime);
            }
            else if (startupTimer<=0)
            {
                Debug.Log("Starting Timer hit 0, game startup");
                starting = false;
                resetTimer();
                eventMan?.Play?.Invoke();
                eventMan?.WaveEnd?.Invoke(true);
            }
            return;//short circuit here when doing startup to avoid running all the other usual timer code.
        }
        if (timer > 0)
        {
            timerLink.GetComponent<Image>().fillAmount = (timer/timerMax);
            //Debug.Log("Timer Counting Down");
            timer = timer - (Time.time - lastTime);
            lastTime = Time.time;
            if (timer/timerMax<=lowTimerValue)
            {
                eventMan?.TimerLow?.Invoke();
            }
        }
        else if (timer <= 0)
        {
            Debug.Log("Timer hit zero, wave end");
            resetTimer();
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
        Debug.Log(achievableValues);
        Debug.Log("Achievable Values");
        foreach(int aaaaa in achievableValues)
        {
            Debug.Log("aaa" + aaaaa);
        }
    }



}