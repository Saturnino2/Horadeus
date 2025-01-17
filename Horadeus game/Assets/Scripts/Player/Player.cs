using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {

    public SO_PlayerInventory inventory;
    public Weapon currentWeapon;

    public PlayerMovement movement;

    private ItemData arrowItem;

    [HideInInspector] public HCamera playerCamera;

    public void Init(HCamera cam) {
        Debug.Log("Init player");

        playerCamera = cam;

        SwitchCursorLock();

        arrowItem = inventory.GetItem(ItemType.Arrow);
        arrowItem.count = 50; //you can enable this for playtesting, it gives you 50 arrows at the start of the game
        movement.Init(cam);

        currentWeapon.Equip(this);
    }

    public void InternalUpdate() {
        movement.InternalUpdate();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SwitchCursorLock();
        }

        SomeCursorCheck();

        if (currentWeapon != null) {
            if (Input.GetMouseButtonDown(0)) //if mouse pressed
            {
                    currentWeapon.UseStart(); 
                    if (arrowItem.count > 0) { //check if the player has any arrows
                        GameUI.inst.EnableCrosshair(true); //make crosshair visible
                    }
                    if (arrowItem.count <= 0) { //if the player has no arrows
                        GameUI.inst.EnableNoAmoSign(true); //make no amo sign visible instead of crosshair
                    }
            }

            if (Input.GetMouseButton(0)) //when holding mouse button
            {
                currentWeapon.UseHold();
                playerCamera.SetZoomPercent(currentWeapon.charge / currentWeapon.maxChargeTime);//make the camera zoom in
            }

            if (Input.GetMouseButtonUp(0)) //if mouse button is released
            {
                if (arrowItem.count > 0) {//if the player has more than 0 arrows
                    currentWeapon.UseRelease(); //"UseRelease" is responsible for shooting arrows
                    inventory.TakeItem(arrowItem, 1); //lower the arrow counter by 1
                }

                playerCamera.SetZoomPercent(0f); //disable camera zoom
                GameUI.inst.EnableCrosshair(false); //make crosshair invisible
                GameUI.inst.EnableNoAmoSign(false); //make NoAmoSign invisible
                
            }
        }
    }

    private void SomeCursorCheck()
    {
        // Gets the cursor position relative to the game window
        Vector2 cursorPosition = playerCamera.cameraComponent.ScreenToViewportPoint(Input.mousePosition);
        // Scales the position properly
        cursorPosition.x *= playerCamera.cameraComponent.scaledPixelWidth;
        cursorPosition.y *= playerCamera.cameraComponent.scaledPixelHeight;

        // Makes cursor invisible only when it's within the game window
        //if (movement.isInWindow(cursorPosition))
        //{
        //    Cursor.visible = false;
        //} else
        //{
        //    Cursor.visible = true;
        //} //This dose not help during building up the game it just keeps unlocking the cursor fromt he game.
    }

    public void InternalFixedUpdate()
    {
        movement.InternalFixedUpdate();
    }

    private void SwitchCursorLock() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}