using System;
using System.Collections.Generic;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace JLChnToZ.LuckyPlayer {
    /// <summary>
    /// A dynamic weight controller but will affects by and to the player's luckyness.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>You can inherit this class to add customizaton</remarks>
    public class LuckyController<T>: IItemWeight<T>, ISuccessCallback<T> {
        /// <summary>
        /// Take a couple percentage of probs when success.
        /// </summary>
        public static double fineTuneOnSuccess = -0.0001;
        internal protected readonly double rare;
        internal protected double baseRarity;
        internal protected PlayerLuck luckInstance;
        internal protected double fineTune;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rare">The rarity which will affects the player's luckyness.</param>
        /// <param name="baseRarity">Alterable rarity value</param>
        public LuckyController(double rare, double baseRarity = 1) {
            this.rare = rare;
            this.baseRarity = baseRarity;
            ResetFineTuneWeight();
        }

        /// <summary>
        /// Same usage as <see cref="IItemWeight{T}.GetWeight(T)"/>
        /// </summary>
        public virtual double GetWeight(T item) {
            if(luckInstance == null) return baseRarity / Math.Pow(2, rare);
            return baseRarity * Math.Pow(2, luckInstance.Luckyness - rare) * fineTune;
        }

        /// <summary>
        /// Calls when on item successfully selected, it will take away a bit probs by percentage of <see cref="fineTuneOnSuccess"/>.
        /// </summary>
        /// <param name="item">The selected item</param>
        public virtual void OnSuccess(T item) {
            fineTune *= 1 + fineTuneOnSuccess;
        }

        internal protected virtual void ResetFineTuneWeight() {
            fineTune = 1;
        }
    }

    /// <summary>
    /// An alternative of <see cref="LuckyController{T}"/> but with limited supply,
    /// which means the item binded will be available in limited amount.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>You can inherit this class to add customizaton</remarks>
    public class LimitedLuckyController<T>: LuckyController<T> {
        internal protected int amount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rare">The rarity which will affects the player's luckyness.</param>
        /// <param name="initialAmount">How many of the item initially have?</param>
        /// <param name="baseRarity">Alterable rarity value</param>
        public LimitedLuckyController(double rare, int initialAmount = 1, double baseRarity = 1) : base(rare, baseRarity) {
            amount = initialAmount;
        }

        /// <summary>
        /// Same usage as <see cref="IItemWeight{T}.GetWeight(T)"/>
        /// </summary>
        public override double GetWeight(T item) {
            return base.GetWeight(item) * amount;
        }

        /// <summary>
        /// Calles when on item successfully selected, the amount will minus one when called.
        /// </summary>
        /// <param name="item">The selected item</param>
        public override void OnSuccess(T item) {
            if(amount > 0) amount--;
            base.OnSuccess(item);
        }
    }

    /// <summary>
    /// Player lucky controller
    /// </summary>
    /// <remarks>You can inherit this class to add customizaton such as changing luckyness algorithm.</remarks>
    public class PlayerLuck {
        protected double luckyness;
        protected Random randomizer;
        /// <summary>
        /// Current player's luckyness
        /// </summary>
        public double Luckyness {
            get { return luckyness; }
        }

        /// <summary>
        /// Constructor with initial 1 luckyness defiend.
        /// </summary>
        public PlayerLuck() {
            luckyness = 1;
        }

        /// <summary>
        /// Constructor with custom luckyness defined.
        /// </summary>
        /// <param name="luck">Initial luckyness</param>
        public PlayerLuck(double luck) {
            luckyness = luck;
        }

        /// <summary>
        /// Gets a random item from the <paramref name="collection"/>, and do further process for luckyness adjustment.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection pool</param>
        /// <param name="random">Optional randomizer</param>
        /// <returns>The item selected</returns>
        public T HandleWithLuck<T>(WeightedCollection<T> collection, Random random = null) {
            if(random == null) {
                if(randomizer == null)
                    randomizer = new Random();
                random = randomizer;
            }
            return HandleWithLuck(collection, random.NextDouble());
        }

        /// <summary>
        /// Gets a random item from the <paramref name="collection"/> from random value given by caller, and do further process for luckyness adjustment.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection pool</param>
        /// <param name="randomValue">The random value generated by caller</param>
        /// <returns>The item selected</returns>
        /// <remarks>This overloaded method is for more advanced uses, which require callers to generate the random number by themself then passing it in.</remarks>
        public T HandleWithLuck<T>(WeightedCollection<T> collection, double randomValue) {
            if(collection == null) throw new ArgumentNullException("collection");
            var weightMapper = collection as IDictionary<T, IItemWeight<T>>;
            LuckyController<T> luckControl;
            foreach(var itemWeight in weightMapper.Values) {
                luckControl = itemWeight as LuckyController<T>;
                if(luckControl == null) continue;
                luckControl.luckInstance = this;
            }
            T result = collection.GetRandomItem(randomValue);
            if((luckControl = weightMapper[result] as LuckyController<T>) != null) {
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
