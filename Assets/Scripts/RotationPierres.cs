using UnityEngine;


public class RotationPierres : MonoBehaviour
{
    // Vitesse de rotation
    [SerializeField] private float vitesseRotation = -0.1f;


    // Est appelée à chaque frame
    void Update()
    {
        // Fait tourner la pierre (axe des Z) selon le temps écoulé
        transform.Rotate(0f, 0f, vitesseRotation * Time.deltaTime);
    }
}
