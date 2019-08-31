using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour {

    public float maxSpeed = 2.0f;
    public float acceleration = 0.2f;
    private float currentSpeed = 0.0f;
    
    public float steeringSpeed = 30.0f; // Max turning speed

    public SensorArray sensorArray; // Distance sensors placed around front of runner

    public float wallPenalty = 100.0f; // Penalty that will be applied to runner's fitness if they hit a wall
    public float checkpointReward = 250.0f; // Reward that will be applied to runner's fitness if they hit the correct checkpoint

    private NeuralNetworkComponents.NeuralNetwork brain = null;
    private int brainAddress;

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
        brain = new NeuralNetworkComponents.NeuralNetwork(7, 2, 4, geneticSequence);
        alive = true;
    }

    protected void FixedUpdate()
    {
        if (alive)
        {
            timeSinceCheckpoint += Time.deltaTime;

            // Populate neural network's first 5 input nodes with data from sensor array
            for (int i = 0; i < sensorArray.sensors.Count; i++)
                brain.setInput(i, sensorArray.sensors[i].getDistancePercentage());

            // Set brain inputs 5 and 6 to the current forward and reverse speeds as percentages of the max speed 
            if (currentSpeed > 0)
            {
                brain.setInput(5, currentSpeed / maxSpeed);
                brain.setInput(6, 0);
            }
            else
            {
                brain.setInput(6, currentSpeed / maxSpeed);
                brain.setInput(5, 0);
            }

            // Get the neural network's left and right outputs
            float turnLeft = -brain.getOutput(0);
            float turnRight = brain.getOutput(1);
            
            // Combine outputs and send it to turn
            turn(turnLeft + turnRight);

            // Get the neural network's forward and back outputs
            float forward = brain.getOutput(2);
            float back = -brain.getOutput(3);

            // Combine outputs and send it to drive
            drive(forward + back);
        }
    }

    /// <summary>
    /// Steers the runner
    /// </summary>
    /// <param name="steering">-1.0f for hard left, 1.0f for hard right</param>
    private void turn(float steering = 0.0f)
    {
        steering = Mathf.Clamp(steering, -1.0f, 1.0f); // Ensure steering is between -1.0f, and 1.0f

        transform.Rotate(0.0f, steering * steeringSpeed * Time.deltaTime, 0.0f);
    }

    /// <summary>
    /// Drives the runner forward or back
    /// </summary>
    /// <param name="input">Normalised input, -1.0 for full reverse, 1.0 for full forward</param>
    private void drive(float input = 0.0f)
    {
        input = Mathf.Clamp(input, -1.0f, 1.0f); // Ensure acceleration is between -1.0f, and 1.0f

        currentSpeed += input * acceleration; // Calculate the would-be current speed from this input

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed); // Ensure that the current speed isn't exceeding the maximum

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
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
