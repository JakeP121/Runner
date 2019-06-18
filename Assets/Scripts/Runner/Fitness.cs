using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitness : MonoBehaviour {

    public float startingFitness = 1000;
    private float currentFitness;

    private void Start()
    {
        currentFitness = startingFitness;
    }

    /// <summary>
    /// Adjusts the fitness of this agent
    /// </summary>
    /// <param name="amount">Amount to adjust by</param>
    public void adjustFitness(float amount)
    {
        currentFitness += amount;

        if (currentFitness > 0)
            currentFitness = 0;
    }

    /// <summary>
    /// Gets the fitness of this agent
    /// </summary>
    /// <returns>Fitness float</returns>
    public float getFitness()
    {
        return currentFitness;
    }
}
