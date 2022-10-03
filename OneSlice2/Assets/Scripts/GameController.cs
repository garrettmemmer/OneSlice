using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject player1;
    public GameObject cpu1;
    public GameObject cpu2;
    public GameObject cpu3;
    public GameObject cpu4;
    public GameObject cpu5;
    public GameObject cpu6; 
    public GameObject enemyFightingSpot;

    public GameController gc;
    //public CameraShake cameraShake; 
    public TraumaInducer tramaInducer;
    public State currentState;

    public int currentRound = 1;

    public TextMeshProUGUI txt;
    public Canvas readyCanvas;
    public Canvas reactionCanvas;
    public Canvas decisionCanvas;
    public Canvas nextRoundCanvas;
    public Canvas transitionCanvas;

    public int animationState = 0;
    public bool bothReady = false;
    public bool playerInput;
    public bool p1Ready = false;
    public bool playerWin = false;
    public bool cpuWin = false;
    private bool waiterBool = false;
    public bool waiter2Bool = false;
    private bool inResetState = false;

    public Text reactionText;

    public float targetTime = 2.0f;
    public float timerValue = 0.0f;
    public float reactionTimeCPU = 3.0f;
    public float reactionTimeDisplay = 0.0f;
    private string timerDisplay;

    private Vector3 defaultTransitionScale;
    private Vector3 defaultTransitionPosition;

    private Vector3 scaleChange;
    private Vector3 positionChange;
    private bool moveCanvas = false;

    public enum State
    {
        //Post It number
        STATE_IDLE,    //1
        STATE_STANCE,  //2
        STATE_GO,  //3
        STATE_TRICK,   //3.1
        STATE_FALSEALARM,    //3.2
        STATE_ATTACK,  //4
        STATE_DECISION, //5
        STATE_RESET    //6
    }

    void Start()
    {
        defaultTransitionScale = transitionCanvas.transform.localScale;
        defaultTransitionPosition = transitionCanvas.transform.position;
        reactionCanvas.enabled = false;
        decisionCanvas.enabled = false;
        nextRoundCanvas.enabled = false;
        transitionCanvas.enabled = false;
        scaleChange = new Vector3(0f, -0.005f, 0f);
        positionChange = new Vector3(2f, 0f, 0.0f);
        currentState = State.STATE_IDLE;
    }

    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.STATE_IDLE: //1

                readyCanvas.enabled = true;
                waiterBool = false;
                InputListener();
                UpdateReadiness();
                break;

            case State.STATE_STANCE: //2

                animationState = 1;

                
                StanceLogic();
                break;

            case State.STATE_GO: //3

                reactionCanvas.enabled = true;

                GoLogic();
                break;

            case State.STATE_TRICK: //3.1
                print("entered TRICK STATE, waiting for input");
                break;
            case State.STATE_FALSEALARM: //3.2
                print("entered FALSE ALARM");
                break;

            case State.STATE_ATTACK: //4
                reactionCanvas.enabled = false;
                decisionCanvas.enabled = true;

                //adding screenshake here
                // StartCoroutine(cameraShake.Shake(.15f, .4f)); this was clunckuy with my own code
                //lets try with the assest store :)
                StartCoroutine(tramaInducer.StartShaking());
                
                
                //transitionCanvas.enabled = true;
                AttackLogic();
                break;

            case State.STATE_DECISION: //5
                playerInput = false;
                DecisionLogic();
                break;

            case State.STATE_RESET: //6
                playerInput = false;
                nextRoundCanvas.enabled = false;
                //decisionCanvas.enabled = false;
                //inResetState = true;
                timerValue = 0.0f;
                txt.text = "0.0";//timerValue.ToString();
                LoadRound(currentRound);
                break;

            default:
                print("fuck :(");
                break;
        }
    }

    public void UpdateReadiness()
    {

        if (playerInput)// p1.playerReady) //(bothReady)
        {
            //sound logic //gong sound
            //we could also just create a reference variable up top
            //that we would initilaize in the inspector
            FindObjectOfType<AudioManager>().Play("Gong");
            //print("switching to STANCE STATE");
            currentState = State.STATE_STANCE;
            readyCanvas.enabled = false;
        }
    }

    public void StanceLogic()
    {
        TimerFunction();
        InputListener();
        if (playerInput && timerValue < targetTime)
        {
            playerInput = false;
            //animationstate = stumble
            //sound = whoa
            print("someone swung too early, soon we will develop a way to punish this");
        }
        if (timerValue >= targetTime)
        {
            FindObjectOfType<AudioManager>().Play("StickBreak");
            currentState = State.STATE_GO;
        }
    }

    public void GoLogic()
    {
        TimerFunction();
        InputListener();
        reactionTimeCPU = Random.Range(5.0f, 7.0f);

        if (reactionTimeCPU <= timerValue)
        {
            cpuWin = true;
            print("cpu won and move to attack");
            currentState = State.STATE_ATTACK;

            // print("enemy attacked and you died");
        }
        if (playerInput)
        {
            playerWin = true;
            currentState = State.STATE_ATTACK;
        }
    }

    public void AttackLogic()
    {
        //TransitionScreen();
        //play the slice sound here
        //FindObjectOfType<AudioManager>().Play("SwordSlice");
        FindObjectOfType<AudioManager>().Play("Slice");
        SliceScreen(); // i think that slice screen needs to be the one that makes the transition to a new state
        if (playerWin)
        {
            animationState = 2;
            //cpuAnimationState = 2.5;

            //another sound here, sheathing sword and mayybe enemy death
            //FindObjectOfType<AudioManager>().Play("SheathSword");
           //FindObjectOfType<AudioManager>().Play("EnemyDeath");

            StartCoroutine(waiter(4));
            if (waiterBool)
            {
                waiterBool = false;
               // currentState = State.STATE_DECISION;
            }
        }
        if (cpuWin)
        {
            animationState = 4;
            //cpuAnimationState = ???; //winning slash
            StartCoroutine(waiter(4));
            if (waiterBool)
            {
                waiterBool = false;
                currentState = State.STATE_DECISION;
            }
        }
    }

    public void DecisionLogic()
    {
        InputListener();
        //next round/page turn switching to next round noise
        FindObjectOfType<AudioManager>().Play("PageTurn");
        nextRoundCanvas.enabled = true;
        if (playerWin)
        {
            animationState = 3;
        }

        if(playerInput)
        {
            print("next round would start here");

           if(currentRound != 4)
           {
                currentRound += 1;
                currentState = State.STATE_RESET;
            } 
        }
    }

    public void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space pressed");
            playerInput = true;
            //gc.bothReady = true;
            //ai.bothReady = true;
        }
    }

    public void TimerFunction()
    {
        timerValue += Time.deltaTime;
        //timerValue - targetTime = timer
        timerDisplay = (timerValue - targetTime).ToString();
        if((timerValue - targetTime) >= 0f)
        {
            txt.text = timerDisplay;
            //txt.text = "wtf";
        }
        
    }

    IEnumerator waiter(int n)
    { 
        //Wait for n seconds
        yield return new WaitForSeconds(n);
        waiterBool = true;
    }

    public void SliceScreen()
    {
        //black out the screen
        if (decisionCanvas.transform.localScale.y >= 0)
        {
            decisionCanvas.transform.localScale += scaleChange;
            moveCanvas = true;
        }
        //move the screen out of the window
        if (moveCanvas)
        {
            decisionCanvas.transform.position += positionChange;
        }
        if (decisionCanvas.transform.position.x >= 2000)
        {
            decisionCanvas.transform.position = defaultTransitionPosition;
            decisionCanvas.transform.localScale = defaultTransitionScale;
            decisionCanvas.enabled = false;
            currentState = State.STATE_DECISION;
        }
    }
    public void TransitionScreen()
    {
        //field defaultTransitionScale = transitionCanvas.transform.localScale;
       // field defaultTransitionPosition = transitionCanvas.transform.position;
        //black out the screen
        if (transitionCanvas.transform.localScale.y >= 0)
        {
            transitionCanvas.transform.localScale += scaleChange;
            moveCanvas = true;
            animationState = 0;
        }
        //move the screen out of the window
        if (moveCanvas)
        {
            transitionCanvas.transform.position += positionChange;
        }
        if(transitionCanvas.transform.position.x >= 2000)
        {
            transitionCanvas.transform.position = defaultTransitionPosition;
            transitionCanvas.transform.localScale = defaultTransitionScale;
            transitionCanvas.enabled = false;
            //if(inResetState)
            //{
                currentState = State.STATE_IDLE;
            //    inResetState = false;
            //}
            
        }
    }

    public void LoadRound(int n)
    {
        //page turn animation
        transitionCanvas.enabled = true;
        waiter(1);
        if (waiterBool)
        {
            TransitionScreen();
        }

        //need to reset both transition screens

        //switch to enemies[n]
        SwitchEnemies();
        //
    }

    public void SwitchEnemies()
    {
        if(currentRound == 1)
        {
            //idek if anything will be done here
        }
        if (currentRound == 2)
        {
            //set cpu2 to main ene    my  
            //set cpu1 position
            //set cpu to onlooker

            cpu1.SetActive(false);
            cpu2.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 3)
        {
            cpu2.SetActive(false);
            cpu3.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 4)
        {
            cpu3.SetActive(false);
            cpu4.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 5)
        {
            cpu4.SetActive(false);
            cpu5.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 6)
        {
            cpu5.SetActive(false);
            cpu6.transform.position = enemyFightingSpot.transform.position;
        }
    }
}


