using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Represents a racecourse complete with checkpoints and (optional) laps
//
public class Racecourse : MonoBehaviour {

    public int laps;

    public List<Transform> startPositions;
    public List<GameObject> checkpoints;

    private float courseLength = 0.0f;

	// Use this for initialization
	void Start () {
        courseLength = calculateCourseLength();
	}

    /// <summary>
    /// Calculates the length of the course
    /// </summary>
    /// <returns>Course length</returns>
    private float calculateCourseLength()
    {
        float length = 0.0f;

        Vector3 averageStartPosition = Vector3.zero;

        foreach (Transform t in startPositions)
            averageStartPosition += t.position;

        averageStartPosition /= startPositions.Count;

        length += (averageStartPosition - checkpoints[0].transform.position).magnitude;

        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (i < checkpoints.Count -1)
                length += (checkpoints[i].transform.position - checkpoints[i + 1].transform.position).magnitude;
        }

        return length;
    }
}
