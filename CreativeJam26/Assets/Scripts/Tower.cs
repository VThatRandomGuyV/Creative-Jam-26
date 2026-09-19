using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Tower
{
    public string name;
    public int cost;
    public GameObject prefab;

    public Tower (string _name, int _cost, GameObject _prefab)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
    }
    // Start is ca
    // lled once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
}
