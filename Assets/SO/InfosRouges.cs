using UnityEngine;


//Permet de créer un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosRouges", menuName = "SO/NouveauRouge")]


public class InfosRouges : ScriptableObject
{
    //Nb de pts de la pièce
    public int nbPts;
}