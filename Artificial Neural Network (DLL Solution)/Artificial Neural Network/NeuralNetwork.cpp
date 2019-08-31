#include "stdafx.h"
#include "NeuralNetwork.h"

void NeuralNetworkComponents::NeuralNetwork::createNetwork(GeneticSequence * geneticSequence)
{
	inputNodes = new InputNode*[inputCount];

	for (int i = 0; i < inputCount; i++)
		inputNodes[i] = new InputNode();

	Node** layerInputs = new Node*[inputCount];

	for (int i = 0; i < inputCount; i++)
		layerInputs[i] = inputNodes[i];

	for (int i = 0; i < hiddenLayerCount; i++)
	{
		CalculationNode** layer = new CalculationNode*[inputCount];

		for (int j = 0; j < inputCount; j++)
		{
			if (geneticSequence == nullptr)
				layer[j] = new CalculationNode(layerInputs, inputCount);
			else
				layer[j] = new CalculationNode(layerInputs, inputCount, geneticSequence->getNodeSequence(i, j));
		}

		hiddenLayers.push_back(layer);

		for (int i = 0; i < inputCount; i++)
			layerInputs[i] = layer[i];
	}

	outputNodes = new CalculationNode*[outputCount];

	for (int i = 0; i < outputCount; i++)
	{
		if (geneticSequence == nullptr)
			outputNodes[i] = new CalculationNode(layerInputs, inputCount);
		else
			outputNodes[i] = new CalculationNode(layerInputs, inputCount, geneticSequence->getNodeSequence(hiddenLayerCount, i));
	}
}

void NeuralNetworkComponents::NeuralNetwork::run()
{
	for (std::vector<CalculationNode**>::iterator i = hiddenLayers.begin(); i != hiddenLayers.end(); i++)
	{
		for (int j = 0; j < inputCount; j++)
			(*i)[j]->run();
	}

	for (int i = 0; i < outputCount; i++)
		outputNodes[i]->run();

	needsRecalculation = false;
}

NeuralNetworkComponents::NeuralNetwork::~NeuralNetwork()
{
	delete[] inputNodes;
	delete[] outputNodes;

	for (std::vector<CalculationNode**>::iterator i = hiddenLayers.begin(); i != hiddenLayers.end(); i++)
		delete[](*i);

	hiddenLayers.clear();
}

void NeuralNetworkComponents::NeuralNetwork::setInput(const int & index, const float & value)
{
	if (index >= inputCount)
	{
		// Invalid index
		return;
	}

	inputNodes[index]->setValue(value);
	needsRecalculation = true;
}

float NeuralNetworkComponents::NeuralNetwork::getOutput(const int & index)
{
	if (index >= outputCount)
	{
		// Invalid index
		return -999.0f;
	}

	if (needsRecalculation)
		run();

	return outputNodes[index]->getValue();
}

NeuralNetworkComponents::GeneticSequence NeuralNetworkComponents::NeuralNetwork::getGeneticSequence()
{
	GeneticSequence sequence = GeneticSequence(inputCount, hiddenLayerCount, outputCount);

	for (std::vector<CalculationNode**>::iterator i = hiddenLayers.begin(); i != hiddenLayers.end(); i++)
		sequence.addLayer((*i), inputCount);

	sequence.addLayer(outputNodes, outputCount);

	return sequence;
}