using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Logic for the runner GameObject
//
public class Runner : MonoBehaviour {

    public float speed = 2.0f; // Constant forward speed
    public float steeringSpeed = 30.0f; // Max turning speed

    private NeuralNetwork.Brain brain = null;
    private SimulationManager manager;

    private float fitness = 0.0f; // How well this runner is performing 
    private bool alive = false; // Has this runner hit a wall?

    /// <summary>
    /// Sets the runner's brain
    /// </summary>
    /// <param name="manager">Simulation manager reference to call back to upon death</param>
    /// <param name="geneticSequence">Sequence to give the brain</param>
    public void setBrain(SimulationManager manager, NeuralNetwork.GeneticSequence geneticSequence = null)
    {
        brain = new NeuralNetwork.Brain(GetComponentInChildren<SensorArray>(), geneticSequence);
        alive = true;
        this.manager = manager;
    }

    protected void FixedUpdate()
    {
        if (alive)
        {
            // Increase fitness every second this runner is alive
            fitness += Time.deltaTime;

            move(brain.calculateSteering());
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

        if (collision.collider.tag == "Wall") // Collided with wall
        {
            // Call back to simulation manager that this runner has died
            manager.runnerDied(this, brain.getGeneticSequence(), fitness);
            alive = false;
        }
        else if (collision.collider.tag == "Goal") // Collided with goal
        {
            // Call back to simulation manager that this runner has succeeded 
            manager.runnerSucceeded(this, brain.getGeneticSequence(), fitness);
            alive = false;
        }
        else
            return;
    }
}
