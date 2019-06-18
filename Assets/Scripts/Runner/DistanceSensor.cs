using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Calculates the distance from this sensor to an object 
// of a specific layer infront of it.
// 
public class DistanceSensor : MonoBehaviour {

    public float maxDistance = 50.0f; // Max distance to test for

    public string layer = "Geometry"; // Layer to test for

    private float distance = 0.0f; // Current distance to layer

    // Calculate a new distance each frame (for gizmos)
    private void Update()
    {
        distance = calculateDistance();
    }

    /// <summary>
    /// Calculate the distance to the layer
    /// </summary>
    /// <returns>Distance to layer</returns>
    private float calculateDistance()
    {
        RaycastHit hit;

        // Raycast forward for max distance, checking for objects in layer
        Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, 1 << LayerMask.NameToLayer(layer));

        if (hit.collider != null) // Something in layer hit, return distance
        {
            float distance = (transform.position - hit.point).magnitude;

            return distance;
        }
        else // Nothing hit, return max distance
            return maxDistance;
    }

    /// <summary>
    /// Gets the distance in units from this sensor to the object it has hit (or its max distance if nothing hit)
    /// </summary>
    public float getDistanceRaw()
    {
        return distance;
    }

    /// <summary>
    /// Gets the distance to the object it has hit as a percentage of its maximum range
    /// </summary>
    public float getDistancePercentage()
    {
        return distance / maxDistance;
    }

    /// <summary>
    /// Draws lines indicating the sensor's forward vector and max distance
    /// 
    /// Also colours the line accordingly to distance of object hit
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1 - (distance / maxDistance), (distance / maxDistance), 0.0f);
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * distance));
    }
}
