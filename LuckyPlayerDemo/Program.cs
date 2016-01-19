using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using JLChnToZ.LuckyPlayer;
using JLChnToZ.LuckyPlayer.WeightedRandomizer;
using Newtonsoft.Json;

namespace LuckyPlayerDemo {
    class Program {
        const int dataVer = 1;

        static AlterablePlayerLuckyness playerLuck;
        static WeightedCollection<Item> collection = new WeightedCollection<Item>();
        static int tickets, inventorySize, inventoryUsed, usedMoney;

        static void Main(string[] args) {
            LoadItems();
            LoadData();

            Console.WriteLine("人品系統 (III) 測試程式 - Lucky Player Testing Application");
            Console.WriteLine();

            int mode = 0;
            while(true) {
                Console.WriteLine("幸運值 (所謂的人品): {0:0.000}, 抽獎券: x{1}, 背包大小: {3}/{4}, [花費了金錢: ${2}].",
                    playerLuck.Luckyness, tickets, usedMoney, inventoryUsed, inventorySize);

                do {
                    Console.Write("選項: (1 = 1抽, 2 = 5連抽, 3 = 10+1連抽, 4 = 課金) >> ");
                } while(!int.TryParse(Console.ReadLine(), out mode));

                switch(mode) {
                    case 1: DoGacha(1, 0); break;
                    case 2: DoGacha(5, 0); break;
                    case 3: DoGacha(11, 1); break;
                    case 4: Buy(); break;
                }
                SaveData();
            }
        }

        static void LoadItems() {
            if(File.Exists("poollist.json")) {
                try {
                    using(var fs = new FileStream("poollist.json", FileMode.Open, FileAccess.Read))
                    using(var fsRead = new StreamReader(fs, Encoding.UTF8)) {
                        var array = JsonConvert.DeserializeObject<JsonItem[]>(fsRead.ReadToEnd());
                        foreach(var itemRaw in array) {
                            var item = new Item { name = itemRaw.name };
                            var luckControl = new ItemLuckyController(itemRaw.rare);
                            collection.Add(item, luckControl);
                        }
                    }
                } catch { }
            } else {
                Console.WriteLine("找不到定義.");
                return;
            }
        }

        static void LoadData() {
            double luck = 1;
            if(File.Exists("player.dat")) {
                try {
                    int fileVer = 0;
                    using(var fs = new FileStream("player.dat", FileMode.Open, FileAccess.Read))
                    using(var fsRead = new BinaryReader(fs)) {
                        fileVer = fsRead.ReadInt32();
                        luck = fsRead.ReadDouble();
                        inventorySize = fsRead.ReadInt32();
                        inventoryUsed = fsRead.ReadInt32();
                        tickets = fsRead.ReadInt32();
                        usedMoney = fsRead.ReadInt32();
                    }
                } catch { }
            } else {
                tickets = 10;
                inventorySize = 100;
                inventoryUsed = 0;
            }
            playerLuck = new AlterablePlayerLuckyness(luck);
        }

        static void SaveData() {
            try {
                using(var fs = new FileStream("player.dat", FileMode.OpenOrCreate, FileAccess.Write))
                using(var fsWrite = new BinaryWriter(fs)) {
                    fsWrite.Write(dataVer);
                    fsWrite.Write(playerLuck.Luckyness);
                    fsWrite.Write(inventorySize);
                    fsWrite.Write(inventoryUsed);
                    fsWrite.Write(tickets);
                    fsWrite.Write(usedMoney);
                }
            } catch { }
        }

        static void DoGacha(int amount, int discount) {
            if(tickets < amount - discount) {
                Console.WriteLine("不夠抽獎券, 請課金!");
                return;
            }
            if(inventoryUsed + amount > inventorySize) {
                Console.WriteLine("背包位置不夠, 請購買!");
                return;
            }
            tickets -= amount - discount;
            inventoryUsed += amount;
            var items = new List<Item>();
            for(int i = 0; i < amount; i++)
                items.Add(playerLuck.HandleWithLuck(collection));
            Console.Write("\n結果: ");
            if(amount < 100)
                foreach(var item in items)
                    Console.Write(item + " ");
            Console.WriteLine();
            foreach(var kv in Count(items))
                Console.WriteLine("{0} x{1}", kv.Key, kv.Value);
            Console.WriteLine();
        }

        static void Buy() {
            int mode;
            do {
                Console.Write("選項: (1 = 1張抽獎券($1) 2 = 10張抽獎券($9) 3 = 10格背包($5) 4 = 不買了) >> ");
            } while(!int.TryParse(Console.ReadLine(), out mode));
            switch(mode) {
                case 1:
                    Console.WriteLine("買了 1 張抽獎券, 花費了 $1.");
                    tickets += 1;
                    usedMoney += 1;
                    playerLuck.Luckyness += 0.5;
                    break;
                case 2:
                    Console.WriteLine("買了 10 張抽獎券, 花費了 $9.");
                    tickets += 10;
                    usedMoney += 9;
                    playerLuck.Luckyness += 5;
                    break;
                case 3:
                    Console.WriteLine("買了 10 格背包, 花費了 $5.");
                    inventorySize += 10;
                    usedMoney += 5;
                    playerLuck.Luckyness += 2;
                    break;
                case 4:
                    Console.WriteLine("沒有買, 折返.");
                    break;
                default:
                    Console.WriteLine("沒有這選項...");
                    break;
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
}
