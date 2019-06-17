using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetworkComponents;

public class SimulationManager : MonoBehaviour {

    public Transform startPos; // Starting position of runners

    public float speed = 1.0f; // Game run-speed

    public int subjectCount = 20;
    private List<GameObject> runnerObjects = new List<GameObject>();
    private List<Runner> runners = new List<Runner>();
    private List<GeneticSequence> sequences = new List<GeneticSequence>();

    int generation = 0;

    public UI ui;

    float groupFitness = 0.0f; // Overall fitness of the group

	// Use this for initialization
	void Start () {
        Time.timeScale = speed;

        createGeneration();
	}
	
	// Update is called once per frame
	void Update () {
        // Increase/decrease time by using W and S keys
        if (Input.GetKeyDown(KeyCode.W))
            Time.timeScale += 0.25f;
        if (Input.GetKeyDown(KeyCode.S) && Time.timeScale >= 0.50f)
            Time.timeScale -= 0.25f;

	}

    /// <summary>
    /// Instantiates a new generation
    /// </summary>
    public void createGeneration()
    {
        generation++;

        for (int i = 0; i < subjectCount; i++) // Iterate through subject counts
        {
            // Create runner object
            GameObject RunnerObj = Instantiate(Resources.Load("Runner")) as GameObject;
            RunnerObj.transform.position = startPos.position;
            RunnerObj.transform.forward = startPos.forward;
            runnerObjects.Add(RunnerObj);

            Runner runner = RunnerObj.GetComponent<Runner>();

            if (sequences.Count == 0) // No genetic sequences yet recorded
                runner.setBrain(this); // Create new brain
            else // Genetic sequences recorded
                runner.setBrain(this, sequences[i]); // Create brain from sequence

            runners.Add(runner);
        }

        sequences.Clear();

        // Set UI for new generation
        ui.increaseGeneration();
        ui.setFitness(groupFitness / subjectCount);
        groupFitness = 0.0f;
    }

    /// <summary>
    /// Callback for when runner hits wall
    /// </summary>
    /// <param name="runner">Runner that hit the wall</param>
    /// <param name="geneticSequence">Genetic sequence of runner</param>
    /// <param name="fitness">Runner's fitness</param>
    public void runnerDied(Runner runner, GeneticSequence geneticSequence, float fitness)
    {
        runners.Remove(runner);
        sequences.Add(geneticSequence);

        ui.increaseFailures();

        groupFitness += fitness;

        if (runners.Count == 0) // If all runners finished
            reproduce(); // Reproduce and make new generation
    }

    /// <summary>
    /// Callback for when runner reaches goal
    /// </summary>
    /// <param name="runner">Runner that hit goal</param>
    /// <param name="geneticSequence">Genetic Sequence of runner</param>
    /// <param name="fitness">Runner's fitness</param>
    public void runnerSucceeded(Runner runner, GeneticSequence geneticSequence, float fitness)
    {
        runners.Remove(runner);
        sequences.Add(geneticSequence);

        ui.increaseSuccesses();

        groupFitness += fitness;

        if (runners.Count == 0) // If all runners finished
            reproduce(); // Reproduce and make new generation
    }

    /// <summary>
    /// Removes worst 50% of runners and creates a new generation from survivors
    /// </summary>
    private void reproduce()
    {
        // Remove worst half
        sequences.RemoveRange(0, subjectCount / 2);

        int possibleMutations = 3;

        // Cross breed best sequences
        for (int i = 0; i < subjectCount / 2; i++)
            sequences.Add(new GeneticSequence(sequences[Random.Range(0, subjectCount / 2)], sequences[Random.Range(0, subjectCount / 2)], Random.Range(0, possibleMutations)));

        clearField();
        createGeneration();
    }

    /// <summary>
    /// Clears the game area of runners
    /// </summary>
    private void clearField()
    {
        for (int i = 0; i < runnerObjects.Count; i++)
            Destroy(runnerObjects[i]);
        runnerObjects.Clear();
        runners.Clear();
    }
}
