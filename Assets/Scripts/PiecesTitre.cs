using UnityEngine;


public class PiecesTitre : MonoBehaviour
{
    //Est appel�e � chaque frame
    void Update()
    {
        //Rotate sur son axe des Y
        transform.Rotate(0, -0.2f, 0);
    }
}
