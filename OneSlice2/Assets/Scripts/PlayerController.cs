using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public AIEnemy ai;
    //public GameObject gc;
   // public GameController gc;
    public bool playerReady = false;

    // Start is called before the first frame update
    void Start()
    {
       // gc.GetComponent<GameController>();
       //gc = GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

        InputListener();
        
    }

    public void InputListener()
    {
        //print("input listener called");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space pressed");
            playerReady = true;
            //gc.bothReady = true;
            //ai.bothReady = true;
        }
    }
}
