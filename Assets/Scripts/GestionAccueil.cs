using UnityEngine;
using UnityEngine.SceneManagement;


public class GestionAccueil : MonoBehaviour
{
    // Infos du perso (SO)
    [SerializeField] private InfosPerso infosPerso;


    // Est appelée au lancement de la scène (avant tout)
    void Awake()
    {
        // Réinitialise le nom du perso
        infosPerso.nomPerso = "";
    }


    // Changer le nom du perso quand on écris qq dans la zone de texte
    public void ChangeNom(string nom)
    {
        infosPerso.nomPerso = nom;
    }


    // Vérifie si le nom du perso a bien été écrit dans la zone de texte
    public void VerifieNom()
    {
        // S'il est écrit quelque chose (donc pas rien), charge la scène suivante
        if (!string.IsNullOrWhiteSpace(infosPerso.nomPerso))
            SceneManager.LoadScene("Terrain");
    }
}