using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
    

    public event EventHandler<IHasProgress.onProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }


    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOAraay;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOAraay;
    

    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;


    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        if (HasKitchenObject()) {
            switch (state) {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                        progressNormalized = (float)fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
                        // Fried
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetburningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                        progressNormalized = (float)burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax) {
                        // Fried
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                        OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }  
            
        }
   }
    public override void Interact(Player player){
        if (!HasKitchenObject())
        {
            // There is no kitchen object on the counter
            if (player.HasKitchenObject()){
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    // Player is carrying something that can be Fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                    OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                        progressNormalized = (float)fryingTimer / fryingRecipeSO.fryingTimerMax
                    });
                }
            } else {
                //Player not carrying anything
            }
        } else {
            // There is a kitchen object on the counter
            if (player.HasKitchenObject()){
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                         state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                        OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }  
                }
            } else {
                //Player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                OnProgressChanged?.Invoke(this, new IHasProgress.onProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO frinygRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (frinygRecipeSO != null){
            return frinygRecipeSO.output;
        } else {
            return null;
        }
    }

    private  FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOAraay){
            if (fryingRecipeSO.input == inputKitchenObjectSO){
                return fryingRecipeSO;
            }
        }
        return null;
    }


    private  BurningRecipeSO GetburningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOAraay){
            if (burningRecipeSO.input == inputKitchenObjectSO){
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried() {
        return state == State.Fried;
    }

}
