using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetworkComponents
{
    //
    // Abstract node class for neural network
    //
    // Base for input, intermediate and output nodes
    //
    abstract public class Node
    {
        protected float value = 0.0f; // The value of this node 

        /// <summary>
        /// Gets the value of this node
        /// </summary>
        /// <returns>Value between 0 and 1</returns>
        virtual public float getValue()
        {
            return value;
        }
    }
}