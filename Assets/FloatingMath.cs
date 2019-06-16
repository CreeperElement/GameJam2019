using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMath
{
    public static bool GreaterThan(float a, float b, float delta)
    {
        return a + delta > b || a - delta > b;
    }

    public static bool LessThan(float a, float b, float delta)
    {
        return a + delta < b || a - delta < b;
    }

    public static bool Equal(float a, float b, float delta)
    {
        return (a - b) <= delta;
    }
}
