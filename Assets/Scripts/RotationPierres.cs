using UnityEngine;

public class RotationPierres : MonoBehaviour
{
    //Est appel�e � chaque frame
    void Update()
    {
        //Rotate sur son axe des Z
        transform.Rotate(0, 0, -0.1f);
    }
}
