using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    static AStarManager instance = null;
    private Node[] nodesInWorld;
    // Use this for initialization
    void Start()
    {

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

    public void calculatePath(Node start, Node end)
    {
        Debug.Log("start algorithm");
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        Node start_node = start;
        Node end_node = end;

        start_node.F = 0;
        start_node.G = 0;
        start_node.H = 0;

        Node current_node = start;
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
            if (current_node.x == end.x &&
                current_node.y == end.y)
            {
                closeList.Add(current_node);
                break;
            }
            // Generate each state node_successor that can come after node_current
            List<Node> successors = new List<Node>();
            successors.Clear();
            for (int i = 0; i < current_node.accessibleNodes.Length; i++)
            {
                Node n;
                n = Instantiate(current_node.accessibleNodes[i]);
                n.F = current_node.accessibleNodes[i].F;
                n.G = current_node.accessibleNodes[i].G;
                n.H = current_node.accessibleNodes[i].H;
                n.x = current_node.accessibleNodes[i].x;
                n.y = current_node.accessibleNodes[i].y;
                //set the cost to the distance, maybe.
                successors.Add(n);
            }
            // For each node_successor of node_current
            for (int i = 0; i < successors.Count; i++)
            {
                // Set the G of node_successor to be the G of node_current plus the G to get to node_successor from node_current
                successors[i].G = current_node.G + successors[i].cost_to_arrive;
                // Find node_successor on the OPEN list
                bool found_in_open_list = false;
                for (int a = 0; a < openList.Count; a++)
                {
                    // If node_successor is on the OPEN list but the existing one is as good or better then discard this successor and continue with next successor
                    if (successors[i].x == openList[a].x &&
                         successors[i].y == openList[a].y)
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
                    if (successors[i].transform.position.x == closeList[a].transform.position.x &&
                         successors[i].transform.position.y == closeList[a].transform.position.y)
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
                         openList[b].y == successors[i].y)
                    {
                        openList.RemoveAt(b);
                        break;
                    }
                }
                for (int b = 0; b < closeList.Count; b++)
                {
                    if (closeList[b].x == successors[i].x &&
                         closeList[b].y == successors[i].y)
                    {
                        closeList.RemoveAt(b);
                        break;
                    }
                }

                // Set the point2D of node_successor to node_current
                current_node.x = successors[i].x;
                current_node.y = successors[i].y;
                //CHECK THIS PART.

                // Set h to be the estimated distance to node_goal (using the H function)
                float final_H_X = Mathf.Pow(end_node.x - successors[i].x, 2.0f);
                float final_H_Y = Mathf.Pow(end_node.y - successors[i].y, 2.0f);

                if (final_H_X < 0) final_H_X *= -1;
                if (final_H_Y < 0) final_H_Y *= -1;

                successors[i].H = final_H_X + final_H_Y;
                successors[i].F = successors[i].G + successors[i].H;
                
                // Add node_successor to the OPEN list
                openList.Add(successors[i]);
            }
            closeList.Add(current_node);
        }
        foreach(Node n in closeList)
        {
            Debug.Log(n.transform.position);
        }
    }

    public static AStarManager getInstance()
    {
        return instance;
    }
}
