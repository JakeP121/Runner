#pragma once
#include "stdafx.h"

#include "NeuralNetwork.h"
#include <unordered_map>

std::unordered_map<int, NeuralNetworkComponents::NeuralNetwork*> brains;
int nextIndex = 0;

extern "C"
{
	class ANNManager
	{
	public:

		__declspec(dllexport) int createNewBrain_C(int inputCount, int hiddenLayerCount, int outputCount)
		{
			int index = nextIndex;

			brains[index] = new NeuralNetworkComponents::NeuralNetwork(inputCount, hiddenLayerCount, outputCount);

			nextIndex++;

			return index;
		}

		__declspec(dllexport) int createSetBrain_C(int inputCount, int hiddenLayerCount, int outputCount, float geneticSequence[])
		{
			int index = nextIndex;

			NeuralNetworkComponents::GeneticSequence *g = new NeuralNetworkComponents::GeneticSequence(inputCount, hiddenLayerCount, outputCount, geneticSequence);
			brains[index] = new NeuralNetworkComponents::NeuralNetwork(inputCount, hiddenLayerCount, outputCount, g);

			nextIndex++;

			return index;
		}

		__declspec(dllexport) void deleteBrain_C(int index)
		{
			if (brains.find(index) != brains.end())
			{
				delete brains[index];
				brains.erase(index);
			}
		}

		__declspec(dllexport) float* getGeneticSequence_C(int index, int* length)
		{
			if (brains.find(index) != brains.end())
			{
				NeuralNetworkComponents::GeneticSequence sequence = brains[index]->getGeneticSequence();

				*length = sequence.getSize();

				return sequence.toArray();
			}
		}
	};
}