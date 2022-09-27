using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Characteristics", menuName = "Battle/New Characteristics", order = 0)]
public class BattleCharacteristics : ScriptableObject
{
    [Header("Settings")]
    [Range(0, 100)] public float hp = 100;
    public float damage = 1;
}