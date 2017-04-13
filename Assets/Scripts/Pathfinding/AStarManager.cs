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
        foreach (Node actual in nodesInWorld) {
            foreach (Node toCheck in nodesInWorld) {
                if (actual.gameObject == toCheck.gameObject) continue;
                if (Physics.Raycast(actual.transform.position, toCheck.transform.position - actual.transform.position, out hit)) {
                    if (hit.transform.gameObject == toCheck.gameObject) {
                        actual.accessibleNodes.Add(toCheck);
                    }
                }
            }
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
    public void calculatePath(Node start, Node end, out List<Vector3> path)
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
            if (current_node.x == end.data.x &&
                current_node.z == end.data.z)
            {
                closeList.Add(current_node);
                break;
            }
            // Generate each state node_successor that can come after node_current
            List<NodeData> successors = new List<NodeData>();
            successors.Clear();
            for (int i = 0; i < current_node.parent.accessibleNodes.Count; i++)
            {
                NodeData n = new NodeData();
                n.parent = current_node.parent.accessibleNodes[i];
                n.F = current_node.parent.accessibleNodes[i].F;
                n.G = current_node.parent.accessibleNodes[i].G;
                n.H = current_node.parent.accessibleNodes[i].H;
                n.x = current_node.parent.accessibleNodes[i].data.x;
                n.z = current_node.parent.accessibleNodes[i].data.z;
                //set the cost to the distance, maybe.
                successors.Add(n);
            }
            // For each node_successor of node_current
            for (int i = 0; i < successors.Count; i++)
            {
                // Set the G of node_successor to be the G of node_current plus the G to get to node_successor from node_current
                successors[i].G = current_node.G + successors[i].G;
                // Find node_successor on the OPEN list
                bool found_in_open_list = false;
                for (int a = 0; a < openList.Count; a++)
                {
                    // If node_successor is on the OPEN list but the existing one is as good or better then discard this successor and continue with next successor
                    if (successors[i].x == openList[a].x &&
                         successors[i].z == openList[a].z)
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
                    if (successors[i].x == closeList[a].x &&
                         successors[i].z == closeList[a].z)
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
                    if (openList[b].x == successors[i].x &&
                         openList[b].z == successors[i].z)
                    {
                        openList.RemoveAt(b);
                        break;
                    }
                }
                for (int b = 0; b < closeList.Count; b++)
                {
                    if (closeList[b].x == successors[i].x &&
                         closeList[b].z == successors[i].z)
                    {
                        closeList.RemoveAt(b);
                        break;
                    }
                }

                // Set the point2D of node_successor to node_current
                //this makes no fucking sense, so i removed it
                //current_node.x = successors[i].x;
                //current_node.z = successors[i].z;
                //CHECK THIS PART.

                // Set h to be the estimated distance to node_goal (using the H function)
                float final_H_X = Mathf.Pow(end_node.x - successors[i].x, 2.0f);
                float final_H_Z = Mathf.Pow(end_node.z - successors[i].z, 2.0f);

                if (final_H_X < 0) final_H_X *= -1;
                if (final_H_Z < 0) final_H_Z *= -1;

                successors[i].H = final_H_X + final_H_Z;
                successors[i].F = successors[i].G + successors[i].H;
                
                // Add node_successor to the OPEN list
                openList.Add(successors[i]);
            }
            closeList.Add(current_node);
        }
        int counter = 0;
        path = new List<Vector3>();//[closeList.Count];
        foreach(NodeData n in closeList)
        {
            if (!done) { 
            //n.parent.GetComponent<Renderer>().material.color = Color.green;
            }
            path.Add(new Vector3(n.x, 1.0f, n.z));
            //path[counter].x = n.x;
            //path[counter].y = 1.0f;
            //path[counter].z = n.z;
            counter++;
        }
        done = true;
    }

    public static AStarManager getInstance()
    {
        return instance;
    }

    public Node getNearestNode(Transform aiUnitPosition)
    {
        Node closestNode = null;
        float minDist = Mathf.Infinity;
        foreach(Node current in nodesInWorld)
        {
            GameObject nodeObj = GameObject.Find(current.name);

            float dist = Vector3.Distance(nodeObj.transform.position, aiUnitPosition.position);
            if(dist < minDist)
            {
                closestNode = current;
                minDist = dist;
            }

        }
        return closestNode;

    }
}
