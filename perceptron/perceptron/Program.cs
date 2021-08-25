using System;

namespace perceptron
{
    class Program
    {
        class Perceptron
        {
            double[] weights;
            double bias;
            double mutationAmount;
            Random random;
            Func<double, double, double> errorFunc;

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

            public void Randomize(double min, double max)
            {
                bias = random.NextDouble(min, max);

                for(int i = 0; i < weights.Length; i++)
                {
                    weights[i] = random.NextDouble(min, max);
                }
            }

            public double Compute(double[] inputs)
            {
                double rtrn = 0;
                
                for(int i = 0; i < weights.Length; i++)
                {
                    rtrn += weights[i] * inputs[i];
                }
                
                return rtrn + bias;
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
        }

        public static double errorFunc(double d, double a)
        {
            return Math.Pow(d - a, 2);
        }

        static void Main(string[] args)
        {
            Perceptron p = new Perceptron(2, .5, new Random(), errorFunc);

            p.Randomize(-1, 1);

            double error = int.MaxValue;

            while(error > .25)
            {
                error = p.Train(new double[][] { new double[] {0, 1}, new double[] {1, 0}, new double[] {1, 1}, new double[] {0, 0}}, new double[] {0, 0, 1, 1}, error);
                Console.WriteLine(error);
            }

            //output value of certain input to check, use Console.setCursorPosition()
        }
    }
}
