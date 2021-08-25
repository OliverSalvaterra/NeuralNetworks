using System;
using System.Collections.Generic;
using System.Text;

namespace perceptron
{
    static class Extensions
    {
        public static double NextDouble(this Random rnd, double min, double max)
        {
            return (rnd.NextDouble() * (max - min) + min);
        }
    }
}
