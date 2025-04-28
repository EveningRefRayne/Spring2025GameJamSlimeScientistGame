using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] public EventManager eventMan;
    [SerializeField] public GameObject gameOverUI;

    public void OnEnable()
    {
        eventMan.GameOver.AddListener(onGameOver);
    }

    public void OnDisable()
    {
        eventMan.GameOver.RemoveListener(onGameOver);
    }

    public void onGameOver()
    {
        gameOverUI.SetActive(true);
        eventMan?.Pause?.Invoke();
    }
}

