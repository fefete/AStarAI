using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIUnit : MonoBehaviour
{

    //Team Enume
    public enum Team
    {
        Red, Blue
    }

    // Enum States
    public enum State
    {
        //
        // Capture - Go for enemy Flag
        // Defend - defend team Flag
        //
        Capture, Defend
    }

    // Private Varibles
    private string teamTag_;
    private bool teamFlagCaptured_;
    private bool enemeyFlagCaptured_;
    private float moveSpeed;
    private int IDXcounter_;
    public Team team_;

    //public Vector3[] path_;
    public List<Vector3> path_;
    public List<Node> Npath_;

    //Public Variables
    public WorldManager worldManager_;
    public TeamManager teamManager;
    public AStarManager AStarManager_;
    public GameObject recoveryBay;
    private CaptureState captureState_;
    private DefendState defendState_;
    public AISight aiSight_;
    public State state;

    public float health;
    public bool isAlive;
    public bool didRespawn;
    Vector3 respawnPos;
    public float maxChaseDistance;

    // Shooting Variables
    public GameObject enemyUnitTarget_;
    public int shootDamage = 10;
    public float fireRate = 1.0f;
    public float shootRange = 50f;
    public Transform pointOfFire;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.5f);
    private LineRenderer bulletLine;
    private float nextFire;

    public Node startNode;
    public Node targetNode;
    public Node deliveryNode;

    public bool hasFlag;

    public bool isCapturing;
    public bool isFighting;
    public bool isFleeing;
    public bool isDelivering;

    private Renderer rend;

    public Text MainStateTXT;
    public Text SubStateTXT;

    // Use this for initialization
    void Start()
    {
        // Suppose this could be set with a public enum variable 
        teamTag_ = gameObject.tag;

        // Gain Access to the world manager script
        worldManager_ = worldManager_.GetComponent<WorldManager>();

        teamFlagCaptured_ = false;
        enemeyFlagCaptured_ = false;

        rend = GetComponent<Renderer>();

        if (teamTag_ == "BlueTeam")
        {
            team_ = Team.Blue;
            rend.material.color = Color.blue;
        }
        else
        {
            team_ = Team.Red;
            rend.material.color = Color.red;
        }

        moveSpeed = 7.5f;
        isAlive = true;
        didRespawn = false;
        respawnPos = gameObject.transform.position;
        isCapturing = false;
        enemyUnitTarget_ = null;

        bulletLine = GetComponent<LineRenderer>();

        captureState_ = this.GetComponent<CaptureState>();
        defendState_ = this.GetComponent<DefendState>();

        captureState_.enabled = false;
        defendState_.enabled = false;

        startNode = getClosestNode();

        IDXcounter_ = 0;
        hasFlag = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive == false)
        {
            aiSight_.enabled = false;
            captureState_.enabled = false;
            defendState_.enabled = false;
            GetComponent<AIUnit>().enabled = false;
        }
        else
        {
            switch (state)
            {
                case State.Capture:
                    defendState_.enabled = false;
                    captureState_.enabled = true;
                    MainStateTXT.text = "Capture";
                    SubStateTXT.text = GetComponent<CaptureState>().subState.ToString();
                    break;

                case State.Defend:
                    captureState_.enabled = false;
                    defendState_.enabled = true;
                    MainStateTXT.text = "Defend";
                    SubStateTXT.text = GetComponent<DefendState>().subState.ToString();
                    break;
            }

        }
    }

    // Returns a new A* Calculated path.
    public void getNewPath(Node startNode, Node endNode)
    {
        path_.Clear();
        Npath_.Clear();
        AStarManager_.calculatePath(startNode, endNode, out path_, out Npath_);
    }

    //Returns the closest node to the current position of the unit
    public Node getClosestNode()
    {
        return AStarManager_.getNearestNode(this.transform);
    }

    public Node getClosestNodeToPosition(Transform pos)
    {
        return AStarManager_.getNearestNode(pos);
    }

    // Advance the unit from one node to the next
    public void moveAlongPath()
    {
        if (isAlive)
        {
            int maxIDX = path_.Count;

            if (IDXcounter_ >= maxIDX)
            {
                IDXcounter_ = 0;
            }

            Vector3 targetNode = path_[IDXcounter_];

            float distToNode = Vector3.Distance(transform.position, targetNode);

            if (IDXcounter_ != path_.Count - 1 && distToNode < 1.0f)
            {
                IDXcounter_++;
            }

            transform.LookAt(targetNode);
            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetNode, step);
        }
    }

    public IEnumerator flee(Transform pos)
    {
        if (!isFleeing)
        {
            isFleeing = true;
            Debug.Log(this.gameObject.name + " is running away from a fight");
            path_.Clear();

            Node close = getClosestNode();
            Node far = getClosestNodeToPosition(pos);

            AStarManager_.calculatePath(close, far, out path_, out Npath_);

            IDXcounter_ = 0;

            if (path_.Count != 0)
            {
                moveAlongPath();

            }

            yield return null;
        }
    }

    // Deliver Flag State Enumerator/Update Method
    public IEnumerator deliverFlag()
    {
        if (isDelivering)
        {
            IDXcounter_ = 0;
            yield break;
        }
        else
        {
            isDelivering = true;
            IDXcounter_ = 0;
            while (true)
            {

                if (path_.Count != 0)
                {
                    moveAlongPath();
                }
                yield return null;
            }
        }
    }

    public IEnumerator chase(GameObject enemyUnit)
    {
        bool canChase = true;
        while (canChase == true)
        {
            float dist = Vector3.Distance(enemyUnit.transform.position, transform.position);

            //Stop moving/chasing when close enough to the enemy 
            if (dist > 5.0f)
            {
                Vector3 rayDir = transform.position - enemyUnit.transform.position;
                RaycastHit rayHit;
                if (Physics.Raycast(pointOfFire.position, rayDir, out rayHit))
                {
                    if (rayHit.collider.tag != enemyUnit.tag)
                    {
                        getNewPath(getClosestNode(), getClosestNodeToPosition(enemyUnit.transform));
                        IDXcounter_ = 0;
                        moveAlongPath();

                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, enemyUnit.transform.position, moveSpeed * Time.deltaTime);
                    }
                }


            }

            yield return null;
        }
    }

    // Patrol State Enumerator/Update Method
    // Make a Unit patrol randomly between givin points/nodes
    public IEnumerator patrol(List<Node> patrolNodes)
    {

        Debug.Log(gameObject.name + " is Patrolling");
        int randNum = 0;
        int lastNumberPicked = 0;

        IDXcounter_ = 0;

        Node currentNode = patrolNodes[0];
        Node targetNode;
        GameObject targetNodeObj;
        randNum = Random.Range(0, patrolNodes.Count);

        while (true)
        {
            checkForEnemies();
            if (randNum != lastNumberPicked)
            {
                targetNode = patrolNodes[randNum];

                AStarManager_.calculatePath(currentNode, targetNode, out path_, out Npath_);
                if (path_.Count > 0)
                {
                    moveAlongPath();

                    targetNodeObj = GameObject.Find(targetNode.name);
                    transform.LookAt(targetNodeObj.transform);

                    float distToNode = Vector3.Distance(transform.position, targetNodeObj.transform.position);

                    if (distToNode < 1.0f)
                    {
                        transform.position = transform.position;
                        yield return new WaitForSeconds(4.0f);
                        currentNode = targetNode;
                        lastNumberPicked = randNum;
                        randNum = Random.Range(0, patrolNodes.Count);
                    }
                }
            }
            else
            {
                randNum = Random.Range(0, patrolNodes.Count);
            }

            yield return null;
        }
    }

    // Capturing State Enumerator/ Update Method
    // Make the unit advance along a path toward the enemy flag
    public IEnumerator capturing()
    {
        if (isCapturing || hasFlag)
        {
            yield break;
        }
        else
        {
            IDXcounter_ = 0;
            while (true)
            {
                if (path_.Count != 0)
                {
                    moveAlongPath();
                }

                checkForEnemies();

                yield return null;
            }
        }
    }

    public IEnumerator fight()
    {
        if (isFighting || hasFlag)
        {
            yield break;
        }
        else
        {
            while (true)
            {
                //if (!hasFlag)
                //{
                    if (enemyUnitTarget_ != null)
                    {
                        //Debug.Log("Enemy Target: " + enemyUnitTarget_.name);
                        shoot(enemyUnitTarget_);

                        if (enemyUnitTarget_.GetComponent<AIUnit>().isAlive == false)
                        {
                            if (state == State.Capture)
                            {
                                //isFighting = true;
                                GetComponent<CaptureState>().subState = CaptureState.SubState.Empty;
                            }
                            else if (state == State.Defend)
                            {
                                //isFighting = true;
                                GetComponent<DefendState>().subState = DefendState.SubState.Empty;
                            }
                        }

                        if (health <= 40)
                        {
                            StartCoroutine(flee(recoveryBay.transform));
                        }

                    }
                    else
                    {
                        if (state == State.Capture)
                        {
                            //isFighting = true;
                            GetComponent<CaptureState>().subState = CaptureState.SubState.Empty;
                        }
                        else if (state == State.Defend)
                        {
                            //isFighting = true;
                            GetComponent<DefendState>().subState = DefendState.SubState.Empty;
                        }
                    }
                //}
                //else
                //{
                //    yield break;
                //}

                yield return null;
            }
        }
    }

    public void checkForEnemies()
    {
        if (aiSight_.hasTarget)
        {
            enemyUnitTarget_ = aiSight_.GetTarget();

            float distToTarget = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

            if (distToTarget < shootRange)
            {
                enemyUnitTarget_ = aiSight_.GetTarget();
                if (state == State.Capture)
                {
                    GetComponent<CaptureState>().subState = CaptureState.SubState.Fighting;
                }
                else if (state == State.Defend)
                {
                    GetComponent<DefendState>().subState = DefendState.SubState.Fighting;
                }
                else
                {
                    isFighting = false;
                }
            }
        }
        else
        {
            enemyUnitTarget_ = null;
        }
    }

    public void shoot(GameObject target)
    {
        if (target.GetComponent<AIUnit>().isAlive)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                StartCoroutine(bulletEffect());

                Vector3 rayOrigin = pointOfFire.position;
                Vector3 rayDir = target.transform.position - transform.position;

                RaycastHit hit;

                bulletLine.SetPosition(0, pointOfFire.position);

                if (Physics.Raycast(rayOrigin, rayDir, out hit, shootRange))
                {
                    bulletLine.SetPosition(1, hit.point);

                    AIUnit enemyHit = hit.collider.GetComponent<AIUnit>();

                    if (enemyHit != null)
                    {
                        enemyHit.Damage(shootDamage);
                    }
                }
                else
                {
                    bulletLine.SetPosition(1, pointOfFire.position + (rayDir * shootRange));
                }

            }
        }
    }

    private IEnumerator bulletEffect()
    {
        bulletLine.enabled = true;
        yield return shotDuration;
        bulletLine.enabled = false;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            //Call Respawn
            Debug.Log("Unit: " + this.gameObject.name + " is dead");
            isAlive = false;
            rend.enabled = false;
            IDXcounter_ = 0;
            isCapturing = false;
            isFighting = false;
            isDelivering = false;
            isFleeing = false;
            hasFlag = false;

            if (state == State.Capture)
            {
                //isFighting = true;
                GetComponent<CaptureState>().subState = CaptureState.SubState.Empty;
            }
            else if (state == State.Defend)
            {
                //isFighting = true;
                GetComponent<DefendState>().subState = DefendState.SubState.Empty;
            }

            enemyUnitTarget_ = null;
            StartCoroutine(respawn());
        }
    }

    public IEnumerator respawn()
    {
        yield return new WaitForSeconds(4.0f);
        transform.position = respawnPos;
        health = 100;
        isAlive = true;
        didRespawn = true;
        rend.enabled = true;

        aiSight_.enabled = true;
        GetComponent<AIUnit>().enabled = true;

        if (state == State.Capture)
        {
            captureState_.enabled = true;
        }
        else
        {
            defendState_.enabled = true;
        }

    }

    private int genRandNumber(int min, int max)
    {
        return Random.Range(min, max);
    }

    public IEnumerator wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Wait over");
    }

    public void stopCaptureState()
    {
        StopCoroutine(capturing());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == recoveryBay.tag)
        {
            health = 100;
            if (isFleeing == true)
            {
                isFleeing = false;
            }
        }
    }

}
