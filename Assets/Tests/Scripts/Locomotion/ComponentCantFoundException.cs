using System;
using UnityEngine;

public class ComponentCantFoundException : Exception
{
    public ComponentCantFoundException(string message) : base(message)
    {

    }
    public ComponentCantFoundException(GameObject obj, Type componentType) : this($"Can't found a component by type '{componentType.Name}' from object '{obj.name}'.")
    {

    }
}
