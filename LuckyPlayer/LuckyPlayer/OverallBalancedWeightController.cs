using System;
using System.Collections.Generic;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace JLChnToZ.LuckyPlayer {
    /// <summary>
    /// An overall-balanced weight controller which should ensure an item to be appear in certain times of gacha.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This implementation is still experimental, not 100% guarantee to be working.
    /// </remarks>
    public class OverallBalancedWeightController<T>: ISuccessCallback<T> {
        class WeightMap {
            public readonly double orignalWeight;
            public double balancedWeight;

            public WeightMap(double weight) {
                orignalWeight = weight;
                balancedWeight = weight;
            }
        }

        double commonDivisor = 0;
        double sum = 0;
        bool dirtyDivisor = false;
        readonly Dictionary<T, WeightMap> weights = new Dictionary<T, WeightMap>();

        /// <summary>
        /// Get or set the original weight of an item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The weight</returns>
        public double this[T item] {
            get {
                WeightMap weight;
                return weights.TryGetValue(item, out weight) ? weight.orignalWeight : 0;
            }
            set {
                WeightMap weight;
                if(weights.TryGetValue(item, out weight) && weight.orignalWeight == value)
                    return;
                weights[item] = new WeightMap(value);
                dirtyDivisor = true;
            }
        }

        /// <summary>
        /// Clear all weight mapping in this instance of controller.
        /// </summary>
        public void Clear() {
            weights.Clear();
            commonDivisor = 0;
            sum = 0;
            dirtyDivisor = false;
        }

        void CalculateGCD() {
            if(!dirtyDivisor) return;
            sum = commonDivisor = 0;
            foreach(var weight in weights.Values) {
                sum += weight.orignalWeight;
                if(commonDivisor == 0) {
                    commonDivisor = weight.orignalWeight;
                    continue;
                }
                commonDivisor = CalculateGCD(commonDivisor, weight.orignalWeight);
            }
            dirtyDivisor = false;
        }

        static double CalculateGCD(double a, double b) {
            if(a == 0) return b;
            if(b == 0) return a;
            double remainder;
            while(Math.Abs(b) > double.Epsilon) {
                remainder = a % b;
                a = b;
                b = remainder;
            }
            return Math.Abs(a);
        }

        double IItemWeight<T>.GetWeight(T item) {
            CalculateGCD();
            WeightMap weight;
            if(!weights.TryGetValue(item, out weight)) return 0;
            double result = weight.balancedWeight;
            weight.balancedWeight += weight.orignalWeight * commonDivisor / sum;
            return result;
        }
        
        void ISuccessCallback<T>.OnSuccess(T item) {
            WeightMap weight;
            if(!weights.TryGetValue(item, out weight)) return;
            weight.balancedWeight = weight.orignalWeight * commonDivisor / sum;
        }
    }
}
