using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class Skill : MonoBehaviour
{
    [SerializedDictionary("Color", "Amount")]
    public SerializedDictionary<GunColor, int> requiredColors;
    protected GunColor primaryOne;
    protected GunColor primaryTwo;

    [SerializeField] protected WeaponSystem ws;
    [SerializeField] protected KeyCode key;

    protected virtual void Start()
    {
        var e = requiredColors.GetEnumerator();
        e.MoveNext();
        primaryOne = e.Current.Key;
        e.MoveNext();
        primaryTwo = e.Current.Key;
    }
}
