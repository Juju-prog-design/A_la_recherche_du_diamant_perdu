using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour
{
    // Composante "NavMeshAgent" des ennemis et leur cible (le perso)
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform cible;

    void Start()
    {
        // Active la rotation automatique de l'ennemi
        agent.updateRotation = true;
    }

    void Update()
    {
        // Fait en sorte que l'ennemi reste à une hauteur minimum
        if (transform.position.y < 4f)
            transform.position = new Vector3(transform.position.x, 4f, transform.position.z);

        // Donne la destination à l'ennemi
        agent.destination = cible.position;
    }
}