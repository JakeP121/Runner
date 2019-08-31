#pragma once
#include "Node.h"

namespace NeuralNetworkComponents
{
	class InputNode : public Node
	{
	public:
		__declspec(dllexport) void setValue(const float& value)
		{
			this->value = value;
		}
	};
}