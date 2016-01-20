using System;
using System.Collections.Generic;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace JLChnToZ.LuckyPlayer {
    /// <summary>
    /// Helper functions and extensions for Lucky Player.
    /// </summary>
    public static class Helpers {
        /// <summary>
        /// Resets all fine tune weight to 1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void ResetFineTuneWeight<T>(this WeightedCollection<T> collection) {
            if(collection == null) throw new ArgumentNullException("collection");
            LuckyController<T> luckControl;
            foreach(var kv in collection as IDictionary<T, IItemWeight<T>>) {
                luckControl = kv.Value as LuckyController<T>;
                if(luckControl == null) continue;
                luckControl.ResetFineTuneWeight();
            }
        }

        /// <summary>
        /// Scales all fine tune weight based on 1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool ScaleFineTuneWeight<T>(this WeightedCollection<T> collection) {
            if(collection == null) throw new ArgumentNullException("collection");
            var luckControls = new HashSet<LuckyController<T>>();
            double maxWeight = 0;
            LuckyController<T> luckControl;
            foreach(var kv in collection as IDictionary<T, IItemWeight<T>>) {
                luckControl = kv.Value as LuckyController<T>;
                if(luckControl == null) continue;
                luckControls.Add(luckControl);
                if(luckControl.fineTune > maxWeight)
                    maxWeight = luckControl.fineTune;
            }
            if(maxWeight <= 0) return false;
            foreach(var luckCtrl in luckControls)
                luckCtrl.fineTune /= maxWeight;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="rare"></param>
        /// <param name="baseRarity"></param>
        /// <returns></returns>
        public static LuckyController<T> Add<T>(this WeightedCollection<T> collection, T item, double rare, double baseRarity) {
            if(collection == null) throw new ArgumentNullException("collection");
            var luckControl = new LuckyController<T>(rare, baseRarity);
            collection.Add(item, luckControl);
            return luckControl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="rare"></param>
        /// <param name="initialAmount"></param>
        /// <param name="baseRarity"></param>
        /// <returns></returns>
        public static LimitedLuckyController<T> Add<T>(this WeightedCollection<T> collection, T item, double rare, int initialAmount, double baseRarity) {
            if(collection == null) throw new ArgumentNullException("collection");
            var luckControl = new LimitedLuckyController<T>(rare, initialAmount, baseRarity);
            collection.Add(item, luckControl);
            return luckControl;
        }
    }
}
