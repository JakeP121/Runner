using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    //
    // A simple input node for a neural network brain
    //
    // Will receive input from some source to be used 
    // throughout the brain.
    //
    public class InputNode : Node
    {
        private float value; // Input value

        /// <summary>
        /// Sets the input value
        /// </summary>
        public void setValue(float value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the input value (used by intermediate nodes)
        /// </summary>
        public override float getValue()
        {
            return value;
        }
    }
}