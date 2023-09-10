using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core
{
    public class Ref<T>
    {
        public List<Pair<T>> ItemsWithWeight = new List<Pair<T>>();

        public List<T> Items = new List<T>();

        public IPoller<T> Poller;

        public double[] Weights;

        public Ref(List<Pair<T>> itemsWithWeight)
        {
            this.ItemsWithWeight = itemsWithWeight;
            this.Poller = new GenericPoller<T>(Items);
        }

        public void Refresh()
        {
            double originWeightSum = 0d;

            foreach (Pair<T> item in ItemsWithWeight)
            {
                double weight = item.Weight;

                // ignore item which weight is zero
                if (weight <= 0) continue;

                Items.Add(item.Item);

                if (double.IsInfinity(weight)) weight = 10000.0D;

                if (double.IsNaN(weight)) weight = 1.0D;

                originWeightSum += weight;
            }

            double[] exactWeights = new double[Items.Count];
            int index = 0;
            foreach (Pair<T> item in ItemsWithWeight)
            {
                double singleWeight = item.Weight;

                // ignore item which weight is zero.
                if (singleWeight <= 0) continue;

                exactWeights[index++] = singleWeight / originWeightSum;
            }

            Weights = new double[Items.Count];
            double randomRange = 0D;
            for (int i = 0; i < index; i++)
            {
                Weights[i] = randomRange + exactWeights[i];
                randomRange += exactWeights[i];
            }

            double doublePrecisionDelta = 0.0001;

            if (index == 0 || (Math.Abs(Weights[index - 1] - 1) < doublePrecisionDelta)) return;

            throw new ArgumentOutOfRangeException("Cumulative Weight caculate wrong , the sum of probabilities does not equals 1.");
        }

        public override int GetHashCode()
        {
            return ItemsWithWeight.GetHashCode();
        }
    }

    public interface IPoller<T>
    {
        T Next();

        IPoller<T> Refresh(List<T> items);
    }

    public class GenericPoller<T> : IPoller<T>
    {
        private int index = 0;

        private List<T> items = new List<T>();

        public GenericPoller(List<T> items)
        {
            this.items = items;
        }

        public T Next()
        {
            System.Threading.Interlocked.Increment(ref index);

            return items[System.Math.Abs(index % items.Count)];
        }

        public IPoller<T> Refresh(List<T> items)
        {
            return new GenericPoller<T>(items);
        }
    }


}
