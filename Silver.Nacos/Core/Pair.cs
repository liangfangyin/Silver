using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core
{
    public class Pair<T>
    {
        public T Item { get; private set; }

        public double Weight { get; private set; }

        public Pair(T item, double weight)
        {
            this.Item = item;
            this.Weight = weight;
        }
    }
}
