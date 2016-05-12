// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    internal class FuzzyAssignment
    {
        public FuzzyAssignment(FuzzyVariable variable, FuzzySet answer)
        {
            FuzzyVariable = variable;
            Assigned = answer;            
        }

        #region Properties

        public FuzzyVariable FuzzyVariable { get; private set; }

        public FuzzySet Assigned { get; private set; }

        #endregion

        #region Methods

        public double Fuzzify()
        {
            return FuzzyVariable.Fuzzify(Assigned.Name);
        }

        #endregion
    }
}
