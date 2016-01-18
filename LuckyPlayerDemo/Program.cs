using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLChnToZ.LuckyPlayer;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace LuckyPlayerDemo {
    class Program {
        static void Main(string[] args) {
            PlayerLuck playerLuck = new PlayerLuck(1);
            WeightedCollection<Item> collection = new WeightedCollection<Item>();
            collection.Add(new Item { name = "N" }, new LuckyController<Item>(-1, 2 * 100));
            collection.Add(new Item { name = "N+" }, new LuckyController<Item>(0, 2 * 70));
            collection.Add(new Item { name = "R" }, new LuckyController<Item>(2, 60));
            collection.Add(new Item { name = "R+" }, new LuckyController<Item>(3, 40));
            collection.Add(new Item { name = "SR" }, new LuckyController<Item>(4, 30));
            collection.Add(new Item { name = "SR+" }, new LuckyController<Item>(5, 25));
            collection.Add(new Item { name = "SSR" }, new LuckyController<Item>(6, 20));
            collection.Add(new Item { name = "SSR+" }, new LuckyController<Item>(7, 15));
            collection.Add(new Item { name = "UR" }, new LuckyController<Item>(8, 10));
            collection.Add(new Item { name = "UR+" }, new LuckyController<Item>(9, 5));

            Console.WriteLine("人品系統 (III) 測試程式 - Lucky Player Testing Application");
            Console.WriteLine();

            int amount = 0;
            while(true) {
                Console.WriteLine("玩家幸運值 (所謂的人品): {0:0.000}", playerLuck.Luckyness);
                do {
                    Console.Write("多少連抽? ");
                } while(!int.TryParse(Console.ReadLine(), out amount));
                var items = new List<Item>();
                for(int i = 0; i < amount; i++)
                    items.Add(playerLuck.HandleWithLuck(collection));
                Console.Write("結果: ");
                if(amount < 1000)
                    foreach(var item in items)
                        Console.Write(item.name + " ");
                Console.WriteLine();
                foreach(var kv in Count(items))
                    Console.WriteLine("{0} x{1}", kv.Key.name, kv.Value);
                Console.WriteLine();
            }
        }

        static Dictionary<T, int> Count<T>(IEnumerable<T> list) {
            var dict = new Dictionary<T, int>();
            int i;
            foreach(var item in list) {
                if(dict.TryGetValue(item, out i))
                    dict[item] = i + 1;
                else
                    dict.Add(item, 1);
            }
            return dict;
        }
    }

    public class Item {
        public string name;
    }
}
