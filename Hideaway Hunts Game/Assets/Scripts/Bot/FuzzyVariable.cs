using System.Collections.Generic;
using System;

public class FuzzySet
{
    public string name;
    public Func<float, float> membershipFunc;
    public float centroid;

    // INPUT (ไม่ใช้ centroid)
    public FuzzySet(string name, Func<float, float> membershipFunc)
    {
        this.name = name;
        this.membershipFunc = membershipFunc;
    }

    // OUTPUT (ใช้ centroid)
    public FuzzySet(string name, float centroid)
    {
        this.name = name;
        this.centroid = centroid;
    }

    public float GetMembership(float value) => membershipFunc(value);
}

public class FuzzyVariable
{
    public string name; // ชื่อตัวแปร 
    public List<FuzzySet> sets = new(); // ชุดฟัซซี่

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
