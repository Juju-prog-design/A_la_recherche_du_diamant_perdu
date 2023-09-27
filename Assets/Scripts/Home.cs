using UnityEngine;
using UnityEngine.SceneManagement;


public class Home : MonoBehaviour
{
    public void RetourAccueil()
    {
        //Trouve la cam�ra principale et lui dire de fermer sa musique
        GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

        //Charge la sc�ne "Accueil"
        SceneManager.LoadScene("Accueil");
    }
}