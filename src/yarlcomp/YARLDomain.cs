// Copyright (c) Dimitrios Traskas. All rights reserved. See README.md in the project root for license information.

using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace DSparx.YARL.Compiler
{
    public class YARLDomain
    {
        private List<YARLRule> tempRules;

        public YARLDomain()
        {
            FuzzyVariables = new  Dictionary<string, FuzzyVariable>();
            Rulesets = new  Dictionary<string, YARLRuleset>();
            Executors = new List<YARLExecutor>();
            tempRules = new List<YARLRule>();
        }

        #region Properties

        internal Dictionary<string, FuzzyVariable> FuzzyVariables { get; private set; }

        internal Dictionary<string, YARLRuleset> Rulesets { get; private set; }

        internal List<YARLExecutor> Executors { get; set; }

        #endregion

        #region Fuzzy Engine Execution Methods

        public void Ground(string name, double value)
        {
            if (FuzzyVariables.ContainsKey(name)){
                FuzzyVariables[name].GroundValue = value;
            } else {
                throw new Exception(String.Format("Undefined fuzzy variable: {0}", name));
            }
        }

        public void Execute()
        {
            foreach(YARLExecutor executor in Executors){
                Result = executor.Execute();
            }
        }

        public double Result { get; private set; }

        #endregion

        #region Compilation Methods

        public void Compile(string filename)
        {
            try 
            {
                ANTLRFileStream stream = new ANTLRFileStream(filename);
                YARLLexer lexer = new YARLLexer(stream);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                YARLParser parser = new YARLParser(tokens);
                CommonTree tree = parser.program().Tree;
                
                for (int i=0; i < tree.ChildCount; i++) {
                    CommonTree child = (CommonTree)tree.GetChild(i);                
                    switch (child.Text) 
                    {
                        case "FUZZYVAR":
                            CreateFuzzyVariable(child);
                            break;
                        case "RULEDEF":
                            CreateRule(child);
                            break;
                        case "RULESETDEF":
                            CreateRuleSet(child);
                            break;
                        case "EXECUTEDEF":
                            CreateExecutionSet(child);
                            break;
                    }
                }
                tempRules.Clear();

            } catch (RecognitionException re) {
                throw new Exception(String.Format("YARL Syntax Error: encountered an illegal argument on line {0}, column {1}: {2}", re.Line, re.CharPositionInLine, re.Token.Text));
            } catch (CompilationException ce) {
                throw new Exception(String.Format("YARL Compile Error: {0}", ce.Message));
            } catch (RewriteEarlyExitException rex) {
                throw new Exception(String.Format("YARL Compile Error: The program exited early because there are missing statements"));
            }
        }        

        private void CreateFuzzyVariable(CommonTree tree)
        {
            FuzzyVariable variable = new FuzzyVariable();
            for (int i=0; i < tree.ChildCount; i++) {
                CommonTree child = (CommonTree)tree.GetChild(i);
                switch(child.Text)
                {
                    case "FUZZYRANGEDEF":
                        double min = Convert.ToDouble(child.Children[0].Text);
                        double max = Convert.ToDouble(child.Children[1].Text);
                        if (min >= max)
                            throw new CompilationException(String.Format("Invalid range specified for fuzzyvar {0}", variable.Name));

                        variable.FuzzyRange = new FuzzyRange(min, max);
                        break;
                    case "DESCDEF":
                        variable.Description = child.Children[0].Text;                        
                        break;
                    case "FUZZYSETDEF":
                        string shape = child.Children[0].Text;
                        string name = child.Children[1].Text;
                        double num1 = Convert.ToDouble(child.Children[2].Text);
                        double num2 = Convert.ToDouble(child.Children[3].Text);
                        double? num3 = null;
                        
                        if (child.ChildCount == 5){
                            num3 = Convert.ToDouble(child.Children[4].Text);
                        }
                        variable.AddSet(CreateFuzzySet(shape, name, num1, num2, num3));
                        break;
                    default:
                        // this is the variable name
                        variable.Name = child.Text;
                        break;
                }
            }
            variable.RecalculateMinMax();
            FuzzyVariables.Add(variable.Name, variable);            
        }

        private FuzzySet CreateFuzzySet(string shape, string name, double num1, double num2, double? num3)
        {
            FuzzySet set = null;
            switch(shape)
            {
                case "SHAPES":
                    set = new FuzzySetTriangular(name, num1, num1, num2);
                    break;
                case "SHAPEP":
                    if (num3 != null) {
                        set = new FuzzySetTriangular(name, num1, num2, num3.Value);
                    } else {
                        throw new CompilationException(String.Format("Invalid fuzzy set defined for fuzzyvar {0}", name));
                    }
                    break;
                case "SHAPEZ":
                    set = new FuzzySetTriangular(name, num1, num2, num2);                    
                    break;
            }
            return set;
        }

        private void CreateRule(CommonTree tree)
        {
            YARLRule rule = new YARLRule();
            for (int i=0; i < tree.ChildCount; i++) {
                CommonTree child = (CommonTree)tree.GetChild(i);
                switch(child.Text)
                {
                    case "CERTAINTYDEF":
                        double certainty = Convert.ToDouble(child.Children[0].Text);
                        if (certainty < 0 || certainty > 1) {
                            throw new CompilationException(String.Format("Invalid certainty factor specified for rule {0}", rule.Name));
                        }
                        rule.Certainty = certainty;
                        break;
                    case "PRIORITYDEF":
                        int priority = Convert.ToInt32(child.Children[0].Text);
                        if (priority < 0) {
                            throw new CompilationException(String.Format("Invalid priority specified for rule {0}", rule.Name));
                        }
                        rule.Priority = priority;
                        break;
                    case "DESCDEF":
                        rule.Description = child.Children[0].Text;
                        break;
                    case "WEIGHTDEF":
                        double weight = Convert.ToDouble(child.Children[0].Text);
                        if (weight < 0) {
                            throw new CompilationException(String.Format("Invalid weight specified for rule {0}", rule.Name));
                        }
                        rule.Weight = weight;
                        break;
                    case "WHENDEF":
                        BuildRule(rule, child);
                        break;
                    default:
                        // this is the rule name
                        rule.Name = child.Text;
                        break;
                }
            }
            tempRules.Add(rule);
        }

        private void BuildRule(YARLRule rule, CommonTree tree)
        {
             for (int i=0; i < tree.ChildCount; i++) {
                CommonTree child = (CommonTree)tree.GetChild(i);
                if (child.Text == "ANTECEDENTSDEF"){
                    for (int k=0; k < child.ChildCount; k++) {
                        CommonTree nextChild = (CommonTree)child.GetChild(k);
                        if (nextChild.Text == "ANTECEDENTDEF") {
                            string name = nextChild.GetChild(0).GetChild(0).Text;
                            string line = nextChild.GetChild(0).GetChild(0).Line.ToString();
                            FuzzyVariable variable = RetrieveFuzzyVariable(name);
                            if (variable == null) {
                                throw new CompilationException(String.Format("Undefined variable: {0} encountered in rule {1}, line {2}", name, rule.Name, line));
                            }
                    
                            name = nextChild.GetChild(1).Text;
                            line = nextChild.GetChild(1).Line.ToString();
                            FuzzySet set = RetrieveFuzzySet(variable, name);
                            if (set == null) {
                                throw new CompilationException(String.Format("Undefined fuzzy set: {0} encountered in rule {1}, line {2}", name, rule.Name, line));
                            }
                            rule.Antecedents.Add(new FuzzyAssignment(variable, set));
                        } else if (nextChild.Text == "OPERATORDEF") {
                            string name = nextChild.GetChild(0).Text;
                            if (name.CompareTo("and") == 0) {
                                rule.Operators.Add(YARLOperator.AND);
                            } else {
                                rule.Operators.Add(YARLOperator.OR);
                            }
                        }                    
                    }
                } else if (child.Text == "CONSEQUENTDEF"){
                    string name = child.GetChild(0).GetChild(0).Text;
                    string line = child.GetChild(0).GetChild(0).Line.ToString();
                    FuzzyVariable variable = RetrieveFuzzyVariable(name);
                    if (variable == null) {
                        throw new CompilationException(String.Format("Undefined variable: {0} encountered in rule {1}, line {2}", name, rule.Name, line));
                    }

                    name = child.GetChild(1).Text;
                    line = child.GetChild(1).Line.ToString();
                    FuzzySet set = RetrieveFuzzySet(variable, name);
                    if (set == null) {
                        throw new CompilationException(String.Format("Undefined fuzzy set: {0} encountered in rule {1}, line {2}", name, rule.Name, line));
                    }                                        
                    rule.Consequent = new FuzzyAssignment(variable, set);
                }
             }
        }

        private FuzzyVariable RetrieveFuzzyVariable(string name)
        {
            if (FuzzyVariables.ContainsKey(name)) {                
                return FuzzyVariables[name];
            } else {
                return null;
            }
        }

        private FuzzySet RetrieveFuzzySet(FuzzyVariable variable, string name)
        {
            int index = variable.FuzzySets.FindIndex(item => item.Name == name);
            if (index >= 0) {                
                return variable.FuzzySets[index];
            } else {
                return null;
            }
        }

        private void CreateRuleSet(CommonTree tree)
        {
            YARLRuleset ruleset = new YARLRuleset();
            for (int i=0; i < tree.ChildCount; i++) {
                CommonTree child = (CommonTree)tree.GetChild(i);                
                if (child.Text == "DESCDEF"){
                    ruleset.Description = child.GetChild(0).Text;
                } else if (child.Text == "RULECOLLECTIONALLDEFCHOICE"){
                    ruleset.Rules.AddRange(tempRules);
                } else if (child.Text == "RULECOLLECTIONDEFCHOICE") {
                    int count = child.GetChild(0).ChildCount;
                    for(int k=0; k<count; k++){
                        string name = child.GetChild(0).GetChild(k).Text;
                        string line = child.GetChild(0).GetChild(k).Line.ToString();

                        int index = tempRules.FindIndex(item => item.Name == name);
                        if (index < 0) {
                            throw new CompilationException(String.Format("Undefined rule: {0} encountered in ruleset {1}, line {2}", name, ruleset.Name, line));
                        }
                        ruleset.Rules.Add(tempRules[index]);
                    }
                } else {
                    ruleset.Name = child.Text;
                }
            }
            ruleset.Finalise();
            Rulesets.Add(ruleset.Name, ruleset);
        }

        private void CreateExecutionSet(CommonTree tree)
        {
            YARLExecutor executor = new YARLExecutor();
            for (int i=0; i < tree.ChildCount; i++) {
                CommonTree child = (CommonTree)tree.GetChild(i);                
                string name = child.Text;
                string line = child.Line.ToString();

                if (Rulesets.ContainsKey(name)){
                    executor.Rulesets.Add(Rulesets[name]);
                } else {
                    throw new CompilationException(String.Format("Undefined ruleset: {0} encountered in execution statement, line {1}", name, line));
                }                                
            }
            Executors.Add(executor);
        }
        
        #endregion
    }
}
