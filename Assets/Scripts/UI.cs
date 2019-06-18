using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// Simple UI to draw the generation number, number of failures this generation
// and number of successess last generation to the screen
//
public class UI : MonoBehaviour {

    public Text generation;
    private int generationCount = 0;

    public Text fitness;

    /// <summary>
    /// Increases the generation counter
    /// </summary>
    public void increaseGeneration()
    {
        generationCount++;
        generation.text = generationCount.ToString();
    }

    /// <summary>
    /// Sets the average fitness counter
    /// </summary>
    /// <param name="fitness">Running group's average fitness</param>
    public void setFitness(float fitness)
    {
        this.fitness.text = fitness.ToString();
    }
}
