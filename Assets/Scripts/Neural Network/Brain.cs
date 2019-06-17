using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    //
    // An artificial neural network brain consisting of an array of input nodes
    // to represent each sensor in the sensorArray and a single intermediate layer
    //

    public class Brain
    {
        SensorArray sensorArray;

        InputNode[] inputs;
        IntermediateNode[] intermediateNodes;

        /// <summary>
        /// Creates a new brain
        /// </summary>
        /// <param name="sensorArray">Sensors to use as the input nodes</param>
        /// <param name="geneticSequence">Genetic sequence to inherit if this is a child of another brain</param>
        public Brain(SensorArray sensorArray, GeneticSequence geneticSequence = null)
        {
            this.sensorArray = sensorArray;

            createNetwork(geneticSequence);
        }

        /// <summary>
        /// Creates the artificial neural network
        /// </summary>
        /// <param name="geneticSequence">The genetic sequence to copy into the neural network if this is child</param>
        private void createNetwork(GeneticSequence geneticSequence = null)
        {
            // Create input and intermediate node arrays
            inputs = new InputNode[sensorArray.sensors.Count];
            intermediateNodes = new IntermediateNode[sensorArray.sensors.Count];

            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = new InputNode();

            if (geneticSequence != null) // If a genetic sequence was given
            {  
                // Create intermediate nodes with strength values given by genetic sequence
                for (int i = 0; i < intermediateNodes.Length; i++)
                    intermediateNodes[i] = new IntermediateNode(inputs, geneticSequence.getNodeSequence(i));
            }
            else // No genetic sequence was given
            {
                // Create intermediate nodes with random strength values
                for (int i = 0; i < intermediateNodes.Length; i++)
                    intermediateNodes[i] = new IntermediateNode(inputs);
            }
        }

        /// <summary>
        /// Feeds the inputs from sensor array into input nodes
        /// </summary>
        private void feedInputs()
        {
            float sensorMaxDistance = 10.0f;

            for (int i = 0; i < inputs.Length; i++)
                inputs[i].setValue(sensorArray.sensors[i].getDistanceRaw() / sensorMaxDistance);
        }

        /// <summary>
        /// Calculates the steering 
        /// </summary>
        /// <returns>-1.0f for hard left, 1.0f for hard right</returns>
        public float calculateSteering()
        {
            // Feed input nodes with sensor array data
            feedInputs();

            float steering = 0.0f;

            // Create steering value from sum of all intermediate node values
            for (int i = 0; i < intermediateNodes.Length; i++)
                steering += intermediateNodes[i].getValue();

            // Clamp steering between -1.0f and 1.0f
            steering = Mathf.Clamp(steering, -1.0f, 1.0f);

            return steering;
        }

        /// <summary>
        /// Returns the array of ANN input nodes
        /// </summary>
        /// <returns>Array of Nodes</returns>
        public Node[] getInputNodes()
        {
            return inputs;
        }

        /// <summary>
        /// Returns the array of ANN intermediate nodes
        /// </summary>
        /// <returns>Array of Nodes</returns>
        public Node[] getIntermediateNodes()
        {
            return intermediateNodes;
        }

        /// <summary>
        /// Returns the genetic sequence of this brain's artificial neural network
        /// </summary>
        /// <returns>Genetic Sequence</returns>
        public GeneticSequence getGeneticSequence()
        {
            List<float[]> nodeSequences = new List<float[]>();

            // Get genetic data from each intermediate node
            foreach (IntermediateNode i in intermediateNodes)
                nodeSequences.Add(i.getGeneticData());

            return new GeneticSequence(nodeSequences);
        }
    }
}