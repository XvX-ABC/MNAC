using System;
using UnityEngine;

public class ComponentCantFindException : Exception
{
    public ComponentCantFindException(string message) : base(message)
    {

    }
    public ComponentCantFindException(GameObject obj, Type componentType) : this($"Can't find a component by type '{componentType.Name}' from object '{obj.name}'.")
    {

    }
}
