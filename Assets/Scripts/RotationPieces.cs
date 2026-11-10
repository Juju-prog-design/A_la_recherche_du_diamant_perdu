using UnityEngine;


public class RotationPieces : MonoBehaviour
{
    // Vitesse de rotation
    [SerializeField] private float vitesseRotation = -30f;


    // Est appelée à chaque frame
    void Update()
    {
        // Fait tourner la pièce (axe des Y) selon le temps écoulé
        transform.Rotate(0f, vitesseRotation * Time.deltaTime, 0f);
    }
}