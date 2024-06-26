using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class Skill : MonoBehaviour
{
    [SerializedDictionary("Color", "Amount")]
    public SerializedDictionary<GunColor, int> requiredColors;
    public GunColor primaryOne { get; protected set; }
    public GunColor primaryTwo{ get; protected set; }

    [SerializeField] protected WeaponSystem ws;
    [SerializeField] protected KeyCode key;

    protected virtual void Awake()
    {
        var e = requiredColors.GetEnumerator();
        e.MoveNext();
        primaryOne = e.Current.Key;
        e.MoveNext();
        primaryTwo = e.Current.Key;
    }
}
