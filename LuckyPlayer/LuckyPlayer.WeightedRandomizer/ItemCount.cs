using System;
using System.Collections;
using System.Collections.Generic;

namespace JLChnToZ.LuckyPlayer.WeightedRandomizer {
    /// <summary>
    /// Represents an item with a specified amount
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ItemCount<T>: ICollection<T>, IEquatable<ItemCount<T>> {
        class Enumerator: IEnumerator<T> {
            readonly ItemCount<T> parent;
            int index;

            public Enumerator(ItemCount<T> parent) {
                this.parent = parent;
                Reset();
            }

            public T Current {
                get { return parent.item; }
            }

            object IEnumerator.Current {
                get { return parent.item; }
            }

            public bool MoveNext() {
                return ++index < parent.count;
            }

            public void Reset() {
                index = -1;
            }

            void IDisposable.Dispose() { }
        }

        readonly T item;
        readonly int count;

        internal ItemCount(T item, int count) {
            this.item = item;
            this.count = count;
        }

        /// <summary>
        /// The item
        /// </summary>
        public T Item {
            get { return item; }
        }

        /// <summary>
        /// How many items?
        /// </summary>
        public int Count {
            get { return count; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        void ICollection<T>.Add(T item) {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item) {
            return this.item.Equals(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            if(array == null)
                throw new ArgumentNullException("array");
            for(int i = arrayIndex, l = Math.Min(array.Length, arrayIndex + count); i < l; i++)
                array[i] = item;
        }

        /// <summary>
        /// Gets an enumerator that will yields the same item with specified times.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<T> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        /// <summary>
        /// Is the object equals?
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns><c>true</c> if equals</returns>
        public override bool Equals(object obj) {
            var other = obj as ItemCount<T>;
            return other != null && Equals(other);
        }

        /// <summary>
        /// Is the object equals?
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns><c>true</c> if equals</returns>
        public bool Equals(ItemCount<T> other) {
            return item.Equals(other) && count == other.count;
        }

        /// <summary>
        /// Gets the hash code of current instance
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() {
            unchecked {
                int hashCode = 17;
                hashCode = hashCode * 23 + item.GetHashCode();
                hashCode = hashCode * 23 + count.GetHashCode();
                return hashCode;
            }
        }
    }

}
