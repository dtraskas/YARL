// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    internal class YARLRuleset
    {
        public YARLRuleset()
        {
            Name = String.Empty;
            Description = String.Empty;
            Rules = new List<YARLRule>();
            ConsequentVariables = new Dictionary<string, FuzzyVariable>();
        }

        public YARLRuleset(string name)
        {
            Name = name;
            Description = name;
            Rules = new List<YARLRule>();
            ConsequentVariables = new Dictionary<string, FuzzyVariable>();
        }

        #region Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public List<YARLRule> Rules { get; private set; }

        public Dictionary<string, FuzzyVariable> ConsequentVariables { get; private set; }

        #endregion

        #region Methods

        public void Finalise()
        {
            foreach(YARLRule rule in Rules){
                string name = rule.Consequent.FuzzyVariable.Name;
                if (!ConsequentVariables.ContainsKey(name)){
                    ConsequentVariables.Add(name, rule.Consequent.FuzzyVariable);
                }
            }                
        }

        public double Execute()
        {
            foreach(YARLRule rule in Rules){
                string consequentName = rule.Consequent.FuzzyVariable.Name;
                FuzzyVariable consequent = ConsequentVariables[consequentName];
                
                double value = rule.Fire();
                if (value > consequent.GroundValue){
                    rule.Consequent.Assigned.Value = value;    
                }
            }                

            foreach(KeyValuePair<string, FuzzyVariable> kvp in ConsequentVariables){
                double numerator = 0;
                double denominator = 0;

                FuzzyVariable consequent = kvp.Value;
                foreach(FuzzySetTriangular set in consequent.FuzzySets){
                    numerator += set.GetTypicalValue() * set.Value;
                    denominator += set.Value;
                }

                if (denominator != 0) {
                    return numerator / denominator;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
