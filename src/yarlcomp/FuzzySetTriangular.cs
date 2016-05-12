// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;

namespace DSparx.YARL
{
    internal class FuzzySetTriangular : FuzzySet
    {
        public FuzzySetTriangular()
        {
            Name = String.Empty;
            X0 = 0;
            X1 = 0;
            X2 = 0;
        }

        public FuzzySetTriangular(string name)
        {
            Name = name;
            X0 = 0;
            X1 = 0;
            X2 = 0;
        }

        public FuzzySetTriangular(string name, double x0, double x1, double x2)
        {
            Name = name;

            X0 = x0;
            X1 = x1;
            X2 = x2;

            if (X0 > X1 || X1 > X2 || X0 > X2)
                throw new CompilationException(String.Format("Invalid fuzzy set defined for fuzzyset {0}", name));
        }

        #region Properties

        public double X0 { get; set; }

        public double X1 { get; set; }

        public double X2 { get; set; }

        #endregion

        #region Methods
        
        public override double Fuzzify(double value)
        {
            double rc = 0;
            if ((X0 <= value) && (value < X1))
                rc = (value - X0) / (X1 - X0);
            else if ((X1 <= value) && (value < X2))
                rc = (X2 - value) / (X2 - X1);
            else if (value == X1)
                rc = 1;

            Console.WriteLine(String.Format("{0}  :  {1}", Name, rc));
            return rc;
        }

        public override double GetTypicalValue()
        {
            return X1;
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
