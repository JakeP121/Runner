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

    public Text failures;
    private int failureCount = 0;

    public Text successes;
    private int successCount = 0;

    /// <summary>
    /// Increases the generation counter
    /// </summary>
    public void increaseGeneration()
    {
        generationCount++;
        generation.text = generationCount.ToString();

        failureCount = 0;
        failures.text = "0";

        successes.text = successCount.ToString();
        successCount = 0;
    }

    /// <summary>
    /// Increases the failure counter
    /// </summary>
    public void increaseFailures()
    {
        failureCount++;
        failures.text = failureCount.ToString();
    }

    /// <summary>
    /// Increases the successes counter
    /// </summary>
    public void increaseSuccesses()
    {
        successCount++;
    }
}
