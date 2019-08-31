#pragma once
#include "Node.h"

namespace NeuralNetworkComponents
{
	class CalculationNode : public Node
	{
		struct Connection
		{
		public:
			Node* node;
			float strength;
		};

	private:
		Connection* connections = nullptr;
		float bias = 0.5f;

		int numConnections;

	public:
		__declspec(dllexport) CalculationNode() {};
		__declspec(dllexport) CalculationNode(Node** inputNodes, int numInputs, float geneticSequence[] = nullptr);
		__declspec(dllexport) ~CalculationNode();
		__declspec(dllexport) void run();

		float* getGeneticData();

	};
}