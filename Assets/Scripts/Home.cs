using UnityEngine;
using UnityEngine.SceneManagement;


public class Home : MonoBehaviour
{
    // Pour retourner à l'accueil quand on clicke sur le BTN
    public void RetourAccueil()
    {
        // Désactive la musique principale
        GameObject camera = GameObject.Find("Main Camera");
        AudioSource audioPrincipal = camera.GetComponent<AudioSource>();
        audioPrincipal.enabled = false;

        // Arrête la musique stressante si elle est active
        GameObject stress = GameObject.Find("Stress");

        if (stress != null && stress.activeInHierarchy)
        {
            AudioSource audioStress = stress.GetComponentInChildren<AudioSource>(true);
            audioStress.Stop();
        }

        // Retourne à l'accueil
        SceneManager.LoadScene("Accueil");
    }
}