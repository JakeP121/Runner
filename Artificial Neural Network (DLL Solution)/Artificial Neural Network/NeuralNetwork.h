#pragma once
#include "InputNode.h"
#include "CalculationNode.h"
#include "GeneticSequence.h"
#include <vector>

namespace NeuralNetworkComponents
{
	class NeuralNetwork
	{
	private:
		int inputCount;
		int hiddenLayerCount;
		int outputCount;

		InputNode** inputNodes;
		std::vector<CalculationNode**> hiddenLayers;
		CalculationNode** outputNodes;

		bool needsRecalculation = true;

		__declspec(dllexport) void createNetwork(GeneticSequence* geneticSequence = nullptr);
		__declspec(dllexport) void run();

	public:
		__declspec(dllexport) NeuralNetwork(const int& inputCount, const int& hiddenLayerCount, const int& outputCount, GeneticSequence* geneticSequence = nullptr) : inputCount(inputCount), hiddenLayerCount(hiddenLayerCount), outputCount(outputCount)
		{
			if (geneticSequence == nullptr)
				createNetwork(geneticSequence);

			delete geneticSequence; // No longer any need to hold pointer, data copied into neuron strengths and bias
		}

		__declspec(dllexport) ~NeuralNetwork();
		__declspec(dllexport) void setInput(const int& index, const float& value);
		__declspec(dllexport) float getOutput(const int& index = 0);
		__declspec(dllexport) GeneticSequence getGeneticSequence();
	};
}