using UnityEngine;
using System;

public class DistanceTracker : MonoBehaviour
{
    public float DistanceMeters { get; private set; }
    public event Action<float> OnDistanceChanged;
    public float metersPerSecond = 5f;

    public void AddDistance(float meters)
    {
        DistanceMeters += meters;
        OnDistanceChanged?.Invoke(DistanceMeters);
    }

    void Update()
    {
        DistanceMeters += metersPerSecond * Time.deltaTime;
        Debug.Log(DistanceMeters); // 测试用的，记得删
    }

    public int DifficultyTier => Mathf.FloorToInt(DistanceMeters / 100f);
}
