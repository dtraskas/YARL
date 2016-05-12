// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    internal class YARLRule
    {
        public YARLRule()
        {
            Name = String.Empty;
            Description = String.Empty;
            Priority = 1;
            Certainty = 1;
            Weight = 1;

            Antecedents = new List<FuzzyAssignment>();
            Operators = new List<YARLOperator>();
        }

        public YARLRule(string name)
        {
            Name = name;
            Description = name;
            Priority = 1;
            Certainty = 1;
            Weight = 1;

            Antecedents = new List<FuzzyAssignment>();
            Operators = new List<YARLOperator>();
        }

        public YARLRule(string name, int priority, double certainty, double weight)
        {
            Name = name;
            Description = name;
            Priority = priority;
            Certainty = certainty;
            Weight = weight;

            Antecedents = new List<FuzzyAssignment>();
            Operators = new List<YARLOperator>();
        }

        #region Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public double Certainty { get; set; }

        public double Weight { get; set; }

        public FuzzyAssignment Consequent { get; set; }

        public List<FuzzyAssignment> Antecedents { get; private set; }

        public List<YARLOperator> Operators { get; private set; }        

        #endregion

        #region Methods

        public double Fire()
        {
            double[] evaluatedAntecedents = new double[Antecedents.Count];
            int cnt = 0;
            foreach(FuzzyAssignment fa in Antecedents){
                evaluatedAntecedents[cnt++] = fa.Fuzzify();
            }

            cnt = 1;
            double prevValue = evaluatedAntecedents[0];            
            foreach(YARLOperator op in Operators){
                double nextValue = evaluatedAntecedents[cnt++];
                if (op == YARLOperator.AND) {
                    prevValue = Math.Min(prevValue, nextValue);
                } else {
                    prevValue = Math.Max(prevValue, nextValue);
                }                
            }
            return prevValue;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
