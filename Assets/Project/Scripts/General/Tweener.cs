using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class Tweener
{
    public List<Tween> activeTweens = new();
    public void AddTween(Tween tween)
    {
        activeTweens.Add(tween);
    }

    public Tween GetLastTween()
    {
        return activeTweens[activeTweens.Count - 1];
    }

    public void KillTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween.Kill();
        }

        activeTweens.Clear();
    }


}
