using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class Skill : MonoBehaviour
{
    [SerializedDictionary("Color", "Amount")]
    public SerializedDictionary<GunColor, int> requiredColors;
    public GunColor PrimaryOne { get; protected set; }
    public GunColor PrimaryTwo{ get; protected set; }

    [SerializeField] protected WeaponSystem ws;
    [SerializeField] protected KeyCode key;

    protected virtual void Awake()
    {
        var e = requiredColors.GetEnumerator();
        e.MoveNext();
        PrimaryOne = e.Current.Key;
        e.MoveNext();
        PrimaryTwo = e.Current.Key;
    }
}
