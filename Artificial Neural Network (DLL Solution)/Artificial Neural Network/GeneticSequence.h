#pragma once
#include "GeneticSequence.h"
#include "CalculationNode.h"
#include <vector>
#include <OAIdl.h>

namespace NeuralNetworkComponents
{
	class GeneticSequence
	{
		struct Layer
		{
		public:
			std::vector<float*> nodeSequences;
		};

	private:
		std::vector<Layer> layers;
		int inputCount;
		int hiddenLayerCount;
		int outputCount;

		int size;

		__declspec(dllexport) void storeSequence(float sequence[]);

	public:
		__declspec(dllexport) GeneticSequence(const int& inputCount, const int& hiddenLayerCount, const int& outputCount, float sequence[] = nullptr) : inputCount(inputCount), hiddenLayerCount(hiddenLayerCount), outputCount(outputCount)
		{
			if (sequence != nullptr)
				storeSequence(sequence);

			size = hiddenLayerCount * (inputCount * inputCount + inputCount) + outputCount * inputCount + outputCount;
		};

		__declspec(dllexport) GeneticSequence(GeneticSequence& aSeq, GeneticSequence bSeq, const int& mutations = 0);
		__declspec(dllexport) void addLayer(CalculationNode** nodes, int numNodes);
		__declspec(dllexport) float* toArray();
		__declspec(dllexport) SAFEARRAY* toSafeArray();
		__declspec(dllexport) float* getNodeSequence(const int& layer, const int& index);

		__declspec(dllexport) int getSize();
	};
}