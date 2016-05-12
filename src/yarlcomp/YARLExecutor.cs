// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace DSparx.YARL
{
    internal class YARLExecutor
    {
        public YARLExecutor()
        {
            Rulesets = new List<YARLRuleset>();
        }

        public List<YARLRuleset> Rulesets { get; private set; }

        public double Execute()
        {
            foreach(YARLRuleset ruleset in Rulesets){
                return ruleset.Execute();
            }
            return 0;
        }
    }
}
