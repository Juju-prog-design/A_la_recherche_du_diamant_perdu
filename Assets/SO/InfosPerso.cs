using UnityEngine;


//Permet de cr�er un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosPerso", menuName = "SO/NouveauPerso")]


public class InfosPerso : ScriptableObject
{
    //Nom du perso
    public string nomPerso;

    //Nb pts
    public int nbPts = 0;

    //Nb d'ennemis tu�s par le perso
    public int nbEnnemis;

    //Temps du chrono
    public int temps;

    //Nb de PV du perso
    public int nbPV;

    //Nb de pi�ces du perso
    public int nbPieces;
}