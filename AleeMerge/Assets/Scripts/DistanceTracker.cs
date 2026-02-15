using UnityEngine;
using System;

public class DistanceTracker : MonoBehaviour
{
    public float DistanceMeters { get; private set; }
    public event Action<float> OnDistanceChanged;
    public float metersPerSecond = 5f; // 由你的世界滚动速度赋值

    public void AddDistance(float meters)
    {
        DistanceMeters += meters;
        OnDistanceChanged?.Invoke(DistanceMeters);
    }

    void Update()
    {
        DistanceMeters += metersPerSecond * Time.deltaTime;
        Debug.Log(DistanceMeters);
    }

    public int DifficultyTier => Mathf.FloorToInt(DistanceMeters / 100f);
}
