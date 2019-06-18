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

    public float simulationLength = 120.0f; // How many seconds this simulation will run for
    private float simulationTimer = 0.0f; // How many seconds this sumulation has been running for

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

        simulationTimer += Time.deltaTime;

        if (simulationTimer > simulationLength)
        {
            startNewGeneration();
            simulationTimer = 0.0f;
        }
	}

    /// <summary>
    /// Kills off worst runners, breeds a new generation and pits them against the best of the current generation
    /// </summary>
    private void startNewGeneration()
    {
        quicksort(ref runners, 0, runners.Count - 1);

        float averageFitness = 0.0f;

        foreach (Runner r in runners)
            averageFitness += r.GetComponent<Fitness>().getFitness();

        averageFitness /= runners.Count;

        ui.setFitness(averageFitness);

        reproduce(averageFitness);
        clearField();
        createGeneration();
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
                runner.setBrain(); // Create new brain
            else // Genetic sequences recorded
                runner.setBrain(sequences[i]); // Create brain from sequence

            runners.Add(runner);
        }

        sequences.Clear();

        ui.increaseGeneration();
    }

    /// <summary>
    /// Removes worst 50% of runners and creates a new generation from survivors
    /// </summary>
    private void reproduce()
    {
        // Copy best half of runners genetic sequences
        for (int i = runners.Count- 1; i >= subjectCount / 2; i--)
            sequences.Add(runners[i].GetComponent<Runner>().getGeneticSequence());

        int possibleMutations = 3;

        // Cross breed best sequences
        for (int i = 0; i < subjectCount / 2; i++)
            sequences.Add(new GeneticSequence(sequences[Random.Range(0, subjectCount / 2)], sequences[Random.Range(0, subjectCount / 2)], Random.Range(0, possibleMutations)));
    }

    /// <summary>
    /// Removes all runners below average fitness
    /// </summary>
    private void reproduce(float averageFitness)
    {
        // Only store genetic data for above average runners
        {
            int i = runners.Count - 1;
            bool averageRunnerFound = false;

            do
            {
                if (runners[i].GetComponent<Fitness>().getFitness() >= averageFitness)
                    sequences.Add(runners[i].getGeneticSequence());
                else
                    averageRunnerFound = true;

                i--;
            } while (i > 0 && !averageRunnerFound);
        }

        int aboveAverageRunners = sequences.Count;

        int possibleMutations = 3;

        // Cross breed best sequences
        for (int i = 0; i < subjectCount - aboveAverageRunners; i++)
            sequences.Add(new GeneticSequence(sequences[Random.Range(0, aboveAverageRunners)], sequences[Random.Range(0, aboveAverageRunners)], Random.Range(0, possibleMutations)));
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

    /// <summary>
    /// Quicksorts the runners from worst to best fitness levels
    /// 
    /// Adapted from: https://codereview.stackexchange.com/questions/142808/quick-sort-algorithm
    /// </summary>
    /// <param name="runners">Runner array to sort</param>
    /// <param name="left">Start of array partition</param>
    /// <param name="right">End of array partition</param>
    private void quicksort(ref List<Runner> runners, int left, int right)
    {
        if (left > right || left < 0 || right < 0)
            return;

        int index = partition(ref runners, left, right);

        if (index != -1)
        {
            quicksort(ref runners, left, index - 1);
            quicksort(ref runners, index + 1, right);
        }
    }

    /// <summary>
    /// Moves a pivot in the array to its correct position, sorting elements along the way
    /// 
    /// Adapted from: https://codereview.stackexchange.com/questions/142808/quick-sort-algorithm
    /// </summary>
    /// <param name="runners">Runner array to sort</param>
    /// <param name="left">Start of array partition</param>
    /// <param name="right">End of array partition</param>
    /// <returns>Index of end of array</returns>
    private int partition (ref List<Runner> runners, int left, int right)
    {
        if (left > right)
            return -1;

        int end = left;

        Runner pivot = runners[right];

        for (int i = left; i < right; i++)
        {
             if (runners[i].GetComponent<Fitness>().getFitness() < pivot.GetComponent<Fitness>().getFitness())
            {
                swap(ref runners, i, end);
                end++;
            }
        }

        swap(ref runners, end, right);
        return end;
    }

    /// <summary>
    /// Swaps two elements in the runners array
    /// </summary>
    /// <param name="runners">Array housing all runners</param>
    /// <param name="left">Index of first runner to swap</param>
    /// <param name="right">Index of second runner to swap</param>
    private void swap(ref List<Runner> runners, int left, int right)
    {
        Runner temp = runners[left];
        runners[left] = runners[right];
        runners[right] = temp;
    }
}
