using System;
using System.Threading;

namespace perceptron
{
    class Program
    {
        class ErrorFunction
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

        public class ActivationFunction
        {
            Func<double, double> function;
            Func<double, double> derivative;
            public ActivationFunction() 
            {
            }

            public double Function(double input) 
            {
                return 1 / (1 + Math.Pow(Math.E, -input));
            }

            public double Derivative(double input) 
            {
                double func = 1 / (1 + Math.Pow(Math.E, -input));
                return func * (1 - func);
            }
        }

        class Perceptron
        {
            double[] weights;
            double bias;
            double mutationAmount;
            double learningRate;
            Random random;
            Func<double, double, double> errorFunc;
            ActivationFunction a = new ActivationFunction();
            ErrorFunction e = new ErrorFunction();

            public Perceptron(double[] initialWeightValues, double initialBiasValue, double mutationAmount, Random random, Func<double, double, double> errorFunc)
            {
                weights = initialWeightValues;
                bias = initialBiasValue;
                this.mutationAmount = mutationAmount;
                this.random = random;
                this.errorFunc = errorFunc;
            }

            public Perceptron(int amountOfInputs, double mutationAmount, Random random, Func<double, double, double> errorFunc)
            {
                bias = 0;
                weights = new double[amountOfInputs];
                this.mutationAmount = mutationAmount;
                this.random = random;
                this.errorFunc = errorFunc;
            }

            public Perceptron(int amountOfInputs, double learningRate, Random random, Func<double, double, double> errorFunc, ActivationFunction a)
            {
                bias = 0;
                weights = new double[amountOfInputs];
                this.learningRate = learningRate;
                this.random = random;
                this.errorFunc = errorFunc;
                this.a = a;
            }

            public void Randomize(double min, double max)
            {
                bias = random.NextDouble(min, max);

                for(int i = 0; i < weights.Length; i++)
                {
                    weights[i] = random.NextDouble(min, max);
                }
            }

            public double sum(double[] inputs)
            {
                double rtrn = 0;

                for(int i = 0; i < weights.Length; i++)
                {
                    rtrn += weights[i] * inputs[i];
                }

                return rtrn + bias;
            }

            public double Compute(double[] inputs)
            {
                double rtrn = 0;
                
                for(int i = 0; i < weights.Length; i++)
                {
                    rtrn += weights[i] * inputs[i];
                }
                
                return a.Function(rtrn + bias);
            }

            public double[] Compute(double[][] inputs)
            {
                double[] rtrn = new double[inputs.Length];

                for(int i = 0; i < inputs.Length; i++)
                {
                    rtrn[i] = Compute(inputs[i]);
                }

                return rtrn;
            }

            public double getError(double[][]inputs, double[] desiredOutputs)
            {
                double[] computed = Compute(inputs);
                double error = 0;

                for(int i = 0; i < desiredOutputs.Length; i++)
                {
                    error += errorFunc(desiredOutputs[i], computed[i]);
                }

                return error / desiredOutputs.Length;
            }

            public int plusOrMinus()
            {
                return random.Next(0, 2) == 0 ? -1 : 1;
            }

            public double Train(double[][] inputs, double[] desiredOutputs, double currentError)
            {
                int nums = weights.Length + 1; //+1 for bias
                int plus = plusOrMinus();
                int whatToMutate = random.Next(0, nums);
                double mutation = random.NextDouble(-mutationAmount, mutationAmount);
                
                if(whatToMutate == nums - 1)
                {
                    bias += plus * mutation;
                }
                else
                {
                    weights[whatToMutate] += plus * mutation;
                }

                double newError = getError(inputs, desiredOutputs);

                if(currentError - newError < 0)
                {
                    if(whatToMutate == nums - 1)
                    {
                        bias -= plus * mutation;
                    }
                    else
                    {
                        weights[whatToMutate] -= plus * mutation;
                    }
                    
                    newError = currentError;
                }

                return newError;
            }

            public double GDTrain(double[] inputs, double desiredOutput)
            {
                double z = sum(inputs);
                double o = Compute(inputs);
                for(int i = 0; i < inputs.Length; i++)
                {
                    weights[i] += learningRate * -(e.Derivative(o, desiredOutput) * a.Derivative(z)*inputs[i]);
                }
                bias += learningRate * -(e.Derivative(o, desiredOutput) * a.Derivative(z));

                return e.Function(Compute(inputs), desiredOutput);
            }

            public double GDTrain(double[][] inputs, double[] desiredOutputs)
            {
                double[] changes = new double[weights.Length + 1];
                
                for(int j = 0; j < inputs.Length; j++)
                {
                    double z = sum(inputs[j]);
                    double o = Compute(inputs[j]);
                    for (int i = 0; i < inputs[j].Length; i++)
                    {
                        changes[i] += learningRate * -(e.Derivative(o, desiredOutputs[j]) * a.Derivative(z) * inputs[j][i]);
                    }
                    changes[changes.Length - 1] += learningRate * -(e.Derivative(o, desiredOutputs[j]) * a.Derivative(z));
                }

                for(int i = 0; i < changes.Length; i++)
                {
                    if(i != changes.Length - 1)
                    {
                        weights[i] += changes[i];
                    }
                    else
                    {
                        bias += changes[i];
                    }
                }

                double rtrn = 0;
                for(int i = 0; i < inputs.Length; i++)
                {
                    rtrn += e.Function(Compute(inputs[i]), desiredOutputs[i]);
                }

                return rtrn / inputs.Length;
            }
        }
        //change to learningRate*-partialDerivative for train function

        public static double errorFunc(double d, double a)
        {
            return Math.Pow(d - a, 2);
        }

        static void Main(string[] args)
        {
            //Perceptron p = new Perceptron(2, .1, new Random(), errorFunc); hill climber neuron

            Perceptron p = new Perceptron(2, .05, new Random(), errorFunc, new ActivationFunction());

            p.Randomize(-1, 1);

            double error = int.MaxValue;

            var inputs = new double[][] { new double[] {0, 1}, new double[] {1, 0}, new double[] {0, 0}, new double[] {1, 1} };

            var expected = new double[] { 0, 0, 0, 1 };

            while (error > .02)
            {
                Console.SetCursorPosition(0, 0);
                error = p.GDTrain(inputs, expected);

                Console.WriteLine($"Error: {error}");

                var actual = p.Compute(inputs);
                for (int i = 0; i < actual.Length; i++)
                {
                    Console.WriteLine($"Item {i + 1}");

                    Console.WriteLine($"\tActual: {actual[i]}");
                    Console.WriteLine($"\tExpected: {expected[i]}");
                }
                Thread.Sleep(10);
            }
            

            //output value of certain input to check, use Console.setCursorPosition()
        }
    }
}
