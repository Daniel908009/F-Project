using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    private Dictionary<PowerCircuit, bool> powerStates = new Dictionary<PowerCircuit, bool>();

public static PowerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        foreach (PowerCircuit circuit in Enum.GetValues(typeof(PowerCircuit)))
        {
            powerStates[circuit] = false;
        }
    }

    public void SetPower(PowerCircuit circuit, bool value)
    {
        powerStates[circuit] = value;
    }

    public bool IsPowered(PowerCircuit circuit)
    {
        if (circuit == PowerCircuit.MainPower)
            return powerStates[PowerCircuit.MainPower];

        return powerStates[PowerCircuit.MainPower] &&
               powerStates[circuit];
    }
}