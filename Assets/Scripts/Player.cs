using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Player: MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    private BaseCounter selectedCounter;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private KitchenObject kitchenObject;
    
    public bool IsWalking() {
        return isWalking;
    }


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }
    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        //Only check for the selected counter because it's the one being interacted
        if(selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        HandlleMovement();
        HandleInteractions();
    }

    private void HandlleMovement() {
        Vector2 inputVector = gameInput.GetMovementVector2Normalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerHeight = 2f;
        float playerRadius = .7f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            //Cannot move towards moveDir

            //Attemp only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                //Can move only on the X
                moveDir = moveDirX;
            } else {
                //Cannot move only on the X

                //Attemp only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove) {
                    //Can move only on the Y
                    moveDir = moveDirZ;
                } else {
                    //Cannot move in any directions
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVector2Normalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 1.75f;
        if(Physics.Raycast(transform.position, lastInteractDir,out RaycastHit raycastHit,interactDistance, countersLayerMask)) {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if(baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectHoldPoint() {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
