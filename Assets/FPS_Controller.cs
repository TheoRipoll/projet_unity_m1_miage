using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;


[RequireComponent(typeof(CharacterController))] //Ajoute un CharacterController si il n'y en a pas
public class FPS_Controller : MonoBehaviour
{
    //Attributs
    public Camera playerCamera; //Camera du joueur
    public float walkSpeed = 6f; //Vitesse de marche
    public float runSpeed = 12f; //Vitesse de course
    public float jumpPower = 7f; //Puissance du saut
    public float gravity = 10f; //Gravité

    //Sensibilité caméra
    public float lookSpeed = 2f; //Vitesse
    public float lookXLimit = 45f; //Limite 

    Vector3 moveDirection = Vector3.zero; //Direction du mouvement
    float rotationX = 0; //Rotation de la caméra

    public bool canMove = true; //Si le joueur peut bouger

    CharacterController characterController; //Controleur du joueur

    void Start()
    {
        characterController = GetComponent<CharacterController>(); //Récupère le CharacterController
        Cursor.lockState = CursorLockMode.Locked; //Bloque le curseur au milieu de l'écran
        Cursor.visible = false; //Cache le curseur
    }

    void Update()
    {

        #region deplacement
        // Récupère les touches de déplacement
        Vector3 forward = transform.TransformDirection(Vector3.forward); 
        Vector3 right = transform.TransformDirection(Vector3.right); 

        // Maintenir Left Shift pour courrir
        bool isRunning = Input.GetKey(KeyCode.LeftShift); //Mode course lorsque la touche Shift est maintenue
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0; //Vitesse de déplacement en X
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * -Input.GetAxis("Horizontal") : 0; //Vitesse de déplacement en Y
        float movementDirectionY = moveDirection.y; //Direction du mouvement en Y
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); //Direction du mouvement
        #endregion

        #region saut
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded){moveDirection.y = jumpPower;} //Saut
        else{moveDirection.y = movementDirectionY;} //Si le joueur ne saute pas, il continue de tomber
        if (!characterController.isGrounded){moveDirection.y -= gravity * Time.deltaTime;} //Si le joueur n'est pas au sol, il tombe
        #endregion

        #region rotation
        characterController.Move(moveDirection * Time.deltaTime); //Déplace le joueur

        if (canMove) //Si le joueur peut bouger, il peut aussi regarder autour de lui
        {
            rotationX += Input.GetAxis("Mouse Y") * lookSpeed; //Récupère la rotation de la souris
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); //Limite la rotation de la caméra
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 180); //Applique la rotation de la caméra
            transform.rotation *= Quaternion.Euler(0, -Input.GetAxis("Mouse X") * lookSpeed, 0); //Applique la rotation du joueur
        }
        #endregion
    }
}