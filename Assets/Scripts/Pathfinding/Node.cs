using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * HEURISTIC
 *                 //float final_H_X = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(successors[i].x - end_node.x), 2.0f) + Mathf.Pow(Mathf.Abs(successors[i].z - end_node.z), 2.0f));
                float final_H_X = Mathf.Abs(successors[i].x - end_node.x) + Mathf.Abs(successors[i].z - end_node.z);
 * 
 * G
 *                 //successors[i].G = current_node.G + Mathf.Sqrt(Mathf.Pow(Mathf.Abs(current_node.x - successors[i].x), 2.0f) + Mathf.Pow(Mathf.Abs(current_node.z - successors[i].z), 2.0f));
                successors[i].G = q.G + Mathf.Abs(q.x - successors[i].x)+ Mathf.Abs(q.z - successors[i].z);
 */

public class Node : MonoBehaviour {

    public List<Node> accessibleNodes;
    public NodeData data;
    public float G;
    public float H;
    public float F;
    public float cost_to_arrive;

    public void Start()
    {
        data = new NodeData();
        //data.x = transform.position.x;
        //data.y = transform.position.y;
        //data.z = transform.position.z;
        data.G = G;
        data.H = H;
        data.F = F;
        data.name = gameObject.name;
        data.node = this;
        data.CameFrom = null;
    }

}
