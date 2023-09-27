using UnityEngine;
using UnityEngine.SceneManagement;


public class Home : MonoBehaviour
{
    public void RetourAccueil()
    {
        //Trouve la caméra principale et lui dire de fermer sa musique
        GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

        //Charge la scène "Accueil"
        SceneManager.LoadScene("Accueil");
    }
}