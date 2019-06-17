using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetworkComponents
{
    //
    // A simple input node for a neural network brain
    //
    // Will receive input from some source to be used 
    // throughout the brain.
    //
    public class InputNode : Node
    {
        /// <summary>
        /// Sets the input value
        /// </summary>
        public void setValue(float value)
        {
            this.value = value;
        }
    }
}