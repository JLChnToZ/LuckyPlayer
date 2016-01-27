using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLChnToZ.LuckyPlayer {
    public class DestinyTuner<T> {
        static DestinyTuner<T> defaultInstance;
        public static DestinyTuner<T> Default {
            get {
                if(defaultInstance == null)
                    defaultInstance = new DestinyTuner<T>();
                return defaultInstance;
            }
        }

        double fineTuneOnSuccess;
        public double FineTuneOnSuccess {
            get { return fineTuneOnSuccess; }
            set {
                if(value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("Value must be between 0 and 1");
                fineTuneOnSuccess = value;
            }
        }

        public DestinyTuner() {
            fineTuneOnSuccess = -0.0001;
        }

        public DestinyTuner(double fineTuneOnSuccess) {
            FineTuneOnSuccess = fineTuneOnSuccess;
        }

        protected internal virtual void TuneDestinyOnSuccess(LuckyController<T> luckyControl) {
            luckyControl.fineTune *= 1 + fineTuneOnSuccess;
        }
    }
}
