using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProductionSystem
{
    public class ProductionSystemFileLoader
    {
        private Dictionary<string, Fact> _idsToFacts = new Dictionary<string, Fact>();
        private readonly string _blocksSeparator;
        private readonly string _conditionAndConsequenceSeparator;
        private readonly string _factTokensSeparator;
        private readonly string _factsSeparator;
        private readonly string _commentStart;
        
        public ProductionSystemFileLoader(string blocksSeparator = "===", string conditionAndConsequenceSeparator = "->",
                                          string factTokensSeparator = ";", string factsSeparator = ",", 
                                          string commentStart="- -")
        {
            this._blocksSeparator = blocksSeparator;
            this._conditionAndConsequenceSeparator = conditionAndConsequenceSeparator;
            this._factTokensSeparator = factTokensSeparator;
            this._factsSeparator = factsSeparator;
            this._commentStart = commentStart;
        }
        
        public ProductionSystem LoadFromFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            List<string> filteredLines = SkipCommentAndEmptyLines(lines);
            
            var factsAndRulesLinesPair = SeparateFactsAndRulesLines(filteredLines);
            List<string> factLines = factsAndRulesLinesPair.Item1;
            List<string> rulesLines = factsAndRulesLinesPair.Item2;

            return CreateProductionSystemFromLines(factLines, rulesLines);
        }

        public ProductionSystem LoadFromFiles(string factsFileName, string rulesFileName)
        {
            string[] factsLines = System.IO.File.ReadAllLines(factsFileName);
            string[] rulesLines = System.IO.File.ReadAllLines(rulesFileName);
            
            return CreateProductionSystemFromLines(SkipCommentAndEmptyLines(factsLines), 
                SkipCommentAndEmptyLines(rulesLines));
        }

        private List<string> SkipCommentAndEmptyLines(string[] lines)
        {
            return lines.Where(CorrectLine).ToList();
        }

        private bool CorrectLine(string line)
        {
            return !line.StartsWith(_commentStart) && !string.IsNullOrEmpty(line) && line != "________________";
        }
        
        private Tuple<List<string>, List<String>> SeparateFactsAndRulesLines(List<string> lines)
        {
            List<string> factLines = new List<string>();
            List<string> rulesLines = new List<string>();
            int ind;
            for (ind = 0; lines[ind] != _blocksSeparator; ind++)
            {
                factLines.Add(lines[ind]);
            }

            for (int i = ind; i < lines.Count; i++)
            {
                rulesLines.Add(lines[i]);
            }

            return new Tuple<List<string>, List<string>>(factLines, rulesLines);
        }
        
        private ProductionSystem CreateProductionSystemFromLines(List<string> factsLines, List<string> rulesLines)
        {
            List<Fact> facts = LoadFacts(factsLines); // facts should be loaded before rules
            List<Rule> rules = LoadRules(rulesLines);

            return new ProductionSystem(facts, rules);
        }

        private List<Fact> LoadFacts(List<string> factLines)
        {
            List<Fact> result = new List<Fact>();
            foreach (string factLine in factLines)
            {
                result.Add(CreateFactFromLine(factLine));
            }

            return result;
        }

        private Fact CreateFactFromLine(string factLine)
        {
            string[] tokens = factLine.Split(_factTokensSeparator);
            string id = tokens[0];
            string name = tokens[1];
            Fact fact = new Fact(id, name);
            _idsToFacts[id] = fact;
            return fact;
        }
        
        private List<Rule> LoadRules(List<string> ruleLines)
        {
            List<Rule> result = new List<Rule>();
            foreach (string ruleLine in ruleLines)
            {
                result.Add(CreateRuleFromLine(ruleLine));
            }

            return result;
        }

        // uses _idsToFacts, so facts should be loaded before
        private Rule CreateRuleFromLine(string ruleLine)
        {
            string[] tokens = ruleLine.Split(_conditionAndConsequenceSeparator);
            
            string leftSide = tokens[0];
            List<Fact> conditions = FactsFromEnumeration(leftSide);
            
            string rightSide = tokens[1];
            List<Fact> consequences = FactsFromEnumeration(rightSide);
            
            return new Rule(conditions, consequences);
        }

        private List<Fact> FactsFromEnumeration(string factsEnumeration)
        {
            return FactIdsFromEnumeration(factsEnumeration).Select(id => _idsToFacts[id]).ToList();
        }
        
        private string[] FactIdsFromEnumeration(string factsEnumeration)
        {
            return factsEnumeration.Split(_factsSeparator).Select(idString=>idString.Trim()).ToArray();
        }
    }
}