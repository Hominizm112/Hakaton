using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;

public class DoTweenUtils
{
    public float value;

    public Tween DoValue(float startValue, float endValue, float duration)
    {
        value = startValue;

        return DOTween.To(
            () => value,
            x => value = x,
            endValue,
            duration
        );
    }
}