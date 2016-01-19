using JLChnToZ.LuckyPlayer;

namespace LuckyPlayerDemo {
    class AlterablePlayerLuckyness: PlayerLuck {
        public new double Luckyness {
            get { return luckyness; }
            set { luckyness = value; }
        }

        public AlterablePlayerLuckyness() { }

        public AlterablePlayerLuckyness(double luck) : base(luck) { }
    }
}
