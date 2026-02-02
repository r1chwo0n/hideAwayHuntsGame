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

    public Dictionary<string, float> Evaluate(Dictionary<string, float> inputValues)
    {
        Dictionary<string, float> numerator = new(); // Sum of (ruleStrength * centroid)
        Dictionary<string, float> denominator = new(); // Sum of ruleStrength

        foreach (var rule in rules) // วนทุก rules
        {
            float ruleStrength = 1f;

            // AND = min
            foreach (var cond in rule.conditions) // วนทุกเงื่อนไขใน rule
            {
                var set = inputs[cond.Key].sets
                    .First(s => s.name == cond.Value);

                ruleStrength = Mathf.Min(
                    ruleStrength,
                    set.GetMembership(inputValues[cond.Key])
                );
            }

            foreach (var concl in rule.conclusions)
            {
                var outputSet = outputs[concl.Key].sets
                    .First(s => s.name == concl.Value);

                if (!numerator.ContainsKey(concl.Key))
                {
                    numerator[concl.Key] = 0f;
                    denominator[concl.Key] = 0f;
                }

                numerator[concl.Key] += ruleStrength * outputSet.centroid;
                denominator[concl.Key] += ruleStrength;
            }
        }

        Dictionary<string, float> result = new();

        foreach (var key in numerator.Keys)
        {
            result[key] = denominator[key] > 0
                ? numerator[key] / denominator[key]
                : 0f;
        }

        return result;
    }
}
