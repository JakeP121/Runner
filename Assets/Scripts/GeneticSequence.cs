using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    //
    // The genetic sequence that represents a neural network brain
    // through an array of numbers
    //
    public class GeneticSequence
    {
        public List<float[]> nodeSequences = new List<float[]>();

        /// <summary>
        /// Creates a genetic sequence
        /// </summary>
        /// <param name="sequence">
        /// 
        /// 
        /// 
        /// </param>
        public GeneticSequence(List<float[]> sequence)
        {
            this.nodeSequences = sequence;
        }

        /// <summary>
        /// Creates a genetic sequence from combining two parent sequences
        /// </summary>
        /// <param name="aSeq">Parent A's genetic sequence</param>
        /// <param name="bSeq">Parent B's genetic sequence</param>
        public GeneticSequence(GeneticSequence aSeq, GeneticSequence bSeq)
        {
            float[] a = aSeq.toArray();
            float[] b = bSeq.toArray();

            // A and B must be same length
            if (a.Length != b.Length)
            {
                Debug.LogError("Parents do not have compatible genetic sequences");
                return;
            }

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

            // Save this sequence
            storeSequence(mine);
        }

        /// <summary>
        /// Turns genetic sequence into a single array of values
        /// </summary>
        /// <returns></returns>
        public float[] toArray()
        {
            int arraySize = 0;

            // Iterate through each node's genetic sequence and add their length to the overall array size
            for (int i = 0; i < nodeSequences.Count; i++)
                arraySize += nodeSequences[i].Length;

            float[] array = new float[arraySize];

            int currentIndex = 0;
            for (int i = 0; i < nodeSequences.Count; i++)
            {
                for (int j = 0; j < nodeSequences[i].Length; j++)
                {
                    array[currentIndex] = nodeSequences[i][j];
                    currentIndex++;
                }
            }

            return array;
        }

        public void storeSequence(float[] sequence)
        {
            nodeSequences.Clear();

            int nodeSequenceLength = 6; // (5 strengths, 1 bias)

            int i = 0;
            do
            {
                float[] nodeSequence = new float[nodeSequenceLength];

                for (int j = 0; j < nodeSequenceLength; j++)
                    nodeSequence[j] = sequence[i + j];

                i += nodeSequenceLength;

                nodeSequences.Add(nodeSequence);
            }
            while (i < sequence.Length);
        }

        /// <summary>
        /// Splice this genetic sequence with another
        /// </summary>
        /// <param name="other">Sequence to splice with</param>
        /// <param name="splices">Number of random values to copy</param>
        public void splice(GeneticSequence other, int splices = 5)
        {
            float[] ourCode = this.toArray();
            float[] theirCode = other.toArray();

            // Splice random values 
            for (int i = 0; i < splices; i++)
            {
                int index = Random.Range(0, ourCode.Length - 1);
                ourCode[index] = theirCode[index];
            }

            // Save new sequence
            storeSequence(ourCode);
        }

        /// <summary>
        /// Gets the genetic sequence of only a single node
        /// </summary>
        /// <param name="node">Node index</param>
        /// <returns>node's genetic sequence array</returns>
        public float[] getNodeSequence(int node)
        {
            return nodeSequences[node];
        }
    }
}