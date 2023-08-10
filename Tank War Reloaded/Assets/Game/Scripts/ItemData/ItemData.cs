using HAVIGAME;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Data/ItemData")]
public class ItemData : ScriptableObject, IIdentify<int> {
    [SerializeField, ConstantField(typeof(ItemID))] private int id;
    [SerializeField, SpriteField] private Sprite icon;
    [SerializeField] private string displayName;
    [SerializeField, TextArea(3,5)] private string description;

    public int Id => id;
    public Sprite Icon => icon;
    public string Name => displayName;
    public string Description => description;
}
