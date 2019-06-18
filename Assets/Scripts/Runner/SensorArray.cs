using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Simple array of distance sensors. Easier to access than repeatedly getting
// the components from the GameObjects
//
public class SensorArray : MonoBehaviour {

    public List<DistanceSensor> sensors = new List<DistanceSensor>();
}
