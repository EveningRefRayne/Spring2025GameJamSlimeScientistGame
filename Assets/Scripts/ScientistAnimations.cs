using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistAnimations : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("ScientistNeutral");
    }

    [SerializeField] public EventManager eventMan;
    [SerializeField] public GameObject gameOverUI;

    public void OnEnable()
    {
        eventMan.PlayerAte.AddListener(onPlayerAte);
        eventMan.GameOver.AddListener(onGameOver);
        eventMan.TimerLow.AddListener(onTimerLow);
        eventMan.WaveEnd.AddListener(onWaveEnd);
    }

    public void OnDisable()
    {
        eventMan.PlayerAte.RemoveListener(onPlayerAte);
        eventMan.GameOver.RemoveListener(onGameOver);
        eventMan.TimerLow.RemoveListener(onTimerLow);
        eventMan.WaveEnd.RemoveListener(onWaveEnd);
    }

    public void onPlayerAte(Eatables aore)
    {
        anim.Play("ScientistEating");
    }

    public void onGameOver()
    {
        anim.Play("ScientistLoss");
    }
    
    public void onTimerLow()
    {
        anim.Play("ScientistCloseToLosing");
    }

    public void onWaveEnd(bool something)
    {
        if(something)
        {
            anim.Play("ScientistWin");
        }
    }
}
