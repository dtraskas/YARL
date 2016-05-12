// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    public abstract class FuzzySet
    {
        public abstract double Fuzzify(double value);

        public abstract double GetTypicalValue();

        public string Name { get; set; }

        public double Value { get; set; }
    }
}
