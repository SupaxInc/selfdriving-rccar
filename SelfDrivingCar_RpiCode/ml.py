import random, copy, enum, numpy


class NeuralLayer:
    '''Ported version of NeuralLayer from C# library'''

    def __init__(self, activationFunction, activationFunctionDerivative, NumberOfInputs, NumberOfOutputs):
        #Initializes the NeuralLayer object from the constructor parameters:
        self.activationFunction = activationFunction
        self.activationFunctionDerivative = activationFunctionDerivative
        self.NumberOfInputs = NumberOfInputs + 1
        self.NumberOfOutputs = NumberOfOutputs
        self.Inputs = [0.0 for i in range(self.NumberOfInputs)]
        self.Outputs = [0.0 for i in range(self.NumberOfOutputs)]
        self.Weights = [[1.0 for j in range(self.NumberOfInputs)] for i in range(self.NumberOfOutputs)]

    def feedForward(self, input):
        #The data is passed from input nodes to output nodes through activation function.
        for inputIndex in range(self.NumberOfInputs - 1):
            self.Inputs[inputIndex] = input[inputIndex]
        self.Inputs[-1] = 1.0#Sets up the bias to be 1.

        for outputIndex in range(self.NumberOfOutputs):
            self.Outputs[outputIndex] = 0.0
            for inputIndex in range(self.NumberOfInputs):
                self.Outputs[outputIndex] += self.Inputs[inputIndex] * self.Weights[outputIndex][inputIndex]
            self.Outputs = list(map(self.activationFunction, self.Outputs))
        #Returns the output array.
        return self.Outputs

    def randomizeWeights(self):
        #Sets weights to be random value between -0.5 and 0.5)
        self.Weights = [[random.uniform(-0.5, 0.5) for j in range(self.NumberOfInputs)] for i in range(self.NumberOfOutputs)]
        # for outputIndex in range(self.NumberOfOutputs):
        #     for inputIndex in range(self.NumberOfInputs):
        #         self.Weights[outputIndex][inputIndex] = random.uniform(-0.5, 0.5)


class NeuralNetwork:
    '''Ported version of NeuralNetwork from C# library'''
    
    def __init__(self, activationType, layersInfo):
        #Initializes NeuralNetwork object from constructor parameters.
        self.layersInfo = copy.deepcopy(layersInfo)
        self.activationFunction = activationType[0]
        self.activationFunctionDerivative = activationType[1]
        self.Layers = []

        for layerIndex in range(len(self.layersInfo) - 1):
            self.Layers.append(NeuralLayer(self.activationFunction, self.activationFunctionDerivative, self.layersInfo[layerIndex], self.layersInfo[layerIndex + 1]))

    def guess(self, inputs):
        #The main function to predict output based on input
        self.Layers[0].feedForward(inputs)

        for layerIndex in range(1, len(self.Layers)):
            self.Layers[layerIndex].feedForward(self.Layers[layerIndex - 1].Outputs)
        #Returns the final output of the neural network.
        return self.Layers[-1].Outputs

    def randomizeWeights(self):
        #Randomizes weights of all layers in the network.
        for nl in self.Layers:
            nl.randomizeWeights()


class Species(NeuralNetwork):
    '''Ported version of Species from C# library, derived from NuronNetwork'''

    def __init__(self, activationType, layersInfo, dna = None):
        #Initialiazes Species from constructor parameters.
        NeuralNetwork.__init__(self, activationType, layersInfo)
        if dna != None:
            #If the weights/DNA is provided, replace random weights with given values.
            self.setWeightsFromDNA(dna)
        self.Fitness = 0.0

    def setWeightsFromDNA(self, dna):
        #Sets up the weights arrays in Species to match given array.
        index = 0

        for nl in self.Layers:
            for outputIndex in range(nl.NumberOfOutputs):
                for inputIndex in range(nl.NumberOfInputs):
                    nl.Weights[outputIndex][inputIndex] = dna[index]
                    index += 1

    def getWeightsAsDNA(self):
        #Returns the one-dimensional array representing all weights of neural network.
        dna = [weight for neuralLayer in self.Layers for outputNeuron in neuralLayer.Weights for weight in outputNeuron]
        # dna = []
        # for nl in self.Layers:
        #     for outputIndex in range(nl.NumberOfOutputs):
        #         for inputIndex in range(nl.NumberOfInputs):
        #             dna.append(nl.Weights[outputIndex][inputIndex])
        return dna

##
##brain = Species([lambda x: numpy.tanh(x), lambda x: x * (1 - x)], [3, 1])
##
##brain.randomizeWeights()
##dna = brain.getWeightsAsDNA()
##print(str(dna) + " " + str(len(dna)))
##
##print(brain.guess([0.5, 0.5, 0.5]))