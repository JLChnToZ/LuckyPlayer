using System;
using JLChnToZ.LuckyPlayer;

namespace LuckyPlayerDemo {
    public class ItemLuckyController:LuckyController<Item> {
        public ItemLuckyController(double rare, double baseRarity = 1) : base(rare, baseRarity) {
        }

        public override double GetWeight(Item item) {
            item.rarity = rare;
            return base.GetWeight(item);
        }

        public override void OnSuccess(Item item) {
            item.rarity = rare;
            base.OnSuccess(item);
        }
    }
}
