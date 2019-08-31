using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public static class ANNPluginManager {

    private static Dictionary<Runner, int> runnerBrains = new Dictionary<Runner, int>();

    [DllImport("Artificial Neural Network")]
    private static extern int createNewBrain_C(int inputCount, int hiddenLayerCount, int outputCount);

    [DllImport("Artificial Neural Network")]
    private static extern int createSetBrain_C(int inputCount, int hiddenLayerCount, int outputCount, float[] geneticSequence);

    public static void newBrain(Runner runner, int inputCount, int hiddenLayerCount, int outputCount, NeuralNetworkComponents.GeneticSequence geneticSequence = null)
    {
        int brainIndex;

        if (geneticSequence != null)
            brainIndex = createSetBrain_C(inputCount, hiddenLayerCount, outputCount, geneticSequence.toArray());
        else
            brainIndex = createNewBrain_C(inputCount, hiddenLayerCount, outputCount);


        runnerBrains.Add(runner, brainIndex);
    }


    [DllImport("Artificial Neural Network", EntryPoint = "deleteBrain_C", CallingConvention = CallingConvention.Cdecl)]
    private static extern void deleteBrain_C(int index);

    public static void deleteBrain(Runner runner)
    {
        if (runnerBrains.ContainsKey(runner))
        {
            deleteBrain_C(runnerBrains[runner]);
            runnerBrains.Remove(runner);
        }
    }

    [DllImport("Artificial Neural Network", EntryPoint = "getGeneticSequence_C", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getGeneticSequence_C(int index, IntPtr length);

    public static float[] getGeneticSequence(Runner runner)
    {
        if (runnerBrains.ContainsKey(runner))
        {
            IntPtr lengthPtr = Marshal.AllocHGlobal(sizeof(int));

            IntPtr dataPtr = Marshal.AllocHGlobal(sizeof(float));

            dataPtr = getGeneticSequence_C(runnerBrains[runner], lengthPtr);

            int length = Marshal.ReadInt32(lengthPtr);
            float[] sequence = new float[length];

            Marshal.Copy(dataPtr, sequence, 0, length);

            Marshal.FreeCoTaskMem(lengthPtr);
            Marshal.FreeCoTaskMem(dataPtr);

            return sequence;
        }
        else
            return null;
    }
}
