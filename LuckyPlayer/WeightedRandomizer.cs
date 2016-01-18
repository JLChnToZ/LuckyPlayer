using System;
using System.Collections;
using System.Collections.Generic;

namespace JLChnToZ.LuckyPlayer.WeightedRandomizer {
    public interface IItemWeight<T> {
        double GetWeight(T item);
    }

    sealed class FixedItemWeight<T>: IItemWeight<T> {
        readonly double weight;

        internal FixedItemWeight(double weight) {
            this.weight = weight;
        }

        public double GetWeight(T item) {
            return weight;
        }
    }

    public class ItemWeight<T>: IItemWeight<T> {
        double weight;
        public double Weight {
            get { return weight; }
            set { weight = value; }
        }

        public ItemWeight() { weight = 1; }
        public ItemWeight(double weight) { this.weight = weight; }

        double IItemWeight<T>.GetWeight(T item) {
            return weight;
        }
    }

    sealed class ItemWeightCollection<T>: ICollection<double> {
        readonly WeightedCollection<T> parent;

        internal ItemWeightCollection(WeightedCollection<T> parent) {
            this.parent = parent;
        }

        public int Count {
            get { return parent.baseDict.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public void Add(double item) {
            throw new NotSupportedException();
        }

        public bool Contains(double item) {
            foreach(var kv in parent.baseDict)
                if(kv.Value.GetWeight(kv.Key) == item) return true;
            return false;
        }

        public bool Remove(double item) {
            throw new NotSupportedException();
        }

        public void Clear() {
            throw new NotSupportedException();
        }

        public void CopyTo(double[] array, int arrayIndex) {
            foreach(var kv in parent.baseDict)
                array[arrayIndex++] = kv.Value.GetWeight(kv.Key);
        }

        public IEnumerator<double> GetEnumerator() {
            foreach(var kv in parent.baseDict)
                yield return kv.Value.GetWeight(kv.Key);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public class WeightedCollection<T>: ICollection<T>, IDictionary<T, IItemWeight<T>>, IDictionary<T, double>, ICloneable {
        static readonly Random defaultRandomizer = new Random();
        internal readonly Dictionary<T, IItemWeight<T>> baseDict;

        #region Constructors
        public WeightedCollection() {
            baseDict = new Dictionary<T, IItemWeight<T>>();
        }

        public WeightedCollection(int capacity) {
            baseDict = new Dictionary<T, IItemWeight<T>>(capacity);
        }

        public WeightedCollection(IEqualityComparer<T> comparer) {
            baseDict = new Dictionary<T, IItemWeight<T>>(comparer);
        }

        public WeightedCollection(int capacity, IEqualityComparer<T> comparer) {
            baseDict = new Dictionary<T, IItemWeight<T>>(capacity, comparer);
        }

        public WeightedCollection(int capacity, IEnumerable<T> source) : this(capacity) {
            AddRange(source);
        }

        public WeightedCollection(int capacity, IEqualityComparer<T> comparer, IEnumerable<T> source) : this(capacity, comparer) {
            AddRange(source);
        }

        private WeightedCollection(IDictionary<T, IItemWeight<T>> clone) {
            baseDict = new Dictionary<T, IItemWeight<T>>(clone);
        }
        #endregion

        #region Interface Methods
        IItemWeight<T> IDictionary<T, IItemWeight<T>>.this[T key] {
            get { return GetWeight(key); }
            set { SetWeight(key, value); }
        }

        double IDictionary<T, double>.this[T key] {
            get { return GetCurrentWeight(key); }
            set { SetWeight(key, value); }
        }

        public int Count {
            get { return baseDict.Count; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        bool ICollection<KeyValuePair<T, double>>.IsReadOnly {
            get { return false; }
        }

        bool ICollection<KeyValuePair<T, IItemWeight<T>>>.IsReadOnly {
            get { return false; }
        }

        ICollection<T> IDictionary<T, double>.Keys {
            get { return baseDict.Keys; }
        }

        ICollection<T> IDictionary<T, IItemWeight<T>>.Keys {
            get { return baseDict.Keys; }
        }

        ICollection<double> IDictionary<T, double>.Values {
            get { return new ItemWeightCollection<T>(this); }
        }

        ICollection<IItemWeight<T>> IDictionary<T, IItemWeight<T>>.Values {
            get { return baseDict.Values; }
        }

        public void Add(T item) {
            baseDict.Add(item, new FixedItemWeight<T>(1));
        }

        public void Add(T item, double weight) {
            baseDict.Add(item, new FixedItemWeight<T>(weight));
        }

        public void Add(T item, IItemWeight<T> weight) {
            baseDict.Add(item, weight ?? new FixedItemWeight<T>(0));
        }

        void ICollection<KeyValuePair<T, double>>.Add(KeyValuePair<T, double> item) {
            baseDict.Add(item.Key, new FixedItemWeight<T>(item.Value));
        }

        void ICollection<KeyValuePair<T, IItemWeight<T>>>.Add(KeyValuePair<T, IItemWeight<T>> item) {
            baseDict.Add(item.Key, item.Value ?? new FixedItemWeight<T>(0));
        }

        public bool Contains(T item) {
            return baseDict.ContainsKey(item);
        }

        bool ICollection<KeyValuePair<T, double>>.Contains(KeyValuePair<T, double> item) {
            return baseDict.ContainsKey(item.Key);
        }

        bool ICollection<KeyValuePair<T, IItemWeight<T>>>.Contains(KeyValuePair<T, IItemWeight<T>> item) {
            return baseDict.ContainsKey(item.Key);
        }

        bool IDictionary<T, double>.ContainsKey(T key) {
            return baseDict.ContainsKey(key);
        }

        bool IDictionary<T, IItemWeight<T>>.ContainsKey(T key) {
            return baseDict.ContainsKey(key);
        }

        bool IDictionary<T, double>.TryGetValue(T key, out double value) {
            IItemWeight<T> rawValue;
            if(baseDict.TryGetValue(key, out rawValue) && rawValue != null) {
                value = rawValue.GetWeight(key);
                return true;
            }
            value = 0;
            return false;
        }

        bool IDictionary<T, IItemWeight<T>>.TryGetValue(T key, out IItemWeight<T> value) {
            return baseDict.TryGetValue(key, out value) && value != null;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            baseDict.Keys.CopyTo(array, arrayIndex);
        }

        void ICollection<KeyValuePair<T, double>>.CopyTo(KeyValuePair<T, double>[] array, int arrayIndex) {
            foreach(var kv in IterateAsStaticWeight()) array[arrayIndex++] = kv;
        }

        void ICollection<KeyValuePair<T, IItemWeight<T>>>.CopyTo(KeyValuePair<T, IItemWeight<T>>[] array, int arrayIndex) {
            (baseDict as IDictionary<T, IItemWeight<T>>).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return baseDict.Remove(item);
        }

        bool ICollection<KeyValuePair<T, double>>.Remove(KeyValuePair<T, double> item) {
            return baseDict.Remove(item.Key);
        }

        bool ICollection<KeyValuePair<T, IItemWeight<T>>>.Remove(KeyValuePair<T, IItemWeight<T>> item) {
            return baseDict.Remove(item.Key);
        }

        public void Clear() {
            baseDict.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            return baseDict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return baseDict.Keys.GetEnumerator();
        }

        IEnumerator<KeyValuePair<T, IItemWeight<T>>> IEnumerable<KeyValuePair<T, IItemWeight<T>>>.GetEnumerator() {
            return baseDict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<T, double>> IEnumerable<KeyValuePair<T, double>>.GetEnumerator() {
            return IterateAsStaticWeight().GetEnumerator();
        }

        public object Clone() {
            return new WeightedCollection<T>(baseDict);
        }
        #endregion

        public void AddRange(IEnumerable<T> items) {
            foreach(var item in items) baseDict[item] = new FixedItemWeight<T>(1);
        }

        public void RemoveRange(IEnumerable<T> items) {
            foreach(var item in items) baseDict.Remove(item);
        }

        IEnumerable<KeyValuePair<T, double>> IterateAsStaticWeight() {
            foreach(var kv in baseDict)
                yield return new KeyValuePair<T, double>(kv.Key, kv.Value.GetWeight(kv.Key));
        }

        public IItemWeight<T> GetWeight(T item) {
            IItemWeight<T> weight;
            return baseDict.TryGetValue(item, out weight) && weight != null ? weight : new FixedItemWeight<T>(0);
        }

        public double GetCurrentWeight(T item) {
            IItemWeight<T> weight;
            return baseDict.TryGetValue(item, out weight) && weight != null ? weight.GetWeight(item) : 0;
        }

        public bool SetWeight(T item, IItemWeight<T> weight) {
            if(!baseDict.ContainsKey(item)) return false;
            baseDict[item] = weight ?? new FixedItemWeight<T>(0);
            return true;
        }

        public bool SetWeight(T item, double weight) {
            IItemWeight<T> weightRaw;
            if(!baseDict.TryGetValue(item, out weightRaw)) return false;
            if(weightRaw != null) {
                var flexibleWeight = weightRaw as ItemWeight<T>;
                if(flexibleWeight != null) {
                    flexibleWeight.Weight = weight;
                    return true;
                }
            }
            baseDict[item] = new FixedItemWeight<T>(weight);
            return true;
        }

        public T GetRandomItem(Random random = null) {
            int i = 0, count = baseDict.Count;
            if(count < 1) return default(T);
            double totalWeight = 0, countedWeight = 0, randomValue;
            var tempList = new KeyValuePair<T, double>[count];
            foreach(var kv in IterateAsStaticWeight()) {
                tempList[i++] = kv;
                totalWeight += kv.Value;
            }
            if(count == 1) return tempList[0].Key;
            randomValue = (random ?? defaultRandomizer).NextDouble() * totalWeight;
            for(i = 0; i < count; i++) {
                countedWeight += tempList[i].Value;
                if(countedWeight > randomValue)
                    return tempList[i].Key;
            }
            return tempList[count - 1].Key;
        }
    }
}