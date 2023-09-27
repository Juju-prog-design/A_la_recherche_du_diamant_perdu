using UnityEngine;


public class PiecesTitre : MonoBehaviour
{
    //Est appelée à chaque frame
    void Update()
    {
        //Rotate sur son axe des Y
        transform.Rotate(0, -0.2f, 0);
    }
}
