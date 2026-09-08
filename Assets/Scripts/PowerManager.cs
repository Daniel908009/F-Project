using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CircuitPower
{
    public PowerCircuit circuit;
    public float powerConsumption;
}

public class PowerManager : MonoBehaviour
{
    private Dictionary<PowerCircuit, bool> powerStates = new Dictionary<PowerCircuit, bool>();
    [SerializeField] private SwitchInteractable[] switches;
    [SerializeField] private List<CircuitPower> circuitPowers = new List<CircuitPower>();
    [SerializeField] private float totalPowerAvailable = 100f;
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
    private void Update()
    {
        float totalPowerConsumption = GetCurrentPower();

        if (totalPowerConsumption > totalPowerAvailable)
        {
            foreach (CircuitPower circuitPower in circuitPowers)
            {
                SetPower(circuitPower.circuit, false, true);
            }
        }
    }
    public void SetPower(PowerCircuit circuit, bool value, bool isFromOverload = false)
    {
        powerStates[circuit] = value;
        if (isFromOverload)
        {
            foreach (SwitchInteractable switchInteractable in switches)
            {
                if (switchInteractable != null && switchInteractable.GetPowerCircuit() == circuit)
                {
                    switchInteractable.SetSwitchState(value);
                }
            }
        }
    }

    public bool IsPowered(PowerCircuit circuit)
    {
        if (circuit == PowerCircuit.MainPower)
            return powerStates[PowerCircuit.MainPower];

        return powerStates[PowerCircuit.MainPower] &&
               powerStates[circuit];
    }
    public float GetCurrentPower()
    {
        float currentPowerConsumption = 0f;
        foreach (CircuitPower circuitPower in circuitPowers)
        {
            if (IsPowered(circuitPower.circuit))
            {
                currentPowerConsumption += circuitPower.powerConsumption;
            }
        }
        return currentPowerConsumption;
    }
    public float GetTotalPower()
    {
        return totalPowerAvailable;
    }
}