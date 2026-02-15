using UnityEngine;

[CreateAssetMenu(menuName = "Game/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    [Header("Spawn Interval (seconds) vs Distance")]
    public AnimationCurve spawnIntervalByDistance = AnimationCurve.Linear(0, 2.0f, 1000, 0.7f);

    [Header("Chance of Enemy 3 vs Distance (0~1)")]
    public AnimationCurve enemy3ChanceByDistance = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(100, 0),
        new Keyframe(400, 0.15f),
        new Keyframe(1000, 0.35f),
        new Keyframe(2000, 0.5f)
    );

    [Header("Total spawn weight for enemy 1/2 (split equally)")]
    [Range(0f, 1f)] public float enemy12Split = 0.5f;

    public float GetSpawnInterval(float distance)
        => Mathf.Clamp(spawnIntervalByDistance.Evaluate(distance), 0.2f, 10f);

    public (float w1, float w2, float w3) GetWeights(float distance)
    {
        float c3 = Mathf.Clamp01(enemy3ChanceByDistance.Evaluate(distance));
        float rest = 1f - c3;
        float w1 = rest * enemy12Split;
        float w2 = rest * (1f - enemy12Split);
        float w3 = c3;
        return (w1, w2, w3);
    }
}
