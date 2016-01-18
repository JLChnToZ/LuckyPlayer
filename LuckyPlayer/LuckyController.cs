using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace JLChnToZ.LuckyPlayer {
    public class LuckyController<T>: IItemWeight<T> {
        internal protected readonly double rare;
        internal protected double baseRarity;
        internal protected PlayerLuck luckInstance;

        public LuckyController(double rare, double baseRarity = 1) {
            this.rare = rare;
            this.baseRarity = baseRarity;
        }

        public virtual double GetWeight(T item) {
            if(luckInstance == null) return baseRarity / Math.Pow(2, rare);
            return baseRarity * Math.Pow(2, luckInstance.Luckyness - rare);
        }

        public virtual void OnSuccess(T item) { }
    }

    public class LimitedLuckyController<T>: LuckyController<T> {
        internal protected int amount;

        public LimitedLuckyController(double rare, int initialAmount = 1, double baseRarity = 1) : base(rare, baseRarity) {
            amount = initialAmount;
        }

        public override double GetWeight(T item) {
            return base.GetWeight(item) * amount;
        }

        public override void OnSuccess(T item) {
            amount--;
        }
    }

    public class PlayerLuck {
        protected double luckyness;
        public double Luckyness {
            get { return luckyness; }
        }

        public PlayerLuck() {
            luckyness = 1;
        }

        public PlayerLuck(double luck) {
            luckyness = luck;
        }

        public T HandleWithLuck<T>(WeightedCollection<T> collection, Random random = null) {
            if(collection == null) throw new ArgumentNullException("collection");
            var weightMapper = collection as IDictionary<T, IItemWeight<T>>;
            LuckyController<T> luckControl;
            foreach(var itemWeight in weightMapper.Values) {
                luckControl = itemWeight as LuckyController<T>;
                if(luckControl == null) continue;
                luckControl.luckInstance = this;
            }
            T result = collection.GetRandomItem(random);
            if((luckControl = weightMapper[result] as LuckyController<T>) != null) {
                luckControl.OnSuccess(result);
                LuckyControl(luckControl.rare);
                if(luckyness < double.Epsilon) luckyness = double.Epsilon;
            }
            return result;
        }

        protected virtual void LuckyControl(double resultRarity) {
            if(resultRarity <= 0)
                luckyness += Math.Pow(2, resultRarity);
            else
                luckyness /= Math.Pow(2, resultRarity);
        }
    }
}
