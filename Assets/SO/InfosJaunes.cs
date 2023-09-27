using UnityEngine;


//Permet de créer un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosJaunes", menuName = "SO/NouveauJaune")]


public class InfosJaunes : ScriptableObject
{
    //Nb de pts de la pièce
    public int nbPts;
}