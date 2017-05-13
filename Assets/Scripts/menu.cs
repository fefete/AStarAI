using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

    public string ctfScene;
    public string naiveBayesScene;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadSimulation() {
        SceneManager.LoadScene(ctfScene);
    }
    public void LoadNaiveBayes()
    {
        SceneManager.LoadScene(naiveBayesScene);
    }
}
