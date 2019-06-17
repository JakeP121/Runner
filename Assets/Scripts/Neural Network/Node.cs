using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    //
    // Abstract node class for neural network
    //
    // Base for input, intermediate and output nodes
    //
    abstract public class Node
    {
        abstract public float getValue();
    }
}