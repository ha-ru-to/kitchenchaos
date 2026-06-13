using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //There is no KitchenObject
            if (player.HasKitchenObject()) {
                //Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                //Player is not carrying something
            }
        } else {
            //There is a KitchenObject here
            if (player.HasKitchenObject()) {
                //Player is carrying something
            } else {
                //Player is not carrying something
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}