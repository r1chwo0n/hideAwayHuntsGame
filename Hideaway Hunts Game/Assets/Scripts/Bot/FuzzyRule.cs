using System.Collections.Generic;

public class FuzzyRule
{
    public Dictionary<string, string> conditions = new(); // InputName -> FuzzySetName
    public Dictionary<string, string> conclusions = new(); // OutputName -> FuzzySetName
    public float weight = 1f;

    public FuzzyRule AddCondition(string inputName, string fuzzySetName)
    {
        conditions[inputName] = fuzzySetName;
        return this;
    }

    public FuzzyRule AddConclusion(string outputName, string fuzzySetName)
    {
        conclusions[outputName] = fuzzySetName;
        return this;
    }
}
