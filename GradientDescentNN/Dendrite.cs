using System;
using System.Collections.Generic;
using System.Text;

namespace GradientDescentNN
{
    class Dendrite
    {
        public Neuron previous { get; }
        public Neuron next { get; }
        public double weight { get; set; }

        public Dendrite(Neuron p, Neuron n, double weight)
        {
            previous = p;
            next = n;
            this.weight = weight;
        }
    }
}
