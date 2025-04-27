using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FieldItem
{
    private GameState mode = GameState.NONE;
    [SerializeField] private float playerSpeed;
    private Vector3 mov;
    //Listens: Play, Pause
    //Invokes: PlayerAte
    public int age {get; private set; } = 0;

    public void OnEnable()
    {
        eventMan.Play.AddListener(onPlay);
        eventMan.Pause.AddListener(onPause);
    }
    public void OnDisable()
    {
        eventMan.Play.RemoveListener(onPlay);
        eventMan.Pause.RemoveListener(onPause);
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
    
    public void FixedUpdate()
    {
        //Don't do anything when game isn't in play mode
        if (mode == GameState.PAUSE || mode == GameState.NONE) return;

        // Gather input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        horizontal = ((horizontal > 0) ? 1 : ((horizontal < 0) ? -1 : 0));
        vertical = ((vertical > 0) ? 1 : ((vertical < 0) ? -1 : 0));
        if (Mathf.Sqrt(horizontal * horizontal + vertical * vertical) > 0)
        {
            mov.Set(horizontal, vertical, 0);
            transform.localPosition += transform.rotation * mov * Time.deltaTime * playerSpeed;
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if you hit an Eatable and eat it. Will call PlayerAte Invoke after adding the eatable value to its own.
        if (collision.gameObject.GetComponent<Eatables>()) eventMan?.PlayerAte?.Invoke(collision.GetComponent<Eatables>());
    }
}