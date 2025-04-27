using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Eatables : FieldItem
{
    private GameState mode = GameState.PLAY;
    public int age { get; private set; } = 0;
    //Listens: Play, Pause, ItemExpired


    [SerializeField] private NavMeshAgent navAgent;
    public Vector3 moveTarget;
    public List<GameObject> moveBeacons;
    public float AIUpdateTime;
    private float lastAIUpdateTime = 0;
    public float AITimeMin = 0.25f;
    public float AITimeMax = 0.75f;

    public void spawnSetup(int val, Vector3 startPos)
    {
        resetAge();
        setEatValue(val);
        gameObject.transform.position = startPos;
    }


    public void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        randomizeNextUpdateTime();
    }

    public void randomizeNextUpdateTime()
    {
        AIUpdateTime = Random.Range(AITimeMin, AITimeMax);
    }

    public void setupNav(List<GameObject> bcn)
    {
        moveBeacons = bcn;
    }

    public void OnEnable()
    {
        eventMan.Play.AddListener(onPlay);
        eventMan.Pause.AddListener(onPause);
        eventMan.PlayerAte.AddListener(onPlayerAte);
        eventMan.ItemExpired.AddListener(onItemExpired);
        eventMan.WaveEnd.AddListener(onWaveEnd);
    }
    public void OnDisable()
    {
        eventMan.Play.RemoveListener(onPlay);
        eventMan.Pause.RemoveListener(onPause);
        eventMan.PlayerAte.RemoveListener(onPlayerAte);
        eventMan.ItemExpired.RemoveListener(onItemExpired);
        eventMan.WaveEnd.RemoveListener(onWaveEnd);
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
            //It got eaten. Delete itself (or maybe just disable because that's less expensive? And will probably break less other stuff that relies on it.
            this.gameObject.SetActive(false);
        }
    }
    private void onWaveEnd(bool success)
    {
        if(success==true) age += age;
    }

    private void onItemExpired(Eatables target)
    {
        if(target==this)
        {
            //Delete itself, and maybe some other stuff. Or probably just disable itself instead.
            this.gameObject.SetActive(false);
        }
    }
    public void resetAge()
    {
        age = 0;
    }

    public void FixedUpdate()
    {
        //Don't do anything when game isn't in play mode
        if (mode == GameState.PAUSE || mode == GameState.NONE) return;
        //Movement AI
        if (Time.time >= lastAIUpdateTime + AIUpdateTime)
        {
            navAgent.SetDestination(moveBeacons[Random.Range(0, (moveBeacons.Count))].transform.position);
            lastAIUpdateTime = Time.time;
            randomizeNextUpdateTime();
        }
    }
}

