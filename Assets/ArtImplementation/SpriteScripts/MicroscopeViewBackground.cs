using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroscopeViewBackground : MonoBehaviour
{
    // private component SpriteRenderer = GetComponent<SpriteRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, -0.05f));
    }
}
