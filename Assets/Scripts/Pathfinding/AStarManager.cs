using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    static AStarManager instance = null;
    private Node[] nodesInWorld;
    public short number_of_listening_threads;
    public bool ready = false;

    // Use this for initialization
    void Start()
    {
        RaycastHit hit = new RaycastHit();
        foreach (Node actual in nodesInWorld)
        {
            foreach (Node toCheck in nodesInWorld)
            {
                if (actual.gameObject == toCheck.gameObject) continue;
                if (Physics.Raycast(actual.transform.position, toCheck.transform.position - actual.transform.position, out hit))
                {
                    if (hit.transform.gameObject == toCheck.gameObject)
                    {
                        //Debug.DrawRay(actual.transform.position, toCheck.transform.position - actual.transform.position, Color.magenta, 1000000);
                        actual.accessibleNodes.Add(toCheck);
                    }
                }
            }
        }
        foreach (Node actual in nodesInWorld)
        {
            actual.GetComponent<BoxCollider>().enabled = false;
        }
        ready = true;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        nodesInWorld = FindObjectsOfType<Node>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    bool done = false;
    public void calculatePath(Node start, Node end, out List<Vector3> path, out List<Node> nodesInPath)
    {
        Debug.Log("start algorithm");
        List<NodeData> openList = new List<NodeData>();
        List<NodeData> closeList = new List<NodeData>();
        NodeData start_node = start.data;
        NodeData end_node = end.data;

        start_node.F = 0;
        start_node.G = 0;
        start_node.H = 0;

        NodeData current_node = start.data;
        // Put node_start on the OPEN list
        openList.Add(start_node);
        // while the OPEN list is not empty
        while (openList.Count > 0)
        {
            // Get the node off the OPEN list with the lowest f and call it node_current
            int index_to_remove = 0;
            current_node = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].F <= current_node.F)
                {
                    current_node = openList[i];
                    index_to_remove = i;
                }
            }
            openList.RemoveAt(index_to_remove);
            // If node_current is the same state as node_goal: break from the while loop
            if (current_node.node.transform.position == end.transform.position)
            {
                closeList.Add(current_node);
                break;
            }
            // Generate each state node_successor that can come after node_current
            List<NodeData> successors = new List<NodeData>();
            successors.Clear();
            for (int i = 0; i < current_node.node.accessibleNodes.Count; i++)
            {
                NodeData n = new NodeData();
                n.node = current_node.node.accessibleNodes[i];
                n.F = n.node.F;
                n.G = n.node.G + 1.0f;
                n.H = n.node.H;
                n.name = n.node.gameObject.name;
                //I came from this node, important for getting the path later.
                n.CameFrom = current_node;

                successors.Add(n);
            }

            // For each node_successor of node_current
            for (int i = 0; i < successors.Count; i++)
            {
                //RaycastHit hit;
                // Set the G of node_successor to be the G of node_current plus the G to get to node_successor from node_current
                successors[i].G = successors[i].G + Vector3.Distance(successors[i].node.transform.position, current_node.node.transform.position);

                // Find node_successor on the OPEN list
                bool found_in_open_list = false;
                for (int a = 0; a < openList.Count; a++)
                {
                    // If node_successor is on the OPEN list but the existing one is as good or better then discard this successor and continue with next successor
                    if (successors[i].node.transform.position == openList[a].node.transform.position)
                    {
                        if (successors[i].G >= openList[a].G)
                        {
                            found_in_open_list = true;
                        }
                        break;
                    }
                }
                if (found_in_open_list) continue;

                // If node_successor is on the CLOSED list but the existing one is as good or better then discard this successor and continue with next successor
                bool found_in_close_list = false;
                for (int a = 0; a < closeList.Count; a++)
                {
                    if (successors[i].node.transform.position == closeList[a].node.transform.position)
                    {
                        if (successors[i].G >= closeList[a].G)
                        {
                            found_in_close_list = true;
                        }
                        break;
                    }
                }
                if (found_in_close_list) continue;
                // Remove occurences of node_successor from OPEN and CLOSED
                for (int b = 0; b < openList.Count; b++)
                {
                    if (successors[i].node.transform.position == openList[b].node.transform.position)
                    {
                        openList.RemoveAt(b);
                    }
                }
                for (int b = 0; b < closeList.Count; b++)
                {
                    if (successors[i].node.transform.position == closeList[b].node.transform.position)
                    {
                        closeList.RemoveAt(b);
                    }
                }
                //CHECK THIS PART.

                // Set h to be the estimated distance to node_goal (using the H function (euclidean distance) )
                successors[i].H = Vector3.Distance(successors[i].node.transform.position, end_node.node.transform.position);
                successors[i].F = successors[i].G + successors[i].H;

                // Add node_successor to the OPEN list
                openList.Add(successors[i]);
            }
            closeList.Add(current_node);
        }
        int counter = 0;
        path = new List<Vector3>();
        nodesInPath = new List<Node>();
        NodeData endNode = closeList[closeList.Count - 1];
        /*from the last node, we get the path
        
            end->camefrom->camefrom->camefrom = path 

         */
        while(endNode.CameFrom != null)
        {
            path.Add(new Vector3(endNode.node.transform.position.x, 1.0f, endNode.node.transform.position.z));
            endNode = endNode.CameFrom;
            if (counter != 0)
            {
                Debug.DrawRay(new Vector3(path[counter - 1].x, 1.0f, path[counter - 1].z), new Vector3(path[counter].x, 1.0f, path[counter].z) - new Vector3(path[counter - 1].x, 1.0f, path[counter - 1].z), Color.magenta, 1000000);

            }
            counter++;
        }
        path.Reverse();

    }

    public static AStarManager getInstance()
    {
        return instance;
    }

    public Node getNearestNode(Transform aiUnitPosition)
    {
        Node closestNode = null;
        float minDist = Mathf.Infinity;
        foreach (Node current in nodesInWorld)
        {
            GameObject nodeObj = GameObject.Find(current.name);

            float dist = Vector3.Distance(nodeObj.transform.position, aiUnitPosition.position);
            if (dist < minDist)
            {
                closestNode = current;
                minDist = dist;
            }

        }
        return closestNode;

    }
}
