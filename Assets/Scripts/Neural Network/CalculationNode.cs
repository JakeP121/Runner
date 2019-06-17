using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetworkComponents
{
    //
    // A node that calculates a value from multiple other nodes in a neural network
    //

    public class CalculationNode : Node
    {
        // Each connected node will have its own strength
        struct Connection
        {
            public Node node;
            public float strength;
        }

        Connection[] connections; // Input nodes to this node
        float bias = 0.5f; // Modifier for final value of this node

        /// <summary>
        /// Creates a new calculation node
        /// </summary>
        /// <param name="inputNodes">All nodes that will feed into this node</param>
        /// <param name="geneticSequence">The input strengths and bias for this node</param>
        public CalculationNode(Node[] inputNodes, float[] geneticSequence = null)
        {
            if (geneticSequence != null && geneticSequence.Length != inputNodes.Length + 1)
            {
                Debug.LogError("Genetic sequence length does not match number of input nodes and bias");
                return;
            }

            // Create connections array
            connections = new Connection[inputNodes.Length];

            // Populate connections array
            for (int i = 0; i < inputNodes.Length; i++)
            {
                connections[i].node = inputNodes[i];

                if (geneticSequence == null) // If genetic sequence was not given
                    connections[i].strength = Random.Range(0.0f, 1.0f); // Give random strength value
                else // Genetic sequence was given
                    connections[i].strength = geneticSequence[i]; // Give input strength value
            }

            if (geneticSequence == null) // Genetic sequence not given
                bias = Random.Range(0.0f, 1.0f); // Give random bias
            else // Genetic sequence was given
                bias = geneticSequence[geneticSequence.Length - 1]; // Give input bias 
        }

        /// <summary>
        /// Calculate value from all input nodes
        /// </summary>
        /// <returns>Calculated value</returns>
        public float run()
        {
            value = 0.0f;

            // Product is sum of all input node values, multiplied by their strengths
            for (int i = 0; i < connections.Length; i++)
                value += connections[i].node.getValue() * connections[i].strength;

            // Affect product by bias
            value += bias;

            return value;
        }

        public override float getValue()
        {
            return run();
        }

        /// <summary>
        /// Gets the genetic data from this node
        /// </summary>
        /// <returns>Array of all input strengths and node bias</returns>
        public float[] getGeneticData()
        {
            float[] values = new float[connections.Length + 1];

            // Add all strengths to sequence
            for (int i = 0; i < connections.Length; i++)
                values[i] = connections[i].strength;

            // Add bias to sequence
            values[values.Length - 1] = bias;

            return values;
        }
    }
}