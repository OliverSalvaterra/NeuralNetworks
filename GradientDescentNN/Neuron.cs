using System;
using System.Collections.Generic;
using System.Text;

namespace GradientDescentNN
{
    class Neuron
    {
        Dendrite[] dendrites;
        double bias { get; set; }

        public Neuron(double bias)
        {
            this.bias = bias;
        }

        public double calculate()
        {
            double output = 0;

            foreach(Dendrite d in dendrites)
            {
                //input layer check
                output += d.previous.calculate() * d.weight;
            }

            return output + bias;
        }
    }
}
