using System.Collections.Generic;
using System;

public class FuzzySet
{
    public string name;
    public Func<float, float> membershipFunc;

    public FuzzySet(string name, Func<float, float> membershipFunc)
    {
        this.name = name;
        this.membershipFunc = membershipFunc;
    }

    public float GetMembership(float value) => membershipFunc(value);
}

public class FuzzyVariable
{
    public string name;
    public List<FuzzySet> sets = new();

    public FuzzyVariable(string name) => this.name = name;

    public void AddSet(FuzzySet set) => sets.Add(set);

    public Dictionary<string, float> Fuzzify(float input)
    {
        var result = new Dictionary<string, float>();
        foreach (var set in sets)
            result[set.name] = set.GetMembership(input);
        return result;
    }
}
