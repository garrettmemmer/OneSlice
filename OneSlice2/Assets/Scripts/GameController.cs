using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //this is all the enemies we will fight. the ones not currently fighting are actually hidden
    public GameObject[] Enemies;

    //this is all the enemies that are waiting to fight
    public GameObject[] EnemiesIdle;
    
    //not used
    //public GameObject[] EnemyPool;
    //public GameObject[] EnemyIdlePool;

    //idk about these two
    public Renderer enemy;
    public Material[] materials;

    //not used
    //public GameObject enemyFightingSpot;


    //for some reason we reference the main gamecontroller again
    public GameController gc;

    //this holds the camera with the camera shake on it
    public CameraShake cameraShake; 

    //this holds the player because they have the trauma inducer script
    public TraumaInducer tramaInducer;

    //current state of the game
    //rename to gameState?
    public State currentState;

    //variables for round and floor
    public int currentRound = 1;
    public int currentFloor = 1;


    //UI Elements, jeez theres alot
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI floorCounterTxt;
    public Canvas newDecisionCanvas;
    public Animator CanvasAnimatior;
    public Canvas readyCanvas;
    public Animator ReactionCanvasAnimatior;
    public Canvas reactionCanvas;
    public Image reactionImage;
    public Canvas decisionCanvas;
    public Canvas nextRoundCanvas;
    public Canvas transitionCanvas;
    //public Text reactionText; //not used yet


    public int animationState = 0;
    public int animationStateCPU = 0;
    public bool bothReady = false;
    public bool playerInput;
    public bool p1Ready = false;
    public bool playerWin = false;
    public bool cpuWin = false;
    private bool waiterBool = false;
    public bool waiter2Bool = false;
    private bool inResetState = false;



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
        STATE_RESET,    //6
        STATE_NEWROOM
    }

    void Start()
    {
        defaultTransitionScale = transitionCanvas.transform.localScale;
        defaultTransitionPosition = transitionCanvas.transform.position;
        decisionCanvas.enabled = false;
        nextRoundCanvas.enabled = false;
        transitionCanvas.enabled = false;
        scaleChange = new Vector3(0f, -0.005f, 0f);
        positionChange = new Vector3(3f, 0f, 0.0f);
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
                CanvasAnimatior.SetInteger("transitionNumber", 0);
                animationState = 1;
                animationStateCPU = 1;
                StanceLogic();
                break;

            case State.STATE_GO: //3
                //reactionCanvas.enabled = true;
                ReactionCanvasAnimatior.SetInteger("transitionNumber", 0);
                //reactionImage.enabled = false;
                decisionCanvas.enabled = true;
                GoLogic();
                break;

            case State.STATE_TRICK: //3.1
                print("entered TRICK STATE, waiting for input");
                break;
            case State.STATE_FALSEALARM: //3.2
                print("entered FALSE ALARM");
                break;

            case State.STATE_ATTACK: //4
               // reactionCanvas.enabled = false;
                //reactionImage.enabled = false;
                ReactionCanvasAnimatior.SetInteger("transitionNumber", 1);
                decisionCanvas.enabled = true;
                //adding screenshake here
                StartCoroutine(cameraShake.Shake(.001f, .2f)); 
                //this was clunckuy with my own code
                
                //lets try with the assest store :)
                StartCoroutine(tramaInducer.StartShaking());
                //transitionCanvas.enabled = true;
                AttackLogic();
                break;

            case State.STATE_DECISION: //5
                playerInput = false;
                CanvasAnimatior.SetInteger("transitionNumber", 0);
                DecisionLogic();
                break;

            case State.STATE_RESET: //6
                playerInput = false;
                nextRoundCanvas.enabled = false;
                //decisionCanvas.enabled = false;
                //inResetState = true;
                timerValue = 0.0f;
                timerText.text = "0.0";//timerValue.ToString();
                LoadRound(currentRound);
                break;

            case State.STATE_NEWROOM: //7
                //new room transition
                CanvasAnimatior.SetInteger("transitionNumber", 0);

                //new room sound

                //deactivate old enemy

                //spawn 2 new watchers

                //spawn new fighting enemy

                //set state to idle?
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

        FindObjectOfType<AudioManager>().Play("Slice");

        CanvasAnimatior.SetInteger("transitionNumber", 1); 

        if (playerWin)
        {
            animationState = 2;
            animationStateCPU = 3;
            //cpuAnimationState = 2.5;

            //another sound here, sheathing sword and mayybe enemy death
            //FindObjectOfType<AudioManager>().Play("SheathSword");
            //FindObjectOfType<AudioManager>().Play("EnemyDeath");

            StartCoroutine(Waiter(2));
            if (waiterBool)
            {
                waiterBool = false;
                currentState = State.STATE_DECISION;
                //waiterBool = false;
            }
        }
        if (cpuWin)
        {
            animationState = 4;
            animationStateCPU = 2;
            //cpuAnimationState = ???; //winning slash
            StartCoroutine(Waiter(4));
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
            animationStateCPU = 4;
        }

        if(playerInput)
        {
            currentRound += 1;
            currentState = State.STATE_RESET;


            print("next round would start here");
            //last round logic
            if(currentRound > 3) // last round
            {
                currentRound = 0;
                currentFloor += 1;
                //string currentFloorTxtHolder = "FLOOR: ";
               // floorCounterTxt.txt = (currentFloorTxtHolder + currentFloor).ToString;
                floorCounterTxt.SetText("Floor: " + currentFloor);//) = ("Floor: " + currentFloor).ToString;
                currentState = State.STATE_RESET;
                //currentState = State.STATE_NEWFLOOR;
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
            timerText.text = timerDisplay;
            //txt.text = "wtf";
        }
    }

    IEnumerator Waiter(int n)
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
            animationStateCPU = 0;
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
            currentState = State.STATE_IDLE;
        }
    }

    public void LoadRound(int n)
    {
        //page turn animation
        //transitionCanvas.enabled = true;
        Waiter(1);
        if (waiterBool)
        {
            TransitionScreen();
            //HERE to play UI animation
            CanvasAnimatior.SetInteger("transitionNumber", 2);
            //waiter(2);
            //currentState = State.STATE_IDLE;
        }
        //need to reset both transition screens
        //switch to enemies[n]

        SwitchEnemies();
        //CanvasAnimatior.SetInteger("transitionNumber", 0);
    }

    public void SwitchEnemies()
    {
        if(currentRound == 1)
        {
            //idek if anything will be done here  
        }
        if (currentRound == 2)
        {
            Enemies[0].SetActive(false);
            Enemies[1].SetActive(true);
            EnemiesIdle[0].SetActive(false);
            //EnemiesIdle[0].transform.position = enemyFightingSpot.transform.position;

            //cpu1.SetActive(false);
            //cpu2.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 3)
        {
            Enemies[1].SetActive(false);
            Enemies[2].SetActive(true);
            EnemiesIdle[1].SetActive(false);
            //EnemiesIdle[1].transform.position = enemyFightingSpot.transform.position;
            //cpu2.SetActive(false);
            //cpu3.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 4)
        {
            Enemies[2].SetActive(false);
            Enemies[3].SetActive(true);
            EnemiesIdle[2].SetActive(false);
            //EnemiesIdle[2].transform.position = enemyFightingSpot.transform.position;
            //cpu3.SetActive(false);
            //cpu4.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 5)
        {
            Enemies[3].SetActive(false);
            Enemies[4].SetActive(true);
            EnemiesIdle[3].SetActive(false);
            //EnemiesIdle[3].transform.position = enemyFightingSpot.transform.position;
            //cpu4.SetActive(false);
            //cpu5.transform.position = enemyFightingSpot.transform.position;
        }
        if (currentRound == 6)
        {
            Enemies[4].SetActive(false);
            Enemies[5].SetActive(true);
            EnemiesIdle[4].SetActive(false);
            //EnemiesIdle[4].transform.position = enemyFightingSpot.transform.position;
            //cpu5.SetActive(false);
            //cpu6.transform.position = enemyFightingSpot.transform.position;
        }
    }



    public void GenerateEnemies()
    {
        if (currentRound == 1)
        {
            Enemies[0].SetActive(true);
            EnemiesIdle[0].SetActive(true);
            EnemiesIdle[1].SetActive(true);

            //var enemy1 = Enemies[0]; V 
            //var enemy1Skin = enemy1.GetChild(0);
            //var enemy1Joints = enemy1.GetChild(1);
            Enemies[0].GetComponent<MeshRenderer>().material = materials[1];

            //Enemies[0].GetComponentInChild<MeshRenderer>().material = materials[1]; //potential way for procedural colors
            //EnemiesIdle[0].GetComponent<MeshRenderer>().material = materials[1];
        }
        if (currentRound == 2)
        {
            Enemies[0].SetActive(false);
            Enemies[1].SetActive(true);
            EnemiesIdle[0].SetActive(false);
        }
        if (currentRound == 3)
        {
            Enemies[1].SetActive(false);
            Enemies[2].SetActive(true);
            EnemiesIdle[1].SetActive(false);
        }

        //for starting the next round
        //spawn 2 new watchers
        //spawn new fighting enemy
    }

    public void GenerateProceduralEnemies()
    {
        //spawn 2 new watchers
        //spawn new fighting enemy
        //generate colors for new enemies 
    }
}


