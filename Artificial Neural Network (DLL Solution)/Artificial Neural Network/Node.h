#pragma once

namespace NeuralNetworkComponents
{
	class Node
	{
	protected:
		float value = 0.0f;

	public:
		virtual __declspec(dllexport) float getValue() { return value; };
	};
}