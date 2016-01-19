using System;

namespace LuckyPlayerDemo {
    public class Item {
        public string name;

        internal double rarity;
        public string Rarity {
            get {
                return string.Format("{0}☆", Math.Floor(rarity + 1));
            }
        }

        public override string ToString() {
            return string.Format("{0} ({1})", name, Rarity);
        }
    }
}
