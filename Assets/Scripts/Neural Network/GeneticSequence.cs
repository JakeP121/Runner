using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetworkComponents
{
    //
    // A numerical representation of a neural network
    //
    public class GeneticSequence
    {
        struct Layer // Break the sequences between layers to simplify things
        {
            public List<float[]> nodeSequences; // Each float array is a single node's genetic sequence
        }

        private List<Layer> layers = new List<Layer>();

        int inputCount; 
        int hiddenLayerCount;
        int outputCount;

        /// <summary>
        /// Creates a genetic sequence 
        /// </summary>
        /// <param name="inputCount">Number of input nodes</param>
        /// <param name="hiddenLayerCount">Number of layers between network's input and output</param>
        /// <param name="outputCount">Number of output nodes</param>
        public GeneticSequence(int inputCount, int hiddenLayerCount, int outputCount)
        {
            this.inputCount = inputCount;
            this.hiddenLayerCount = hiddenLayerCount;
            this.outputCount = outputCount;
        }

        /// <summary>
        /// Creates a genetic sequence from two parent sequences
        /// </summary>
        /// <param name="aSeq">Parent A</param>
        /// <param name="bSeq">Parent B</param>
        /// <param name="mutations">Number of possible mutations</param>
        public GeneticSequence(GeneticSequence aSeq, GeneticSequence bSeq, int mutations = 0)
        {
            float[] a = aSeq.toArray();
            float[] b = bSeq.toArray();

            // A and B must be same length
            if (a.Length != b.Length)
            {
                Debug.LogError("Parents do not have compatible genetic sequences");
                return;
            }

            inputCount = aSeq.inputCount;
            hiddenLayerCount = aSeq.hiddenLayerCount;
            outputCount = aSeq.outputCount;

            float[] mine = new float[a.Length];

            // Iterate through this new (blank) genetic sequence
            for (int i = 0; i < mine.Length; i++)
            {
                // Randomly pick between parent A and parent B's values
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                    mine[i] = a[i];
                else
                    mine[i] = b[i];
            }

            // Mutate
            for (int i = 0; i < mutations; i++)
            {
                int index = Random.Range(0, mine.Length);
                mine[index] = Random.Range(0.0f, 1.0f);
            }

            storeSequence(mine); // Save sequence
        }

        /// <summary>
        /// Adds another layer to the genetic sequence
        /// </summary>
        /// <param name="nodes">Neural network's nodes in this layer</param>
        public void addLayer(CalculationNode[] nodes)
        {
            // Create a new layer
            Layer newLayer = new Layer();
            newLayer.nodeSequences = new List<float[]>();

            // Iterate through nodes and add their genetic data to a new layer
            foreach (CalculationNode node in nodes)
                newLayer.nodeSequences.Add(node.getGeneticData());

            // Save the layer
            layers.Add(newLayer);
        }

        /// <summary>
        /// Converts the seperated arrays of this genetic sequence into a single array
        /// </summary>
        /// <returns>A single float array representation of this genetic sequence</returns>
        public float[] toArray()
        {
            //
            // First pass - find array size
            //
            int arraySize = 0;

            foreach (Layer layer in layers)
            {
                foreach (float[] nodeSequence in layer.nodeSequences)
                    arraySize += nodeSequence.Length;
            }

            // Create array
            float[] array = new float[arraySize];

            //
            // Second pass - populate array
            //

            int index = 0;
            foreach (Layer layer in layers) // Iterate through layers
            {
                foreach (float[] nodeSequence in layer.nodeSequences) // Iterate through nodes
                {
                    foreach (float f in nodeSequence) // Iterate through values in node sequences
                    {
                        array[index] = f;
                        index++;
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// Converts a float array representation of a sequnce into a more OO approach and saves it
        /// </summary>
        /// <param name="sequence">Sequence to save</param>
        private void storeSequence(float[] sequence)
        {
            // Clear any current sequence
            layers.Clear();

            int sequenceLength = inputCount + 1; // Input strengths plus bias

            int sequenceIndex = 0;

            for (int i = 0; i < hiddenLayerCount; i++) // Iterate through layers
            {
                Layer newLayer = new Layer();
                newLayer.nodeSequences = new List<float[]>();

                for (int j = 0; j < inputCount; j++) // Iterate through nodes
                {
                    float[] newSequence = new float[sequenceLength];

                    for (int k = 0; k < sequenceLength; k++) // Iterate through values in node sequences
                    {
                        newSequence[k] = sequence[sequenceIndex];
                        sequenceIndex++;
                    }

                    newLayer.nodeSequences.Add(newSequence);
                }

                layers.Add(newLayer); // Add the layer to the array
            }

            //
            // Add output layer
            //
            Layer outputLayer = new Layer();
            outputLayer.nodeSequences = new List<float[]>();

            for (int i = 0; i < outputCount; i++) // Iterate through output nodes
            {
                float[] newSequence = new float[sequenceLength]; // New sequence for each new node

                for (int j = 0; j < sequenceLength; j++) // Iterate through values in node sequence
                {
                    newSequence[j] = sequence[sequenceIndex];
                    sequenceIndex++;
                }

                // Add the sequence to the layer
                outputLayer.nodeSequences.Add(newSequence);
            }

            // Add the layer to the array
            layers.Add(outputLayer);
        }

        /// <summary>
        /// Gets an individual node's genetic sequence
        /// </summary>
        /// <param name="layer">Node's layer index (excluding input layer)</param>
        /// <param name="index">Node's index within layer</param>
        /// <returns>Specified node's genetic sequence</returns>
        public float[] getNodeSequence(int layer, int index)
        {
            // Make sure layer and index are within bounds
            if (layer >= hiddenLayerCount + 1 || index >= inputCount)
            {
                Debug.LogError("Invalid layer or index");
                return null;
            }

            return layers[layer].nodeSequences[index];
        }
    }
}