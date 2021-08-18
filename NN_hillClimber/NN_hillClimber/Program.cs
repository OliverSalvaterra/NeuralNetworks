using System;

namespace NN_hillClimber
{
    class Program
    {
        public static double error(string input, string actual)
        {
            double total = 0;
            int l = actual.Length;
            for(int i = 0; i < l; i++)
            {
                total += Math.Abs(actual[i] - input[i]);
            }
            return total / l;
        }

        public static string rndString(int l)
        {
            string rtrn = "";
            Random rnd = new Random();

            for(int i = 0; i < l; i++)
            {
                rtrn += (char)rnd.Next(32, 127);
            }

            return rtrn;
        }

        public static string mutate(string input)
        {
            Random rnd = new Random();
            string output = "";
            int l = input.Length;
            int rand = rnd.Next(0, l);

            for (int i = 0; i < l; i++)
            {
                if(i == rand)
                {
                    output += (char)(input[i] + plusOrMinus());
                }
                else
                {
                    output += input[i];
                }
            }

            return output;
        }

        public static int plusOrMinus()
        {
            Random rnd = new Random();

            return rnd.Next(0, 2) == 0 ? -1 : 1;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Input a target string");
            string target = Console.ReadLine();

            string mutater = rndString(target.Length);
            double stringError = error(mutater, target);

            if(stringError != 0.0)
            {
                while (true)
                {
                    string mutation = mutate(mutater);
                    double newError = error(mutation, target);
                    
                    if(newError < stringError)
                    {
                        stringError = newError;
                        mutater = mutation;
                        Console.WriteLine(mutater + " != " + target + " ; (" + stringError + ")");
                    }
                    else if(newError == 0.0)
                    {
                        break;
                    }
                }
            }

            Console.WriteLine(target + " = " + target + " ; (0.0)");
        }
    }
}
