using UnityEngine;


//Permet de créer un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosPerso", menuName = "SO/NouveauPerso")]


public class InfosPerso : ScriptableObject
{
    //Nom du perso
    public string nomPerso;

    //Nb pts
    public int nbPts = 0;

    //Nb d'ennemis tués par le perso
    public int nbEnnemis;

    //Temps du chrono
    public int temps;

    //Nb de PV du perso
    public int nbPV;

    //Nb de pièces du perso
    public int nbPieces;
}