using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductionSystem
{
    class Program
    {
        static void DeduceShowcase(ProductionSystem ps, Fact[] startingFacts)
        {
            var res = ps.Deduce(startingFacts);
            Console.WriteLine($"From starting facts {string.Join(", ", startingFacts.ToList())} deduce:");
            foreach (Fact fact in res)
            {
                Console.WriteLine(fact);
            }
        }

        static void DeduceBackShowcase(ProductionSystem ps, Fact endFact)
        {
            var classFacts = ps.DeduceBack(endFact);
            Console.WriteLine($"From end fact {endFact} deduce back:");
            foreach (Fact fact in classFacts)
            {
                Console.WriteLine(fact.Name);
            }
        }
        
        static void Main(string[] args)
        {
            string factsFile = "../../../test_facts.txt";
            string rulesFule = "../../../test_rules.txt";
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
            // deducing
            Fact[] startingFacts = new[] {new Fact("f1", "Твой класс Варвар")};
            DeduceShowcase(ps, startingFacts);
            // deducing back
            Fact endFact = new Fact("f12", "");
            DeduceBackShowcase(ps, endFact);
        }
    }
}