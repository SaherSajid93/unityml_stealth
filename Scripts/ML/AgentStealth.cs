using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;
using UnityEngine.AI;

//Define actions the agent can take
// Reward function for the agent
public class AgentStealth : Agent
{
    Rigidbody agentRb;
    private RayPerception rayPer;
    private AcademyStealth envAcademy;

    //Ethan's scripts
    //PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    PlayerInventory playerInventory;

    LastPlayerSighting lastPlayerSighting;

    LevelGeneration levelGeneration;

    //get the game objexts for these 4 scripts
    public GameObject[] switchUnits;    // for laserSwitchDeactivation
    public GameObject[] enemies;        // for enemyShooting
    public GameObject doorExit;         // for lift trigger
    public GameObject keyCard;          // for key pick up

    //Copied from player movement:
    public AudioClip shoutingClip;      // Audio clip of the player shouting.
    public float turnSmoothing = 15f;   // A smoothing value for turning the player.
    public float speedDampTime = 0.1f;  // The damping for the speed parameter


    private Animator anim;              // Reference to the animator component.
    private HashIDs hash;           // Reference to the HashIDs.

    public bool shout;
    public float speed;

    int maxStep =  1000; // just setting a divider for the rewards.

    private UnityEngine.AI.NavMeshAgent agentNav;                               // Reference to the nav mesh agent.
    
    // NAVIGATION / DIRECTION Goals
//    public UnityEngine.AI.NavMeshSurface meshSurface;
    public Transform target;
    private NavMeshPath oldPath;
    private NavMeshPath newPath;
    public GameObject pathObject;
    public float newpathLength;
    public float oldpathLength;

    bool isNewDecisionStep;
    int currentDecisionStep;

    void Awake()
    {
        levelGeneration = this.GetComponent<LevelGeneration>();
       // meshSurface = this.GetComponent<NavMeshSurface>();

        // Setting up the references.
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();

        // Set the weight of the shouting layer to 1.
        anim.SetLayerWeight(1, 1f);
        doorExit = GameObject.Find("door_exit_outer");

        //
        pathObject.GetComponent<meshBuilder>().enabled = true;


    }

    private void Update()
    {

    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();


       // playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerInventory = GetComponent<PlayerInventory>();

        agentRb = GetComponent<Rigidbody>();
        rayPer = GetComponent<RayPerception>();
        envAcademy = FindObjectOfType<AcademyStealth>();

        // used this in laser switch deactivation, enemy shooting, lift trigger, key pick up

        foreach (GameObject switchunit in switchUnits)
            switchunit.GetComponent<LaserSwitchDeactivation>().agentStealth = this;

        foreach (GameObject enemy in enemies)
            enemy.GetComponent<EnemyShooting>().agentStealth = this;
        doorExit.GetComponent<DoorAnimation>().agentStealth = this;
        keyCard.GetComponent<KeyPickup>().agentStealth = this;
        
        playerHealth.agentStealth = this;

        agentNav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Navigation / Path stuff
        //        meshSurface.BuildNavMesh();
        oldPath = new NavMeshPath();
        newPath = new NavMeshPath();
        target = keyCard.transform;
        if (GetPath(newPath, transform.position, target.position, NavMesh.AllAreas))
            newpathLength = GetPathLength(newPath);
        oldpathLength = newpathLength;


    }

    void Start()
    {

    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(12.635f, -4.785f, -22.82f);
        pathObject.GetComponent<meshBuilder>().enabled = false;
        
        playerHealth.playerDead = false;
        this.transform.position = new Vector3(-2.5f, 0, 0);
        playerInventory.hasKey = false;
        playerHealth.health = 100;

        //resetting enemies

        enemies[0].transform.position = new Vector3(-18f, 0, 6.5f);
        enemies[0].transform.rotation = new Quaternion(0,0,0,0); 
        enemies[1].transform.position = new Vector3(-19f, 0, 37);
        enemies[1].transform.rotation = new Quaternion(0, 0, 0, 0);
        enemies[2].transform.position = new Vector3(-29f, 0, 43.5f);
        enemies[2].transform.rotation = new Quaternion(0, 0, 0, 0);

        foreach (GameObject enemy in enemies)
        {
            if(enemy.activeSelf)
                enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;

        }

        //reset the keycard so it is enabled
        keyCard.SetActive(true);

		
		// TODO: Take this out
        //reset the switches so the lasers are all enabled
        foreach (GameObject switchunit in switchUnits)
            switchunit.GetComponent<LaserSwitchDeactivation>().laser.SetActive(true);

        //turn the alarms off
        lastPlayerSighting.position = lastPlayerSighting.resetPosition;

        levelGeneration.DisableAll(); // Disable all active GOs
        levelGeneration.generateLevel((int)envAcademy.resetParameters["difficulty"]); // Enable n
        // refer to academy.difficulty

        // Navigation / Path stuff
//        meshSurface.BuildNavMesh();
        oldPath = new NavMeshPath();
        newPath = new NavMeshPath();
        target = keyCard.transform;

        if (GetPath(newPath, transform.position, target.position, NavMesh.AllAreas))
            newpathLength = GetPathLength(newPath);
        oldpathLength = newpathLength;

        pathObject.GetComponent<meshBuilder>().enabled = true;

        isNewDecisionStep = true;
        currentDecisionStep = 1;

        //envAcademy.AcademyReset();
    }

    /// <summary>
    /// How the agent learns about the world
    /// </summary>
    public override void CollectObservations()
    {
        //ray perception observations:
        var rayDistance = 40f;
        float[] rayAngles = { 0f, 15f, 30f, 45f, 60f, 75, 90f, 105f, 120f, 135f, 150f, 165f, 180f};
        var detectableObjects = new[] { "Enemy", "laserfence", "exitlift", "door", "switch", "key", "wall"};
        //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1f, 0f));


        //Ethan's position/rotation observations
        AddVectorObs(this.transform.position); // Ethan's position
        // And rotation
        AddVectorObs(gameObject.transform.rotation.z);
        AddVectorObs(gameObject.transform.rotation.x);

        oldpathLength = newpathLength;
        if (GetPath(newPath, transform.position, target.position, NavMesh.AllAreas))
            newpathLength = GetPathLength(newPath);
        else newpathLength = 1000;

        AddVectorObs(newpathLength);

    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
	public override void AgentAction(float[] vectorAction, string textAction)
    {
        //x,y, sneak and shout
        bool sneak;
        bool shout;
        bool switchdeactivate;
        float horizontal;
        float vertical;

        horizontal = vectorAction[0];
        vertical = vectorAction[1];
        sneak = (vectorAction[2]>0);
        shout = (vectorAction[3]>0);
        switchdeactivate = (vectorAction[4] > 0);


        MovementManagement(horizontal, vertical, sneak);
        // Set the animator shouting parameter.
        anim.SetBool(hash.shoutingBool, shout);
        AudioManagement(shout);

        // Move rewards
        if(isNewDecisionStep)
            IMovedReward();

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / maxStep);

        IncrementDecisionTimer();

    }

    /// <summary>
    /// Encourage moving towards target
    /// </summary>
    /// 
    public void IMovedReward()
    {


        //Debug.Log(("OP: " + oldpathLength + "NP: " + newpathLength).ToString());


        if (newpathLength < oldpathLength)
        {
            AddReward(1f / maxStep);
        }
        else
            AddReward(-1f / maxStep);
    }


    /// <summary>
    /// Called when the agent reaches the lift successfully
    /// </summary>
    /// 
    public void IEnteredLift()
        {
            // If the doors open
            // instead of the player entering 
            // We use a reward of 10.
            AddReward(1000f);

            // By marking an agent as done AgentReset() will be called automatically.
            Done();
        }

        /// <summary>
        /// Called when the agent collects Key Card
        /// </summary>
        public void ICollectedKey()
        {
            // We use a reward of 5.
            AddReward(500f);
        // Just this test case
        // To Be Removed Later
        // By marking an agent as done AgentReset() will be called automatically.
       // Debug.Log(Time.time);
        Done();
        // for more complex cases, instead of done being called, the next line should be uncommented
        // target = doorExit.transform;
    }

        /// <summary>
        /// Called when the agent disables lasers
        /// </summary>
        public void IDisabledLaser()
        {
            // We use a reward of 2.5.
            AddReward(100f);

        }

        /// <summary>
        /// Called when the agent is shot
        /// </summary>
        public void IGotShot()
        {
            // We use a reward of -5.
            AddReward(-50f);
        }

        public void IDied()
        {
            Debug.Log("Dead");
            // Adding a negative reward here.. and calling done
            AddReward(-1000f);           //-10 for dying

            Done();
        }

        public void ITriggeredAlarm()
        {
            AddReward(-50f);
        }


        // functions from playermovement:

    public void MovementManagement(float horizontal, float vertical, bool sneaking)
        {
            // Set the sneaking parameter to the sneak input.
            anim.SetBool(hash.sneakingBool, sneaking);

            // If there is some axis input...
            if (horizontal != 0f || vertical != 0f)
            {
                // ... set the players rotation and set the speed parameter to 5.5f.
                Rotating(horizontal, vertical);
                anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
            }
            else
                // Otherwise set the speed parameter to 0.
                anim.SetFloat(hash.speedFloat, 0);

        }


        void Rotating(float horizontal, float vertical)
        {
            // Create a new vector of the horizontal and vertical inputs.
            Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

            // Create a rotation based on this new vector assuming that up is the global y axis.
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // Create a rotation that is an increment closer to the target rotation from the player's rotation.
            Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);

            // Change the players rotation to this new rotation.
            GetComponent<Rigidbody>().MoveRotation(newRotation);
        }


        void AudioManagement(bool shout)
        {
            // If the player is currently in the run state...
            if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hash.locomotionState)
            
            {
                // ... and if the footsteps are not playing...
                if (!GetComponent<AudioSource>().isPlaying)
                    // ... play them.
                    GetComponent<AudioSource>().Play();
            }
            else
                // Otherwise stop the footsteps.
                GetComponent<AudioSource>().Stop();

            // If the shout input has been pressed...
            if (shout)
                // ... play the shouting clip where we are.
                AudioSource.PlayClipAtPoint(shoutingClip, transform.position);
        }


    //**************************************************************************
    //**************************************************************************


    /// <summary>
    /// Reward for previous/current distance based on decision frequency.
    /// </summary>
    public void IncrementDecisionTimer()
    {
        if (currentDecisionStep == agentParameters.numberOfActionsBetweenDecisions ||
            agentParameters.numberOfActionsBetweenDecisions == 1)
        {
            currentDecisionStep = 1;
            isNewDecisionStep = true;
        }
        else
        {
            currentDecisionStep++;
            isNewDecisionStep = false;
        }
    }


    public static bool GetPath(NavMeshPath path, Vector3 fromPos, Vector3 toPos, int passableMask)
    {
        path.ClearCorners();

        if (NavMesh.CalculatePath(fromPos, toPos, passableMask, path) == false)
            return false;

        return true;
    }

    public static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }
}
