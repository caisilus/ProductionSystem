using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ProductionSystem
{
    public class ProductionSystemFileLoader
    {
        private readonly Dictionary<string, Fact> _idsToFacts = new Dictionary<string, Fact>();
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
            
            List<string> factLines = ExtractFactLines(filteredLines);
            List<string> rulesLines = ExtractRuleLines(filteredLines);

            return CreateProductionSystemFromLines(factLines, rulesLines);
        }

        public ProductionSystem LoadFromFiles(string factsFileName, string rulesFileName)
        {
            string[] factLines = System.IO.File.ReadAllLines(factsFileName);
            string[] ruleLines = System.IO.File.ReadAllLines(rulesFileName);

            List<string> filteredFactLines = SkipCommentAndEmptyLines(factLines);
            List<string> filteredRuleLines = SkipCommentAndEmptyLines(ruleLines);
            
            return CreateProductionSystemFromLines(filteredFactLines, filteredRuleLines);
        }

        private List<string> SkipCommentAndEmptyLines(string[] lines)
        {
            return lines.Where(CorrectLine).ToList();
        }

        private bool CorrectLine(string line)
        {
            return !line.StartsWith(_commentStart) && !string.IsNullOrEmpty(line) && line != "________________";
        }

        private List<string> ExtractFactLines(List<string> lines)
        {
            List<string> factLines = new List<string>();
            for (int i = 0; lines[i] != _blocksSeparator; i++)
            {
                factLines.Add(lines[i]);
            }

            return factLines;
        }

        private List<string> ExtractRuleLines(List<string> lines)
        {
            int separatorLineIndex = lines.FindIndex(line => line.Trim() == _blocksSeparator);
            if (separatorLineIndex == -1)
                throw new InvalidOperationException($"Block separator {_blocksSeparator} not found in file");

            List<string> ruleLines = new List<string>();
            for (int i = separatorLineIndex + 1; i < lines.Count; i++)
            {
                ruleLines.Add(lines[i]);
            }

            return ruleLines;
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
                Fact fact = CreateFactFromLine(factLine);
                _idsToFacts[fact.Id] = fact; // side effect, but name contains 'load'
                result.Add(fact);
            }

            return result;
        }

        private Fact CreateFactFromLine(string factLine)
        {
            string[] tokens = factLine.Split(_factTokensSeparator);
            string id = tokens[0];
            string name = tokens[1];
            Fact fact = new Fact(id, name);
            return fact;
        }
        
        private List<Rule> LoadRules(List<string> ruleLines)
        {
            return ruleLines.Select(CreateRuleFromLine).ToList();
        }

        // uses _idsToFacts, so facts should be loaded before
        private Rule CreateRuleFromLine(string ruleLine)
        {
            string[] tokens = ruleLine.Split(_conditionAndConsequenceSeparator);
            
            ValidateRuleTokens(tokens);
            
            string leftSide = tokens[0];
            List<Fact> conditions = FactsFromEnumeration(leftSide);
            
            string rightSide = tokens[1];
            List<Fact> consequences = FactsFromEnumeration(rightSide);
            
            return new Rule(conditions, consequences);
        }

        private void ValidateRuleTokens(string[] tokens)
        {
            if (tokens.Length > 2)
                throw new ArgumentException(
                    $"Rule line contains more than 2 tokens separated by {_conditionAndConsequenceSeparator}");
        }
        
        // actual use of _idsToFacts
        private List<Fact> FactsFromEnumeration(string factsEnumeration)
        {
            return FactIdsFromEnumeration(factsEnumeration).Select(id => _idsToFacts[id]).ToList();
        }
        
        private IEnumerable<string> FactIdsFromEnumeration(string factsEnumeration)
        {
            return factsEnumeration.Split(_factsSeparator).Select(idString=>idString.Trim());
        }
    }
}