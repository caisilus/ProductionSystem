using System;

namespace ProductionSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string factsFile = "../../../facts.txt";
            string rulesFule = "../../../rules.txt";
            ProductionSystemFileLoader productionSystemFileLoader = new ProductionSystemFileLoader();
            ProductionSystem ps = productionSystemFileLoader.LoadFromFiles(factsFile, rulesFule);
            foreach (Fact fact in ps.Facts)
            {
                Console.WriteLine($"{fact.Id} {fact.Name}");
            }
            foreach (Rule rule in ps.Rules)
            {
                Console.WriteLine(rule);
            }
        }
    }
}