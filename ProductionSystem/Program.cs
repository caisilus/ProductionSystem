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
        }
    }
}