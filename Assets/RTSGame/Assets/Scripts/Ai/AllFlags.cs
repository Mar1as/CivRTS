using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllFlags
{
    private static AllFlags instance = new AllFlags();
    public static AllFlags Instance => instance;


    public List<SimpleZone> flags = new List<SimpleZone>();
}
