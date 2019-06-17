using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;

public class SimulationManager : MonoBehaviour {

    public Transform startPos;

    public int subjectCount = 20;
    private List<GameObject> runnerObjects = new List<GameObject>();
    private List<Runner> runners = new List<Runner>();
    private List<GeneticSequence> sequences = new List<GeneticSequence>();

    int generation = 0;

    public UI ui;

	// Use this for initialization
	void Start () {
        Time.timeScale = 35.0f;

        createGeneration();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
            Time.timeScale += 0.25f;
        if (Input.GetKeyDown(KeyCode.S) && Time.timeScale >= 0.50f)
            Time.timeScale -= 0.25f;

	}

    public void createGeneration()
    {
        generation++;

        for (int i = 0; i < subjectCount; i++)
        {
            GameObject RunnerObj = Instantiate(Resources.Load("Runner")) as GameObject;
            RunnerObj.transform.position = startPos.position;
            RunnerObj.transform.forward = startPos.forward;
            runnerObjects.Add(RunnerObj);

            Runner runner = RunnerObj.GetComponent<Runner>();

            if (sequences.Count != 0)
                runner.setBrain(this, sequences[i]);
            else
                runner.setBrain(this);

            runners.Add(runner);
        }

        sequences.Clear();
        ui.increaseGeneration();
    }

    public void runnerDied(Runner runner, GeneticSequence geneticSequence, float fitness)
    {
        runners.Remove(runner);
        sequences.Add(geneticSequence);

        ui.increaseFailures();

        if (runners.Count == 0)
            reproduce();
    }

    public void runnerSucceeded(Runner runner, GeneticSequence geneticSequence, float fitness)
    {
        runners.Remove(runner);
        sequences.Add(geneticSequence);

        ui.increaseSuccesses();

        if (runners.Count == 0)
            reproduce();
    }

    private void reproduce()
    {
        // Remove first 25 results (low fitness)
        sequences.RemoveRange(0, subjectCount / 2);

        // Gene editting
        for (int i = 0; i < subjectCount / 2; i++)
            sequences.Add(new GeneticSequence(sequences[Random.Range(0, subjectCount / 2)], sequences[Random.Range(0, subjectCount / 2)]));

        clearField();
    }
    
    private void clearField()
    {
        for (int i = 0; i < runnerObjects.Count; i++)
            Destroy(runnerObjects[i]);
        runnerObjects.Clear();
        runners.Clear();

        createGeneration();
    }
}
