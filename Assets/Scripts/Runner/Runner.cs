using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour {

    public float speed = 2.0f; // Constant forward speed
    public float steeringSpeed = 30.0f; // Max turning speed

    public SensorArray sensorArray; // Distance sensors placed around front of runner

    public float wallPenalty = 100.0f; // Penalty that will be applied to runner's fitness if they hit a wall
    public float checkpointReward = 250.0f; // Reward that will be applied to runner's fitness if they hit the correct checkpoint

    private NeuralNetworkComponents.NeuralNetwork brain = null;

    private bool alive = false; // Has this runner hit a wall?

    private Fitness fitness;

    private float timeSinceCheckpoint = 0.0f; // Time since this runner last hit a checkpoint
    private int currentCheckpoint = 0;

    private Racecourse racecourse = null;

    private void Start()
    {
        fitness = GetComponent<Fitness>();
        racecourse = GameObject.FindObjectOfType<Racecourse>();
    }

    /// <summary>
    /// Sets the runner's brain
    /// </summary>
    /// <param name="geneticSequence">Sequence to give the brain</param>
    public void setBrain(NeuralNetworkComponents.GeneticSequence geneticSequence = null)
    {
        brain = new NeuralNetworkComponents.NeuralNetwork(5, 1, 2, geneticSequence);
        alive = true;
    }

    protected void FixedUpdate()
    {
        if (alive)
        {
            timeSinceCheckpoint += Time.deltaTime;

            // Populate neural network's input with data from sensor array
            for (int i = 0; i < sensorArray.sensors.Count; i++)
                brain.setInput(i, sensorArray.sensors[i].getDistancePercentage());

            // Get the neural network's left and right outputs
            float turnLeft = -brain.getOutput(0);
            float turnRight = brain.getOutput(1);
            
            // Combine outputs, clamp the value between -1 and 1 and move
            move(Mathf.Clamp(turnLeft + turnRight, -1.0f, 1.0f));
        }
    }

    /// <summary>
    /// Moves runner
    /// </summary>
    /// <param name="steering">-1.0f for hard left, 1.0f for hard right</param>
    private void move(float steering = 0.0f)
    {
        transform.Rotate(0.0f, steering * steeringSpeed * Time.deltaTime, 0.0f);

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!alive)
            return;

        if (collision.collider.tag == "Wall") // Collided with wall, penalise
            fitness.adjustFitness(-100);
        else
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alive)
            return;

        if (other.tag == "Checkpoint") // Collided with checkpoint
            hitCheckpoint(other.gameObject);
    }

    /// <summary>
    /// Called when the runner hits a checkpoint
    /// </summary>
    /// <param name="checkpoint">The checkpoint that has been hit</param>
    private void hitCheckpoint(GameObject checkpoint)
    {
        // Wrong checkpoint, penalise
        if (checkpoint != racecourse.checkpoints[currentCheckpoint])
        {
            fitness.adjustFitness(-checkpointReward);
            return;
        }

        // Reward the player with the checkpointReward minus how long it has taken them (to encourage faster runs)
        if (timeSinceCheckpoint < checkpointReward)
            fitness.adjustFitness(checkpointReward - timeSinceCheckpoint);

        timeSinceCheckpoint = 0.0f; // Reset time since last checkpoint to 0

        currentCheckpoint++;

        // Wrap around to first checkpoint if race finished
        if (currentCheckpoint >= racecourse.checkpoints.Count)
            currentCheckpoint = 0;
    }

    /// <summary>
    /// Gets the runner's brain's genetic sequence
    /// </summary>
    public NeuralNetworkComponents.GeneticSequence getGeneticSequence()
    {
        return brain.getGeneticSequence();
    }

}
