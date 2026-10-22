using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalMods
{
    static float neutral = 1f;
    static float advantage = 1.5f;
    //static float disadvantage = 0.5f;

    public static float Modifier(ElementalTypes skillElement, ElementalTypes enemyElement)
    {
        float elementMod = neutral;

        if (skillElement == ElementalTypes.Power)
        {
            if (enemyElement == ElementalTypes.Money)
            {
                elementMod = advantage;
            }
        }
        else if (skillElement == ElementalTypes.Money)
        {
            if (enemyElement == ElementalTypes.Fame)
            {
                elementMod = advantage;
            }
        }
        else if (skillElement == ElementalTypes.Fame)
        {
            if (enemyElement == ElementalTypes.Power)
            {
                elementMod = advantage;
            }
        }
        else if (skillElement == ElementalTypes.Corruption)
        {
            if (enemyElement == ElementalTypes.Power || skillElement == ElementalTypes.Money || skillElement == ElementalTypes.Fame)
            {
                elementMod = advantage;
            }
        }
        else if (skillElement == ElementalTypes.Purity)
        {
            if (enemyElement == ElementalTypes.Corruption)
            {
                elementMod = advantage;
            }
        }

        return elementMod;
    }
}

public enum ElementalTypes
{
    Power,
    Money,
    Fame,
    Corruption,
    Purity
}
