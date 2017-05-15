using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NaiveBayesManager : MonoBehaviour
{

    public NaiveBayes b;

    public Button fast;
    public Button heavy;
    public Text C;
    public Text result;

    //agent class 0 fast / 1 heavy
    int classType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickFast()
    {
        C.text = "Fast";
        classType = 0;
    }
    public void OnClickHeavy()
    {
        C.text = "Heavy";
        classType = 1;
    }
    public void calculate()
    {
        result.text = b.Classify(classType);
    }

}
