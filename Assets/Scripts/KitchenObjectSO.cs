using UnityEngine;

[CreateAssetMenu(fileName ="KitchenFoods")]
public class KitchenObjectSO : ScriptableObject {
    
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}