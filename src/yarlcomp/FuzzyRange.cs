// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;

namespace DSparx.YARL
{
    internal class FuzzyRange
    {
        public FuzzyRange()
        {
            Min = double.MinValue;
            Max = double.MaxValue;
        }

        public FuzzyRange(double min, double max)
        {
            if (min >= max)
                throw new CompilationException("The minimum value of the fuzzy range cannot be greater or equal to the maximum");

            Min = min;
            Max = max;
        }

        public double Min { get; private set; }

        public double Max { get; private set; }
    }
}
