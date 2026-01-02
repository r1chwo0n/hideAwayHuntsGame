using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FormFuzzyAITests
{
    FormFuzzyAISetup setup;
    ActiveFormSelector selector;

    SituationSummary MakeSituation(
        float nearest,
        float avg,
        int density,
        int usSeeing,
        int enemiesSeeing)
    {
        return new SituationSummary
        {
            nearestEnemyDistance = nearest,
            avgEnemyDistance = avg,
            enemyCountInRange = density,
            usSeeingEnemies = usSeeing,
            enemiesSeeingUs = enemiesSeeing
        };
    }

    [SetUp]
    public void Setup()
    {
        var go = new GameObject("FuzzySetup");
        setup = go.AddComponent<FormFuzzyAISetup>();

        var selGO = new GameObject("Selector");
        selector = selGO.AddComponent<ActiveFormSelector>();
        selector.fuzzySetup = setup;
    }

    [Test]
    public void Near_Surrounded_Blind_ShouldBeBad()
    {
        var s = MakeSituation(
            nearest: 10f,
            avg: 0.3f,
            density: 3,
            usSeeing: 0,
            enemiesSeeing: 3
        );

        float score = selector.Evaluate(s);

        Assert.Less(score, 0.4f);
    }

    [Test]
    public void Near_ClearSight_FewEnemies_ShouldBeGood()
    {
        var s = MakeSituation(
            nearest: 20f,
            avg: 0.4f,
            density: 1,
            usSeeing: 2,
            enemiesSeeing: 0
        );

        float score = selector.Evaluate(s);

        Assert.Greater(score, 0.7f);
    }

    [Test]
    public void Far_Unaware_ShouldBeOk()
    {
        var s = MakeSituation(
            nearest: 160f,
            avg: 0.9f,
            density: 0,
            usSeeing: 0,
            enemiesSeeing: 0
        );

        float score = selector.Evaluate(s);

        Assert.IsTrue(score >= 0.4f && score <= 0.7f);
    }

}