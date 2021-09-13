using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class ErrorFunction
    {
        Func<double, double, double> function;
        Func<double, double, double> derivative;
        public ErrorFunction(Func<double, double, double> function, Func<double, double, double> derivative)
        {
            this.function = function;
            this.derivative = derivative;
        }

        public ErrorFunction()
        {

        }

        public double Function(double a, double d)
        {
            return Math.Pow(d - a, 2);
        }
        public double Derivative(double a, double d)
        {
            return -2 * (d - a);
        }
    }

    public class NeuralNetwork
    {
        public Layer[] layers;
        ErrorFunction errorFunc;

        public NeuralNetwork(params int[] neuronsPerLayer)
        {
            errorFunc = new ErrorFunction();
            layers = new Layer[neuronsPerLayer.Length];

            layers[0] = new Layer(neuronsPerLayer[0], null);
            for(int i = 1; i < layers.Length; i++)
            {
                layers[i] = new Layer(neuronsPerLayer[i], layers[i - 1]);
            }
        }
        public void Randomize(Random random, double min, double max) 
        {
            foreach(Layer l in layers)
            {
                l.Randomize(random, min, max);
            }
        }
        public double[] Compute(double[] inputs) 
        {
            double[] outputs = new double[layers[layers.Length - 1].Neurons.Length];
            
            for(int i = 0; i < layers[0].Neurons.Length; i++)
            {
                layers[0].Neurons[i].Output = inputs[i];
            }

            for(int i = 1; i < layers.Length; i++)
            {
                if(i >= layers.Length - 1)
                {
                    outputs = layers[i].Compute();
                }
                else
                {
                    layers[i].Compute();
                }
            }

            return outputs;
        }
        public double GetError(double[] inputs, double[] desiredOutputs) 
        {
            double rtrn = 0;
            double[] a = Compute(inputs);

            for(int i = 0; i < desiredOutputs.Length; i++)
            {
                rtrn += Math.Abs(desiredOutputs[i] - a[i]);
            }

            return rtrn / desiredOutputs.Length;
        }
    }

    public class GeneticTrain
    {
        Func<double[], double> f;
        (NeuralNetwork, double)[] networks;

        public GeneticTrain(Func<double[], double> f, int[] networkSize, int batchSize)
        {
            this.f = f;
            networks = new (NeuralNetwork, double)[batchSize];

            for(int i = 0; i < batchSize; i++)
            {
                networks[i] = (new NeuralNetwork(networkSize), default);
            }
        }

        public void Mutate(NeuralNetwork n, Random random, double mutationRate)
        {
            foreach (Layer layer in n.layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    //Mutate the Weights
                    for (int i = 0; i < neuron.dendrites.Length; i++)
                    {
                        if (random.NextDouble() < mutationRate)
                        {
                            if (random.Next(2) == 0)
                            {
                                neuron.dendrites[i].Weight *= random.NextDouble(0.5, 1.5); //scale weight
                            }
                            else
                            {
                                neuron.dendrites[i].Weight *= -1; //flip sign
                            }
                        }
                    }

                    //Mutate the Bias
                    if (random.NextDouble() < mutationRate)
                    {
                        if (random.Next(2) == 0)
                        {
                            neuron.bias *= random.NextDouble(0.5, 1.5); //scale weight
                        }
                        else
                        {
                            neuron.bias *= -1; //flip sign
                        }
                    }
                }
            }
        }

        public void Crossover(NeuralNetwork winner, NeuralNetwork loser, Random random)
        {
            for (int i = 0; i < winner.layers.Length; i++)
            {
                //References to the Layers
                Layer winLayer = winner.layers[i];
                Layer childLayer = loser.layers[i];

                int cutPoint = random.Next(winLayer.Neurons.Length); //calculate a cut point for the layer
                bool flip = random.Next(2) == 0; //randomly decide which side of the cut point will come from winner

                //Either copy from 0->cutPoint or cutPoint->Neurons.Length from the winner based on the flip variable
                for (int j = (flip ? 0 : cutPoint); j < (flip ? cutPoint : winLayer.Neurons.Length); j++)
                {
                    //References to the Neurons
                    Neuron winNeuron = winLayer.Neurons[j];
                    Neuron childNeuron = childLayer.Neurons[j];

                    //Copy the winners Weights and Bias into the loser/child neuron
                    for(int n = 0; n < winNeuron.dendrites.Length; i++)
                    {
                        childNeuron.dendrites[n].Weight = winNeuron.dendrites[n].Weight;
                    }
                    childNeuron.bias = winNeuron.bias;
                }
            }
        }

        public void Train((NeuralNetwork net, double fitness)[] population, Random random, double mutationRate)
        {
            Array.Sort(population, (a, b) => b.fitness.CompareTo(a.fitness));

            int start = (int)(population.Length * 0.1);
            int end = (int)(population.Length * 0.9);

            //Notice that this process is only called on networks in the middle 80% of the array
            for (int i = start; i < end; i++)
            {
                Crossover(population[random.Next(start)].net, population[i].net, random);
                Mutate(population[i].net, random, mutationRate);
            }

            //Removes the worst performing networks
            for (int i = end; i < population.Length; i++)
            {
                population[i].net.Randomize(random, 0, 1);
            }
        }

        public void SetFitness()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
