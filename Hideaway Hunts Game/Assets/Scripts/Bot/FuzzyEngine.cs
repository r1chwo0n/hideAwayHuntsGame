using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzzyEngine
{
    public Dictionary<string, FuzzyVariable> inputs = new();
    public Dictionary<string, FuzzyVariable> outputs = new();
    public List<FuzzyRule> rules = new();

    public void AddInput(FuzzyVariable variable) => inputs[variable.name] = variable;
    public void AddOutput(FuzzyVariable variable) => outputs[variable.name] = variable;
    public void AddRule(FuzzyRule rule) => rules.Add(rule);

    public Dictionary<string, string> Evaluate(Dictionary<string, float> inputValues)
    {
        Dictionary<string, float> outputScores = new();
        Dictionary<string, string> outputDecision = new();

        foreach (var rule in rules)
        {
            float ruleStrength = 1f;

            foreach (var cond in rule.conditions)
            {
                var inputName = cond.Key;
                var setName = cond.Value;

                var fuzzySet = inputs[inputName].sets.FirstOrDefault(s => s.name == setName);
                float membership = fuzzySet.GetMembership(inputValues[inputName]);
                ruleStrength = Mathf.Min(ruleStrength, membership);
            }

            foreach (var conclusion in rule.conclusions)
            {
                string outputName = conclusion.Key;
                string setName = conclusion.Value;

                string key = $"{outputName}_{setName}";
                if (!outputScores.ContainsKey(key))
                    outputScores[key] = 0;

                outputScores[key] += ruleStrength * rule.weight;
            }
        }

        foreach (var output in outputs)
        {
            string outputName = output.Key;
            var relevant = outputScores
                .Where(kv => kv.Key.StartsWith(outputName + "_"))
                .OrderByDescending(kv => kv.Value)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(relevant.Key))
            {
                string setName = relevant.Key.Split('_')[1];
                outputDecision[outputName] = setName;
            }
        }

        return outputDecision;
    }
}
