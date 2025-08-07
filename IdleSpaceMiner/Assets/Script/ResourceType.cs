using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "IdleSpaceMiner/Resource")]
public class ResourceType : ScriptableObject
{
    public string resourceName;
    public Sprite icon;
    public int baseValue;
}