using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Node[] accessibleNodes;
    public float G;
    public float H;
    public float F;
    public float x;
    public float y;
    public float cost_to_arrive;

    public void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

}
