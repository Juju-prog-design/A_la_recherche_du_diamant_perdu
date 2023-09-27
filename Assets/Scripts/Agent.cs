using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour
{
    //Composante NavMeshAgent des ennemis ainsi que leur cible (le perso)
    [SerializeField] NavMeshAgent agent;


    //Est appelée à chaque frame
    void Update()
    {
        //Fait en sorte que les ennemis restent à une hauteur minimum (sur l'axe des Y)
        if (transform.position.y < 4f)
        {
            transform.position = new Vector3(transform.position.x, 4f, transform.position.z);
        }


        //Donne la destination (perso) aux ennemis
        agent.destination = GameObject.Find("Perso").transform.position;

        //Permet que les ennemis suivent du regard le perso
        agent.updateRotation = true;
    }
}
