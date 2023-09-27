using UnityEngine;


public class RotationPieces : MonoBehaviour
{
    //Est appelée à chaque frame
    void Update()
    {
        //Rotate sur son axe des Y
        transform.Rotate(0, -0.5f, 0);
    }
}
