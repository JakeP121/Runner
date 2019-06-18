using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetworkComponents
{
    //
    // An artifical neural network created from a variable
    // amount of inputs, hidden layers and outputs
    //
    public class NeuralNetwork
    {
        private int inputCount; // Number of input nodes
        private int hiddenLayerCount; // Number of hidden layers (of which each layer has inputCount nodes)
        private int outputCount; // Number of output nodes

        private InputNode[] inputNodes;
        private List<CalculationNode[]> hiddenLayers = new List<CalculationNode[]>();
        private CalculationNode[] outputNodes;

        private bool needsRecalculation = true; // Does the neural network need to be recalculated before an output can be given?

        /// <summary>
        /// Creates a neural network 
        /// </summary>
        /// <param name="inputCount">Number of inputs that the network will take into consideration</param>
        /// <param name="hiddenLayerCount">Number of calculation layers between input and output</param>
        /// <param name="outputCount">Number of outputs that this network will calculate</param>
        /// <param name="geneticSequence">Genetic sequence to copy from</param>
        public NeuralNetwork(int inputCount, int hiddenLayerCount, int outputCount, GeneticSequence geneticSequence = null)
        {
            this.inputCount = inputCount;
            this.hiddenLayerCount = hiddenLayerCount;
            this.outputCount = outputCount;

            createNetwork(geneticSequence);
        }

        /// <summary>
        /// Assigns/randomises neural network values
        /// </summary>
        /// <param name="geneticSequence">Genetic sequence to apply to the network</param>
        private void createNetwork(GeneticSequence geneticSequence = null)
        {
            // Create input nodes
            inputNodes = new InputNode[inputCount];

            for (int i = 0; i < inputCount; i++)
                inputNodes[i] = new InputNode();


            Node[] layerInputs = inputNodes; // The input nodes of each layer will be the nodes in the previous layer (starting with input)

            // Create hidden layers
            for (int i = 0; i < hiddenLayerCount; i++)
            {
                CalculationNode[] layer = new CalculationNode[inputCount];

                // Create nodes in layer
                for (int j = 0; j < inputCount; j++)
                {
                    if (geneticSequence == null) // No genetic sequence given
                        layer[j] = new CalculationNode(layerInputs); // Create random inputs
                    else // Genetic sequence given
                        layer[j] = new CalculationNode(layerInputs, geneticSequence.getNodeSequence(i, j)); // Assign appropriate node sequence
                }

                hiddenLayers.Add(layer);

                layerInputs = layer; // Next layer's inputs will be this current layer
            }

            // Create output nodes
            outputNodes = new CalculationNode[outputCount];

            for (int i = 0; i < outputCount; i++)
            {
                if (geneticSequence == null)
                    outputNodes[i] = new CalculationNode(layerInputs);
                else
                    outputNodes[i] = new CalculationNode(layerInputs, geneticSequence.getNodeSequence(hiddenLayerCount, i));
            }
        }

        /// <summary>
        /// Runs the neural network
        /// </summary>
        private void run()
        {
            // Iterate through hidden layers
            foreach (CalculationNode[] layer in hiddenLayers)
            {
                // Iterate through nodes in this layer
                foreach (CalculationNode node in layer)
                    node.run();
            }

            // Iterate through output nodes
            foreach (CalculationNode node in outputNodes)
                node.run();

            needsRecalculation = false;
        }

        /// <summary>
        /// Sets the value of an input node
        /// </summary>
        /// <param name="index">Index of the input node</param>
        /// <param name="value">Value to set</param>
        public void setInput(int index, float value)
        {
            // Make sure index is valid
            if (index >= inputCount)
            {
                Debug.LogError("Invalid index");
                return;
            }

            inputNodes[index].setValue(value);

            needsRecalculation = true;
        }

        /// <summary>
        /// Gets the result of the output node
        /// </summary>
        /// <param name="index">Index of the output node if there are more than one</param>
        /// <returns>The value of the output node or -999999 if invalid index</returns>
        public float getOutput(int index = 0)
        {
            // Make sure index is valid
            if (index >= outputCount)
            {
                Debug.LogError("Invalid index");
                return -9999999.0f;
            }
            
            // Only re-run the neural network if an input has changed
            if (needsRecalculation)
                run();

            return outputNodes[index].getValue();
        }

        /// <summary>
        /// Creates a genetic sequence from this network's strength and bias values
        /// </summary>
        /// <returns>A Genetic sequence</returns>
        public GeneticSequence getGeneticSequence()
        {
            GeneticSequence sequence = new GeneticSequence(inputCount, hiddenLayerCount, outputCount);

            // Add values from hidden layers
            foreach (CalculationNode[] nodes in hiddenLayers)
                sequence.addLayer(nodes);

            // Add values from output layer
            sequence.addLayer(outputNodes);

            return sequence;
        }
    }
}