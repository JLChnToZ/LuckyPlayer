using System;
using System.Collections.Generic;

using JLChnToZ.LuckyPlayer.WeightedRandomizer;

namespace WeightRandomizerShowcase {
    class Program {
        static WeightedCollection<string> weightedCollection;

        static void Main(string[] args) {
            weightedCollection = new WeightedCollection<string>();
            while(true) {
                try {
                    Console.WriteLine();
                    Console.Write(">> ");
                    string cmd = Console.ReadLine();
                    var cmdSplitted = cmd.Split(' ');
                    if(cmdSplitted.Length < 1) continue;
                    switch(cmdSplitted[0].ToLower()) {
                        case "add": Add(cmdSplitted); break;
                        case "remove": Remove(cmdSplitted); break;
                        case "cls": Console.Clear(); break;
                        case "clear": Clear(); break;
                        case "list": List(); break;
                        case "random": Random(cmdSplitted); break;
                        case "help": Help(); break;
                        default: Console.WriteLine("I don't know what should I do with \"{0}\".", cmd); break;
                    }
                } catch(Exception ex) {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
        }

        static void Add(string[] cmdSplitted) {
            double weight;
            string content;
            if(cmdSplitted.Length < 3)
                return;
            if(!double.TryParse(cmdSplitted[1], out weight))
                return;
            content = string.Join(" ", cmdSplitted, 2, cmdSplitted.Length - 2);
            weightedCollection.Add(content, weight);
            Console.WriteLine("\"{0}\" has been added into the pool. Pool item count = {1}", content, weightedCollection.Count);
        }

        static void Remove(string[] cmdSplitted) {
            string content;
            if(cmdSplitted.Length < 2) return;
            content = string.Join(" ", cmdSplitted, 1, cmdSplitted.Length - 1);
            if(!weightedCollection.Remove(content)) {
                Console.WriteLine("\"{0}\" does not exists.", content);
                return;
            }
            Console.WriteLine("\"{0}\" has been removed from the pool. Pool item count = {1}", content, weightedCollection.Count);
        }

        static void Random(string[] cmdSplitted) {
            if(weightedCollection.Count < 1) {
                Console.WriteLine("We get nothing from the pool. It is empty.");
                return;
            }
            long times = 1;
            double expectedProbs = 0;
            string content, untilContent = null;
            if(cmdSplitted.Length > 1) {
                if(cmdSplitted[1].ToLower() == "until" && cmdSplitted.Length > 2) {
                    untilContent = string.Join(" ", cmdSplitted, 2, cmdSplitted.Length - 2);
                    if(!weightedCollection.Contains(untilContent)) {
                        Console.WriteLine("The pool does not contain \"{0}\".", untilContent);
                        return;
                    }
                    times = long.MaxValue;
                    double totalProbs = 0;
                    foreach(var weight in (weightedCollection as IDictionary<string, double>).Values)
                        totalProbs += weight;
                    if(totalProbs > 0)
                        expectedProbs = weightedCollection.GetCurrentWeight(untilContent) / totalProbs;
                } else if(!long.TryParse(cmdSplitted[1], out times) || times <= 0)
                    times = 1;
            }
            Console.WriteLine("Lets do gacha, here are the results:");
            var resultDict = new Dictionary<string, long>();
            long count = 0, totalCount = 0;
            for(totalCount = 0; totalCount < times; totalCount++) {
                content = weightedCollection.GetRandomItem();
                Console.WriteLine("#{0}: {1}", totalCount + 1, content);
                if(!resultDict.TryGetValue(content, out count))
                    count = 0;
                resultDict[content] = count + 1;
                if(!string.IsNullOrEmpty(untilContent) && content == untilContent)
                    break;
            }
            Console.WriteLine();
            Console.WriteLine("Summary: ");
            foreach(var result in resultDict)
                Console.WriteLine("{0}: x{1}", result.Key, result.Value);

            if(!string.IsNullOrEmpty(untilContent)) {
                totalCount++;
                Console.WriteLine(
                    "We have gacha {0} times to get what you want ({1}).",
                    totalCount,
                    untilContent
                );
                Console.WriteLine(
                    "Your current luckyness index is {0:###.00}.",
                    Math.Log(1.0 / totalCount / expectedProbs) + 5
                );
            }
        }

        static void Clear() {
            weightedCollection.Clear();
            Console.WriteLine("Pool cleared.");
        }

        static void List() {
            Console.WriteLine("List out the items in the pool:");
            foreach(var item in (weightedCollection as IDictionary<string, double>))
                Console.WriteLine("- {0}: {1}", item.Key, item.Value);
        }

        static void Help() {
            Console.WriteLine("Usage:");
            Console.WriteLine("add (weight) (content)");
            Console.WriteLine("  Adds an item into the pool.");
            Console.WriteLine("remove (content)");
            Console.WriteLine("  Removes an item from the pool.");
            Console.WriteLine("clear");
            Console.WriteLine("  Clear the pool.");
            Console.WriteLine("list");
            Console.WriteLine("  List the content in the pool.");
            Console.WriteLine("random (times=1)");
            Console.WriteLine("  Do gacha for times specified.");
            Console.WriteLine("random until (content)");
            Console.WriteLine("  Do gacha until the target item is popped out.");
            Console.WriteLine("cls");
            Console.WriteLine("  Clears the console.");
            Console.WriteLine("help");
            Console.WriteLine("  Display this usage.");
        }
    }
}
