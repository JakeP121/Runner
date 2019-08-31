#include "stdafx.h"
#include "CalculationNode.h"

NeuralNetworkComponents::CalculationNode::CalculationNode(Node** inputNodes, int numInputs, float geneticSequence[])
{
	srand(time(NULL));

	numConnections = numInputs;
	connections = new Connection[numConnections];

	for (int i = 0; i < numConnections; i++)
	{
		connections[i].node = inputNodes[i];

		if (geneticSequence == nullptr)
			connections[i].strength = static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
		else
			connections[i].strength = geneticSequence[i];
	}

	if (geneticSequence == nullptr)
		bias = static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
	else
		bias = geneticSequence[numConnections];
}

NeuralNetworkComponents::CalculationNode::~CalculationNode()
{
	delete[] connections;
}

void NeuralNetworkComponents::CalculationNode::run()
{
	value = 0.0f;

	int numConnections = sizeof(connections) / sizeof(*connections);

	for (int i = 0; i < numConnections; i++)
		value += connections[i].node->getValue() * connections[i].strength;

	value += bias;
}

float* NeuralNetworkComponents::CalculationNode::getGeneticData()
{
	float* values = new float[numConnections + 1];

	for (int i = 0; i < numConnections; i++)
		values[i] = connections[i].strength;

	values[numConnections] = bias;

	return values;
}