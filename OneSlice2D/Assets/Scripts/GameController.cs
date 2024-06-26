using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //this is all the enemies we will fight. the ones not currently fighting are actually hidden
    public GameObject[] Enemies;

    //this is all the enemies that are waiting to fight
    public GameObject[] EnemiesIdle;

    public State currentState;
    public bool playerInput;
    public GameObject playerUnit;
    public GameObject aiUnit;

    //public TextMeshProUGUI timerText;
    public float targetTime = 2.0f;
    public float timerValue = 0.0f;
    private string timerDisplay;

    public float reactionTimeCPU = 3.0f;
    public float reactionTimeDisplay = 0.0f;

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
    public Canvas gameEndingCanvas;
    //public Text reactionText; //not used yet

    //AI Animation States
    const string AI_Idle = "TestIdle";
    const string AI_Ready = "TestReady";
    const string AI_Slice = "TestSlice";
    const string AI_Decision = "TestFalseStart";
    const string AI_Death = "TestDefeat";
    const string AI_Victory = "TestVicotry";
    private string currentAiAnimaton;
    private Animator aiAnimator;

    //Player Animation States
    const string Player_Idle = "TestIdle";
    const string Player_Ready = "TestReady";
    const string Player_Slice = "TestSlice";
    const string Player_FalseStart = "TestFalseStart";
    const string Player_Death = "TestDefeat";
    const string Player_Victory = "TestVicotry";
    private string currentPlayerAnimaton;
    private Animator playerAnimator;


    public bool playerWin = false;
    public bool cpuWin = false;
    private bool waiterBool = false;
    public int currentRound = 1;
    public int currentFloor = 1;

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
        STATE_NEWROOM,
        STATE_VICTORY,
        STATE_DEFEAT
    }

    private void Awake()
    {
        playerUnit = GameObject.Find("Player");
        //aiUnit = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
       // defaultTransitionScale = transitionCanvas.transform.localScale;
        //defaultTransitionPosition = transitionCanvas.transform.position;
        decisionCanvas.enabled = false;
        nextRoundCanvas.enabled = false;
        transitionCanvas.enabled = false;
        gameEndingCanvas.enabled = false;
        //we are going to have to get the player object and then the animator off the player object. I think we can do that here and in the awake function
        playerAnimator = playerUnit.GetComponent<Animator>();   
        aiAnimator= playerUnit.GetComponent<Animator>();
        //scaleChange = new Vector3(0f, -0.005f, 0f);
        //positionChange = new Vector3(3f, 0f, 0.0f);
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
                ChangePlayerAnimationState(Player_Idle);
               // ChangeAIAnimationState();
                InputListener();
                IdleLogic();
                break;

            case State.STATE_STANCE: //2
                CanvasAnimatior.SetInteger("transitionNumber", 0);
                ChangePlayerAnimationState(Player_Ready);
                //animationState = 1;
                ///animationStateCPU = 1;
                StanceLogic();
                break;

            case State.STATE_GO: //3
                ReactionCanvasAnimatior.SetInteger("transitionNumber", 0);
                decisionCanvas.enabled = true;
                //an animation of powering up //ChangePlayerAnimationState(Player_ready);
                GoLogic();
                break;

            case State.STATE_TRICK: //3.1
                print("entered TRICK STATE, waiting for input");
                break;
            case State.STATE_FALSEALARM: //3.2
                print("entered FALSE ALARM");
                print("right now, nothing happens, soon we will send to defeat state");
                //FalseAlarmLogic();
                ChangePlayerAnimationState(Player_FalseStart);
          
                currentState = State.STATE_DEFEAT;
                break;

            case State.STATE_ATTACK: //4
                                
                ReactionCanvasAnimatior.SetInteger("transitionNumber", 1);
                decisionCanvas.enabled = true;

                //update the position
                //playerUnit.transform.parent = transform.postSlicePosition;

                //slice animation
                ChangePlayerAnimationState(Player_Slice);
                //adding screenshake here
                //StartCoroutine(cameraShake.Shake(.001f, .2f));
                //this was clunckuy with my own code

                //lets try with the assest store :)
                // StartCoroutine(tramaInducer.StartShaking());

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

            case State.STATE_VICTORY: //7
                /*
                * wtf do we need in a win state?
                * a transition screen
                * exagerated final round 
                * new panel with victory message + restart or quit button 
                   * victory animations
                 * 
                */
                ChangePlayerAnimationState(Player_Victory); 
                gameEndingCanvas.enabled = true;
                gameEndingCanvas.transform.GetChild(1).transform.gameObject.SetActive(true);
                break;


            case State.STATE_DEFEAT: //7
                /*
               * wtf do we need in a defeat state?
               * a transition screen
               * exagerated final round 
               * new panel with victory message + restart or quit button 
                  * victory animations
                * 
               */
                ChangePlayerAnimationState(Player_Death);
                gameEndingCanvas.enabled = true;
                gameEndingCanvas.transform.GetChild(0).transform.gameObject.SetActive(true);
                break;

            default:
                print("fuck :(");
                break;
        }
    }



    void ChangePlayerAnimationState(string newAnimation)
    {
        if (currentPlayerAnimaton == newAnimation) return;
        playerAnimator.Play(newAnimation);
        currentPlayerAnimaton = newAnimation;
    }

    void ChangeAIAnimationState(string newAnimation)
    {
        if (currentAiAnimaton == newAnimation) return;
        aiAnimator.Play(newAnimation);
        currentAiAnimaton = newAnimation;
    }

    public void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("space pressed");
            playerInput = true;
            //gc.bothReady = true;
            //ai.bothReady = true;
            //ChangePlayerAnimationState(Player_ready);
        }
        //playerInput = false;
        //ChangePlayerAnimationState(Player_idle);
    }

    public void TimerFunction()
    {
        timerValue += Time.deltaTime;
        //timerValue - targetTime = timer
        timerDisplay = (timerValue - targetTime).ToString();
        if ((timerValue - targetTime) >= 0f)
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

    public void IdleLogic()
    {
        if (playerInput)// p1.playerReady) //(bothReady)
        {
            //sound logic //gong sound
            //we could also just create a reference variable up top
            //that we would initilaize in the inspector
            //FindObjectOfType<AudioManager>().Play("Gong");
            //print("switching to STANCE STATE");
            currentState = State.STATE_STANCE;
            readyCanvas.enabled = false;
        }
    }

    public void StanceLogic()
    {
        playerInput = false;
        //animationState = 2;
        TimerFunction();
        InputListener();
        //IntenseMomentFunction(); //things like zoom, audio, slight screenshake
        if (playerInput && timerValue < targetTime)
        {
            playerInput = false;
            //animationstate = stumble
            //sound = whoa
            currentState = State.STATE_FALSEALARM;
            print("someone swung too early, soon we will develop a way to punish this");
        }
        if (timerValue >= targetTime)
        {
            //FindObjectOfType<AudioManager>().Play("StickBreak");
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

        //FindObjectOfType<AudioManager>().Play("Slice");

        CanvasAnimatior.SetInteger("transitionNumber", 1);

        if (playerWin)
        {
            //animationState = 3;
            //animationStateCPU = 3;
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
            //animationState = 3;
            //animationStateCPU = 2;
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
        //FindObjectOfType<AudioManager>().Play("PageTurn");
        nextRoundCanvas.enabled = true;
        if (playerWin)
        {
            //animationState = 5;
            //animationStateCPU = 4;
        }

        if (playerWin && currentRound == 6)
        {
            //animationState = 5;
            //animationStateCPU = 4;
            print("the script does make live changes ");
        }

        if (playerInput && currentRound < 6)
        {
            currentRound += 1;
            currentState = State.STATE_RESET;
    

            print("next round would start here");
            //last round logic
            
            if (currentRound == 6) // last round
            {
                /*
                //this is code that will progress to another floor, like if this was a tower battle or something. 
                currentRound = 0;
                currentFloor += 1;
                //string currentFloorTxtHolder = "FLOOR: ";
                floorCounterTxt.SetText("Floor: " + currentFloor);//) = ("Floor: " + currentFloor).ToString;
                currentState = State.STATE_RESET;
                //currentState = State.STATE_NEWFLOOR;
            */
                //this is code to display the Win State
                print("the final round is starting. ");
            }
            if (currentRound > 6) // last round
            {
                /*
                //this is code that will progress to another floor, like if this was a tower battle or something. 
                currentRound = 0;
                currentFloor += 1;
                //string currentFloorTxtHolder = "FLOOR: ";
                floorCounterTxt.SetText("Floor: " + currentFloor);//) = ("Floor: " + currentFloor).ToString;
                currentState = State.STATE_RESET;
                //currentState = State.STATE_NEWFLOOR;
            */

                //this is code to display the Win State
                print("the final round is over. ");
                currentState = State.STATE_VICTORY; //this will be the gameover screen
            }

        }
    }

    /*
     * wtf do we need in a win state?
     * a transition screen
     * exagerated final round 
     * new panel with victory message + restart or quit button 
     * victory animations
     * 
     */

    public void LoadResetGame()
    {
        //fader.gameObject.SetActive(true);
        //LeanTween.scale(fader, Vector3.zero, 0f);
        //LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
       // {
            //FindObjectOfType<AudioManager>().Play("ButtonClick");
            SceneManager.LoadScene("GameScene"); //test scene
            //Invoke("LoadGame", 0.5f);
       // });
    }

    public void QuitGame()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonClick");
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonClick");
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// this is not currently being used
    /// </summary>
    public void SliceScreen()
    {
        /*
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
        */
    }

    /// <summary>
    /// this may be necessary if we are going to stick with UI cut screens
    /// </summary>
    public void TransitionScreen()
    {
        /*
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
        if (transitionCanvas.transform.position.x >= 2000)
        {
            transitionCanvas.transform.position = defaultTransitionPosition;
            transitionCanvas.transform.localScale = defaultTransitionScale;
            transitionCanvas.enabled = false;
            currentState = State.STATE_IDLE;
        }
        */
    }

    public void LoadRound(int n)
    {
        //page turn animation
        //transitionCanvas.enabled = true;
        Waiter(1);
        if (waiterBool)
        {
            //built in function to move the screen
            //TransitionScreen();
            //HERE to play UI animation
            CanvasAnimatior.SetInteger("transitionNumber", 2);
            //waiter(2);
            //currentState = State.STATE_IDLE;
        }
        //need to reset both transition screens
        //switch to enemies[n]

        SwitchEnemies();
        //CanvasAnimatior.SetInteger("transitionNumber", 0);

        currentState = State.STATE_IDLE;
    }

    public void SwitchEnemies()
    {
        if (currentRound == 1)
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

    //code for generating enemies (not being used)
    /*
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
    */
}
