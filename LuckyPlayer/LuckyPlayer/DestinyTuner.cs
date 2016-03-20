using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLChnToZ.LuckyPlayer {
    /// <summary>
    /// &quot;Destiny Tuner&quot;: An extension of lucky controller which will balance the probs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DestinyTuner<T> {
        static DestinyTuner<T> defaultInstance;
        /// <summary>
        /// Default instance of <see cref="DestinyTuner{T}"/>, defaults to all lucky controllers if undefined.
        /// </summary>
        public static DestinyTuner<T> Default {
            get {
                if(defaultInstance == null)
                    defaultInstance = new DestinyTuner<T>();
                return defaultInstance;
            }
        }

        protected double fineTuneOnSuccess;
        /// <summary>
        /// The fine tune value when success selected an item
        /// </summary>
        public double FineTuneOnSuccess {
            get { return fineTuneOnSuccess; }
            set { fineTuneOnSuccess = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DestinyTuner() {
            fineTuneOnSuccess = -0.0001;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DestinyTuner(double fineTuneOnSuccess) {
            FineTuneOnSuccess = fineTuneOnSuccess;
        }

        /// <summary>
        /// Called when an item binded with lucky controller successfully selected.
        /// </summary>
        /// <param name="luckyControl">The lucky controller</param>
        protected internal virtual void TuneDestinyOnSuccess(LuckyController<T> luckyControl) {
            luckyControl.fineTune *= 1 + fineTuneOnSuccess;
        }
    }
}
