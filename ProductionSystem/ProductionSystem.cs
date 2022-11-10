using System.Collections.Generic;
using System.Linq;

namespace ProductionSystem
{
    public class ProductionSystem
    {
        private HashSet<Fact> facts;
        private HashSet<Rule> applicableRules;
        private List<Rule> rules;
        private bool factsSetChanged;

        public ProductionSystem(List<Rule> rules)
        {
            this.facts = new HashSet<Fact>();
            this.rules = rules;
            this.applicableRules = new HashSet<Rule>();
            this.factsSetChanged = false;
        }

        public IEnumerable<Fact> Facts => facts;

        public IEnumerable<Rule> Rules => rules;
        
        public IEnumerable<Fact> Deduce(IEnumerable<Fact> startingFacts)
        {
            LoadStartingFacts(startingFacts);
            
            while (factsSetChanged)
            {
                factsSetChanged = false;
                DeduceStep();
            }

            return facts;
        }

        private void LoadStartingFacts(IEnumerable<Fact> startingFacts)
        {
            Clear();
            UpdateFacts(startingFacts);
        }

        private void Clear()
        {
            applicableRules.Clear();
            facts.Clear();
        }
        
        private void DeduceStep()
        {
            UpdateApplicableRules();
            foreach (Rule rule in applicableRules)
            {
                UpdateFacts(rule.Consequences);
            }
        }

        private void UpdateFacts(IEnumerable<Fact> newFacts)
        {
            foreach (Fact fact in newFacts)
            {
                bool added = facts.Add(fact);
                if (added)
                    this.factsSetChanged = true;
            }
        }
        
        private void UpdateApplicableRules()
        {
            foreach (Rule rule in rules)
            {
                if (rule.IsApplicable(this.facts))
                {
                    this.applicableRules.Add(rule);
                }
            }
        }

        public IEnumerable<Fact> DeduceBack(Fact endFact)
        {
            Queue<Fact> queue = new Queue<Fact>();
            queue.Enqueue(endFact);
            HashSet<Fact> axiomFacts = new HashSet<Fact>();
            HashSet<Fact> visited = new HashSet<Fact>();
            while (queue.Count > 0)
            {
                Fact fact = queue.Dequeue();
                var possibleParents = GetPossibleParents(fact);
                if (possibleParents.Count == 0)
                {
                    axiomFacts.Add(fact);
                    continue;
                }
                foreach (var possibleParentFact in possibleParents)
                {
                    if (!visited.Contains(possibleParentFact))
                        queue.Enqueue(possibleParentFact);
                }

                visited.Add(fact);
            }

            return axiomFacts;
        }

        private List<Fact> GetPossibleParents(Fact fact)
        {
            List<Fact> possibleParents = new List<Fact>();
            foreach (var rule in rules)
            {
                if (rule.HasConsequence(fact))
                {
                    possibleParents.AddRange(rule.Conditions);
                }
            }

            return possibleParents;
        }
    }
}