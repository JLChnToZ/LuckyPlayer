using System;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace JLChnToZ.LuckyPlayer {
    /// <summary>
    /// A dynamic weight controller but will affects by and to the player's luckyness.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>You can inherit this class to add customizaton</remarks>
    public class LuckyController<T>: IItemWeight<T>, ISuccessCallback<T> {
        internal protected readonly double rare;
        internal protected double baseRarity;
        internal protected PlayerLuck luckInstance;
        internal protected double fineTune;
        internal protected DestinyTuner<T> destinyTuner;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rare">The rarity which will affects the player's luckyness.</param>
        /// <param name="baseRarity">Alterable rarity value</param>
        public LuckyController(double rare, double baseRarity = 1, DestinyTuner<T> destinyTuner = null) {
            this.rare = rare;
            this.baseRarity = baseRarity;
            this.destinyTuner = destinyTuner ?? DestinyTuner<T>.Default;
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
            destinyTuner.TuneDestinyOnSuccess(this);
        }

        internal protected virtual void ResetFineTuneWeight() {
            fineTune = 1;
        }
    }
}