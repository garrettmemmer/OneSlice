 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController: MonoBehaviour
{
    //public PlayerController p1;
    public GameController gc;
    public State currentState;
    public bool bothReady = false;
    public bool playerInput;
    public bool p1Ready = false;

    public Text reactionText;
    public float targetTime = 2.0f;
    public float timerValue = 0.0f;
    public float reactionTime = 0.0f;

    public enum State
    {
        //Post It number
        STATE_IDLE,    //1
        STATE_STANCE,  //2
        STATE_TENSE,  //3
        STATE_TRICK,   //3.1
        STATE_FALSEALARM,    //3.2
        STATE_ATTACK,  //4
        STATE_SWING,   //5
        STATE_DECISION //6
    }

    void Start()
    {
        currentState = State.STATE_IDLE;
       //p1.GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateState();
        //UpdateReadiness();
        InputListener();
        //p1ready = true;
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.STATE_IDLE: //1
                if (bothReady)// p1.playerReady) //(bothReady)
                {
                    print("switching to STANCE STATE");
                    currentState = State.STATE_STANCE;
                    
                }
                break;
            case State.STATE_STANCE: //2
                if (p1Ready) //or enemyInput)
                {
                    print("switching to STANCE GO");
                    currentState = State.STATE_TENSE;
                    print("fuck :)");
                }
                break;
            case State.STATE_TENSE: //3
                print("entered TENSE STATE, waiting for go sound");
                //
                TimerFunction();
                print(timerValue);
                if (playerInput) 
                {
                    print("fuck :)");
                }
                if (timerValue >= targetTime)
                {
                    print("f ");
                    currentState = State.STATE_TENSE;
                }

                break;
            case State.STATE_TRICK: //3.1
                print("entered TRICK STATE, waiting for input");
                break;
            case State.STATE_FALSEALARM: //3.2
                print("entered FALSE ALARM");
                break;
            case State.STATE_ATTACK: //4
                print("entered Attack STATE, waiting for player or CPU to strike");
                break;
            case State.STATE_SWING: //5
                print("t");
                break;
            case State.STATE_DECISION: //6
                print("t");
                break;


            default:
                print("fuck :(");
                break;
        }
    }

    public void UpdateReadiness()
    {
        if (p1Ready)
        {
            bothReady = true;
        }
    }

    public void InputListener()
    {
        //print("input listener called");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space pressed");
            p1Ready = true;
            //gc.bothReady = true;
            //ai.bothReady = true;
        }
    }

    public void TimerFunction()
    {
        timerValue += Time.deltaTime;
    }
}

