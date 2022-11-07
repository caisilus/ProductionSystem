using System.Collections.Generic;

namespace ProductionSystem
{
    public class ProductionSystem
    {
        private HashSet<Fact> facts;
        private List<Rule> rules;

        public ProductionSystem(List<Fact> startingFacts, List<Rule> rules)
        {
            facts = new HashSet<Fact>();
            foreach (Fact fact in startingFacts)
            {
                facts.Add(fact);
            }

            this.rules = rules;
        }
    }
}