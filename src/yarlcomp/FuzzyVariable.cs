// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    internal class FuzzyVariable
    {
        public FuzzyVariable()
        {
            Name = String.Empty;
            FuzzyRange = new FuzzyRange();
            FuzzySets = new List<FuzzySet>();            
            Description = String.Empty;
        }

        public FuzzyVariable(string name)
        {
            Name = name;
            FuzzyRange = new FuzzyRange();
            FuzzySets = new List<FuzzySet>();            
            Description = String.Empty;
        }

        public FuzzyVariable(string name, FuzzyRange range)
        {
            Name = name;
            FuzzyRange = range;
            FuzzySets = new List<FuzzySet>();            
            Description = String.Empty;
        }

        #region Properties

        public string Name { get; set; }

        public double GroundValue { get; set; }

        public List<FuzzySet> FuzzySets { get; private set; }

        public FuzzyRange FuzzyRange { get; set; }

        public string Description { get; set; }

        #endregion

        #region Methods

        public double Fuzzify(string fuzzySetName)
        {
            FuzzySet set = FuzzySets.Find(item => item.Name == fuzzySetName);
            return set.Fuzzify(GroundValue);            
        }

        public void AddSet(FuzzySet set)
        {
            FuzzySets.Add(set);            
        }

        public void RecalculateMinMax()
        {
            double min = double.MaxValue;
            foreach(FuzzySetTriangular set in FuzzySets){
                min = Math.Min(min, set.X0);
            }

            double max = double.MinValue;
            foreach(FuzzySetTriangular set in FuzzySets){
                max = Math.Max(max, set.X2);
            }

            if (FuzzyRange.Min != min || FuzzyRange.Max != max)
                throw new CompilationException(String.Format("Invalid range specified for fuzzyvar {0}", Name));
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
