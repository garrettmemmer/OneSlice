using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gameController;
    private Animator anim;


    public bool playerReady = false;

    void Awake()
    {
        //gameController = FindObjectOfType<GameController>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // gc.GetComponent<GameController>();
        //gc = GetComponent<GameController>();
        anim = GetComponent<Animator>();
        //gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameController)
        //{
       //     print("gamnecontroller isnt getting assigned");
        //}
        //print("Game Controller, animation state is: " + gameController.animationState);
        if (gameController.animationState == 0) //
        {
            anim.SetInteger("CurrentState", 0);
        }
        if (gameController.animationState == 1) //
        {
            anim.SetInteger("CurrentState", 1);
        }
        if (gameController.animationState == 2) //
        {
            anim.SetInteger("CurrentState", 2);
        }
        if (gameController.animationState == 3) //
        {
            anim.SetInteger("CurrentState", 3);
        }
        if (gameController.animationState == 4) //
        {
            anim.SetInteger("CurrentState", 4);
        }
        if (gameController.animationState == 4) //
        {
            anim.SetInteger("CurrentState", 4);
        }
    }
}
