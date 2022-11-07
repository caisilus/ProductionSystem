using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductionSystem
{
    public class Rule
    {
        public List<Fact> Conditions { get; private set; }
        public List<Fact> Consequences { get; private set; }

        public Rule(List<Fact> conditions, List<Fact> consequences)
        {
            Conditions = conditions;
            Consequences = consequences;
        }

        public bool IsApplicable(HashSet<Fact> facts)
        {
            foreach (Fact cond in Conditions)
            {
                if (!facts.Contains(cond))
                    return false;
            }

            return true;
        }

        // TODO can be changed for your needs
        public override string ToString()
        {
            return FactsListToString(Conditions) + "->" + FactsListToString(Consequences);
        }

        private string FactsListToString(List<Fact> facts)
        {
            return string.Join(",", facts);
        }
    }
}