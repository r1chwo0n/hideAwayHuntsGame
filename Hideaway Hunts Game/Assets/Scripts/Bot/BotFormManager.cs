using System.Collections.Generic;
using UnityEngine;

public class BotFormManager : MonoBehaviour
{
    [Header("Bot Forms")]
    public List<Transform> forms = new();

    // จำนวนร่างที่ยังมีชีวิตอยู่
    public int AliveFormsCount => forms.Count;

    void Awake()
    {
        foreach (var form in forms)
        {
            if (!form) continue;

            var killable = form.GetComponent<Killable>();

            if (killable != null)
                killable.OnKilled += OnFormKilled;
            else
                Debug.LogWarning(form.name + " has no Killable component");
        }
    }

    // ===== PUBLIC API =====

    public void RegisterForm(Transform form)
    {
        if (!form) return;
        if (forms.Contains(form)) return;

        forms.Add(form);

        var killable = form.GetComponent<Killable>();
        if (killable != null)
        {
            killable.OnKilled += OnFormKilled;
        }
        else
        {
            Debug.LogWarning($"{form.name} has no Killable component");
        }
    }

    public void UnregisterForm(Transform form)
    {
        if (!form) return;

        if (forms.Remove(form))
        {
            var killable = form.GetComponent<Killable>();
            if (killable != null)
            {
                killable.OnKilled -= OnFormKilled;
            }
        }
    }

    // ===== EVENT HANDLER =====

    void OnFormKilled(Transform deadForm)
    {
        UnregisterForm(deadForm);
    }
}
