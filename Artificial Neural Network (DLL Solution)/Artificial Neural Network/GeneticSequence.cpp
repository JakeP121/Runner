#include "stdafx.h"
#include "GeneticSequence.h"

void NeuralNetworkComponents::GeneticSequence::storeSequence(float sequence[])
{
	for (std::vector<Layer>::iterator i = layers.begin(); i != layers.end(); i++)
	{
		for (std::vector<float*>::iterator j = (*i).nodeSequences.begin(); j != (*i).nodeSequences.end(); j++)
		{
			delete[](*j);
		}
	}

	layers.clear();

	int sequenceLength = sizeof(sequence) / sizeof(*sequence);
	int sequenceIndex = 0;

	for (int i = 0; i < hiddenLayerCount; i++)
	{
		Layer l;

		for (int j = 0; j < inputCount; j++)
		{
			float* newSequence = new float[sequenceLength];

			for (int k = 0; k < sequenceLength; k++)
			{
				newSequence[k] = sequence[sequenceIndex];
				sequenceIndex++;
			}

			l.nodeSequences.push_back(newSequence);
		}

		layers.push_back(l);
	}


	Layer outputLayer;

	for (int i = 0; i < outputCount; i++)
	{
		float* newSequence = new float[sequenceLength];

		for (int j = 0; j < sequenceLength; j++)
		{
			newSequence[j] = sequence[sequenceIndex];
			sequenceIndex++;
		}

		outputLayer.nodeSequences.push_back(newSequence);
	}

	layers.push_back(outputLayer);
}

NeuralNetworkComponents::GeneticSequence::GeneticSequence(GeneticSequence & aSeq, GeneticSequence bSeq, const int & mutations)
{
	srand(time(NULL));

	float* a = aSeq.toArray();
	float* b = bSeq.toArray();

	int aLength = sizeof(a) / sizeof(*a);
	int bLength = sizeof(b) / sizeof(*b);

	if (aLength != bLength)
	{
		// Parents do not have compatible genetic sequences
		return;
	}

	this->inputCount = aSeq.inputCount;
	this->hiddenLayerCount = aSeq.hiddenLayerCount;
	this->outputCount = aSeq.outputCount;

	float* mine = new float[aLength];

	for (int i = 0; i < aLength; i++)
	{
		if (static_cast<float>(rand()) / static_cast<float>(RAND_MAX) > 0.5f) // If Random Value (between 0.0 and 1.0) is greater than 0.5
			mine[i] = a[i];
		else
			mine[i] = b[i];
	}

	for (int i = 0; i < mutations; i++)
	{
		int index = rand() % aLength;
		mine[index] = static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
	}

	storeSequence(mine);
}

void NeuralNetworkComponents::GeneticSequence::addLayer(CalculationNode** nodes, int numNodes)
{
	Layer l;

	for (int i = 0; i < numNodes; i++)
		l.nodeSequences.push_back(nodes[i]->getGeneticData());

	layers.push_back(l);
}

float* NeuralNetworkComponents::GeneticSequence::toArray()
{
	float* arr = new float[size];

	int index = 0;

	// Itereate through hidden layers
	for (int i = 0; i < layers.size() - 1; i++)
	{
		// Iterate through nodes in layer
		for (int j = 0; j < layers[i].nodeSequences.size(); j++)
		{
			// Iterate through node's connections and bias
			for (int k = 0; k < inputCount + 1; k++)
			{
				arr[index] = layers[i].nodeSequences[j][k];
				index++;
			}
		}
	}

	// Itereate through output layer's nodes
	for (int i = 0; i < layers[layers.size() - 1].nodeSequences.size(); i++)
	{
		// Iterate through node's connections and bias
		for (int j = 0; j < inputCount + 1; j++)
		{
			arr[index] = layers[layers.size() - 1].nodeSequences[i][j];
			index++;
		}
	}

	return arr;
}

SAFEARRAY* NeuralNetworkComponents::GeneticSequence::toSafeArray()
{
	SAFEARRAY* safeSequence = new SAFEARRAY();
	float* data = toArray();
	SAFEARRAYBOUND  Bound;
	Bound.lLbound = 0;
	Bound.cElements = size;

	safeSequence = SafeArrayCreate(VT_R8, 1, &Bound);

	double HUGEP *pdFreq;
	HRESULT hr = SafeArrayAccessData(safeSequence, (void HUGEP* FAR*)&pdFreq);
	if (SUCCEEDED(hr))
	{
		for (DWORD i = 0; i < size; i++)
			*pdFreq++ = data[i];

		SafeArrayUnaccessData(safeSequence);
	}

	return safeSequence;
}

float* NeuralNetworkComponents::GeneticSequence::getNodeSequence(const int & layer, const int & index)
{
	if (layer >= hiddenLayerCount + 1 || index >= inputCount)
	{
		// Invalid layer or index
		return nullptr;
	}

	return layers[layer].nodeSequences[index];
}

int NeuralNetworkComponents::GeneticSequence::getSize()
{
	return size;
}
