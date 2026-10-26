using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BougePerso : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Vitesse de marche du perso")]
    [SerializeField] private float vitesseMarche = 6.0f;

    [Tooltip("Vitesse de course du perso")]
    [SerializeField] private float vitesseCourse = 15.0f;

    [Tooltip("Vitesse de rotation du perso")]
    [SerializeField] private float vitesseRotation = 1.0f;

    [Tooltip("Accélération et décélération du perso")]
    [SerializeField] private float accelerationDeceleration = 10.0f;


    [Space(5)]
    [Header("Saut")]
    [Tooltip("Hauteur du saut du perso")]
    [SerializeField] private float hauteurSaut = 1.2f;

    [Tooltip("Valeur de gravité (défaut = -9.81f)")]
    [SerializeField] private float Gravite = -15.0f;

    [Tooltip("Temps nécessaire pour sauter à nouveau")]
    [SerializeField] private float delaiSaut = 0.1f;

    [Tooltip("Temps nécessaire pour être considéré dans l'état de chute")]
    [SerializeField] private float delaiChute = 0.15f;

    // Délai nécessaire pour sauter à nouveau
    private float sautTimeout;

    // Délai nécessaire pour être considéré dans l'état de chute
    private float chuteTimeout;

    // Booléen pour savoir le début de l'animation de saut (avec et sans arme)
    private bool debutAnim = false;


    [Space(5)]
    [Header("Perso au sol")]
    [Tooltip("Détermine si le perso touche le sol ou non")]
    [SerializeField] private bool toucheSol = true;

    [Tooltip("Distance du perso du sol")]
    [SerializeField] private float distanceDuSol = -0.14f;

    [Tooltip("Radius du Character controller pour savoir si le perso touche au sol ou non")]
    [SerializeField] private float GroundedRadius = 0.5f;

    [Tooltip("Layer du sol")]
    [SerializeField] private LayerMask layerSol;


    [Space(5)]
    [Header("Cinemachine")]
    [Tooltip("GameObject que la Cinemachine va suivre")]
    [SerializeField] private GameObject cibleCinemachine;

    [Tooltip("Nb de degrés de la caméra pour se déplacer vers le haut")]
    [SerializeField] private float maxMouvCamHaut = 90.0f;

    [Tooltip("Nb de degrés de la caméra pour se déplacer vers le bas")]
    [SerializeField] private float maxMouvCamBas = -90.0f;

    // Cinémachine pour la 1re et 3e personne (cameraTP) ainsi que la caméra principale (cinemachineTP)
    [SerializeField] private GameObject cinemachineFP;
    [SerializeField] private GameObject cinemachineTP;
    [SerializeField] private GameObject cameraTP;

    // Valeur permettant d'ajuster la vélocité et le temps de rotation de la caméra 3e personne
    [SerializeField] private float ajusteVelocityRotation = 0.1f;
    [SerializeField] private float ajusteTempsRotation = 0.1f;

    // Seuil de la caméra
    private const float seuil = 0.01f;

    // Rotation X de la Cinemachine
    private float niveauCinemachine;


    [Space(5)]
    // Animator du perso ainsi que son arme et la mire
    [Header("Arme")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject arme;
    [SerializeField] GameObject mire;

    // Délai pour ranger l'arme et booléen pour savoir si l'arme est équipée ou non
    private float cooldown;
    private bool equipe = false;


    [Space(5)]
    [Header("Panneaux")]
    // Panneau du début
    [SerializeField] private GameObject panneauDebut;

    // Panneau au centre du village
    [SerializeField] private GameObject panneauInfos;

    // Panneau de la maison du maire
    [SerializeField] private GameObject panneauMaire;

    // Panneau des limites du village
    [SerializeField] private GameObject panneauLimites;

    // Panneau Options
    [SerializeField] private GameObject panneauOptions;

    [Space(5)]
    [Header("Coeurs")]
    [SerializeField] private GameObject PV4;
    [SerializeField] private GameObject PV3;
    [SerializeField] private GameObject PV2;
    [SerializeField] private GameObject PV1;


    [Space(5)]
    [Header("Vitesse")]
    // Vitesse
    private float vitesse;

    // Vitesse (vélocité) de rotation
    private float velociteRotation = 53.0f;

    // Vitesse verticale
    private float vitesseVerticale;

    // Vitesse finale
    private float vitesseFinale = 53.0f;

    // Valeur du changement de vitesse du perso
    private float changementVitesse;


    [Space(5)]
    [Header("Sons")]
    // GameObject avec le son des pièces jaunes
    [SerializeField] private GameObject piecesJaunes;

    // GameObject avec le son des pièces rouges
    [SerializeField] private GameObject piecesRouges;

    // GameObject avec le son de la douleur du perso
    [SerializeField] private GameObject ouch;

    // GameObject avec le son du perso qui tire avec son arme
    [SerializeField] private GameObject tire;

    // GameObject avec le son du perso qui recharge son arme
    [SerializeField] private GameObject recharge;

    // GameObject avec la musique stressante
    [SerializeField] private GameObject stress;

    [Space(5)]
    // Durée d'invincibilité
    [SerializeField] private float dureeInvincibilite = 1.5f;

    // Temps où le perso pourra reprendre des dégâts
    private float prochainDegatPossible = 0f;


    [Space(5)]
    [Header("Autres")]
    // Character controller du perso
    private CharacterController controller;

    // Diamant
    [SerializeField] private GameObject diamant;

    // Script "StarterAssetsInputs" du perso
    private StarterAssetsInputs lesInputs;

    // SO "InfosPerso" du perso
    [SerializeField] private InfosPerso infosPerso;

    // Zone de texte avec le chrono
    [SerializeField] private TMP_Text temps;

    // Temps actuel du chrono
    float currentTime = 0f;

    // Valeur de départ de temps du chrono
    float startingTime = 120;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------

    // Est appelée au lancement de la scène
    void Start()
    {
        // Réinitialise les valeurs du nb de pièces, des ennemis tués, de PV et le chrono au lancement de la scène
        infosPerso.nbPieces = 0;
        infosPerso.nbEnnemis = 0;
        infosPerso.nbPV = 4;
        currentTime = startingTime;

        // Trouve les composantes du Character controller et le script "StarterAssetsInputs" du perso
        controller = GetComponent<CharacterController>();
        lesInputs = GetComponent<StarterAssetsInputs>();

        // Réinitialise les délais de saut et de chute au lancement de la scène
        sautTimeout = delaiSaut;
        chuteTimeout = delaiChute;

        // Affiche le panneau du début
        panneauDebut.SetActive(true);

        // Invoque la méthode "EnleverPanneaux" dans 3 secondes
        Invoke("EnleverPanneaux", 3);
    }


    // Est appelée à chaque frame
    void Update()
    {
        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
        {
            // Exécute la méthode suivante
            Bouge();
        }

        // Sinon (quand on est 3e personne)
        else
        {
            // Exécute la méthode suivante
            DirectionTP();
        }

        // Lance le chrono et met à jour la valeur du temps du perso
        currentTime -= 1 * Time.deltaTime;
        temps.text = "Temps : " + currentTime.ToString("0");
        infosPerso.temps = (int)currentTime;

        // Si le temps est plus grand ou égal à 30
        if (currentTime <= 30f)
        {
            // Dit à la caméra principale d'arrêter sa musique
            GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

            // Active la musique stressante
            stress.SetActive(true);
        }

        // Si le temps est plus grand à 30
        if (currentTime > 30f)
        {
            // Désactive la musique stressante
            stress.SetActive(false);
        }

        // Si le temps est plus petit ou égal à 0
        if (currentTime <= 0)
        {
            // Met la valeur du temps à 0 (pour ne pas qu'elle continue à descendre)
            currentTime = 0;

            // Dit au GameObject ayant la musique stressante d'arrêter sa musique
            stress.SetActive(false);

            // Exécute la méthode "SceneDefaite"
            SceneDefaite();
        }

        // Exécute les méthodes suivantes à chaque frame
        Attaque();
        SautEtGravite();
        VerifierSol();
        DebutAnim();
        DiamantApparu();
        PointsVie();
    }


    // Est appelée après que les fonctions dans Update soient exécutées
    void LateUpdate()
    {
        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
        {
            // Exécute la méthode suivante
            RotationCamera();
        }
    }


    // -----------------------------------------------------------------------CINEMACHINES------------------------------------------------------------------

    // Permet à la caméra 3e personne de bouger
    void DirectionTP()
    {
        // Valeurs des mouvements horizontaux et verticaux de la caméra
        float gaucheDroite = lesInputs.bouge.x;
        float avantArriere = lesInputs.bouge.y;

        // Détermine si le perso marche ou cours (si marche = vitesse de marche, si course = vitesse de course)
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;


        // Vitesse horizontale de la caméra et marge de vitesse (droit à l'erreur à 0.1)
        float vitessseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float ecartVitesse = 0.1f;

        // Si la vitesse horizontale est plus petite que la vitesse voulue - l'écart de vitesse OU l'inverse
        if (vitessseHorizontale < vitesseVoulue - ecartVitesse || vitessseHorizontale > vitesseVoulue + ecartVitesse)
        {
            vitesse = Mathf.Lerp(vitessseHorizontale, vitesseVoulue, Time.deltaTime * accelerationDeceleration);
            vitesse = Mathf.Round(vitesse * 1000f) / 1000f;
        }

        // Sinon, la vitesse est égale à la vitesse voulue
        else
        {
            vitesse = vitesseVoulue;
        }


        // Rend plus fluide le changement de vitesse entre "Idle", "Marche" et "Course"
        changementVitesse = Mathf.Lerp(changementVitesse, vitesseVoulue, Time.deltaTime * accelerationDeceleration);

        // Si le changement de vitesse est plus petit que 0.1, met sa valeur à 0 (pour pas que la valeur soit négative)
        if (changementVitesse < 0.01f) changementVitesse = 0;

        // Donne la valeur du changement de vitesse à celle du float "Vitesse" de l'Animator du perso
        animator.SetFloat("Vitesse", changementVitesse);


        // Normalise les valeurs de mouvements
        Vector3 direction = new Vector3(gaucheDroite, vitesseVerticale, avantArriere).normalized;


        // Si la magnitude des valeurs de mouvements normalisées sont plus petites ou égales à 0.1 ET que celles de déplacements du perso ne sont pas égales à 0
        if (direction.magnitude >= 0.1f && lesInputs.bouge != Vector2.zero)
        {
            // Angle voulu pour faire rotater le perso ((transform.eulerAngles.y), car pivot perso = sur axe des Y)
            float angleVoulu = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cinemachineTP.transform.eulerAngles.y;

            // Angle de rotation du perso
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVoulu, ref ajusteVelocityRotation, ajusteTempsRotation);

            // Rotate le perso
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Direction du mouvement (normalisée) du perso selon l'angle de rotation
            Vector3 moveDir = (Quaternion.Euler(0f, angleVoulu, 0f) * Vector3.forward).normalized;

            // Bouge le perso selon les valeurs de mouvements normalisées (avec la rotation tout en prenant en compte s'il saute) avec son Character controller
            controller.Move(moveDir.normalized * (vitesseVoulue * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
        }

        // Sinon, bouge le perso selon ses valeurs de mouvements normalisées (tout en prenant en compte s'il saute) avec son Character controller
        else
        {
            controller.Move(direction.normalized * (vitesse * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
        }
    }


    // Rotation de la caméra quand le perso est en mode 1re personne
    void RotationCamera()
    {
        // Détecte si il y a un mouvement de la caméra
        if (lesInputs.regarder.sqrMagnitude >= seuil)
        {
            // Empêche les mouvements de la souris de multiplier la vitesse de la caméra (axes X et Y)
            float deltaTimeMultiplier = 1;
            niveauCinemachine += lesInputs.regarder.y * vitesseRotation * deltaTimeMultiplier;
            velociteRotation = lesInputs.regarder.x * vitesseRotation * deltaTimeMultiplier;

            // Met à jour les valeurs des angles possibles que la Cinemachine peut faire
            niveauCinemachine = GrandeurAngle(niveauCinemachine, maxMouvCamBas, maxMouvCamHaut);

            // Met à jour les valeurs de la Cinemachine
            cibleCinemachine.transform.localRotation = Quaternion.Euler(niveauCinemachine, 0.0f, 0.0f);

            // Permet au perso d'effectuer une rotation à gauche et à droite
            transform.Rotate(Vector3.up * velociteRotation);
        }
    }


    // Détermine les angles possibles que la Cinemachine peut faire
    static float GrandeurAngle(float angleGauche, float minGauche, float maxGauche)
    {
        // Empêche la Cinemachine d'avoir une rotation trop grande ou trop faible (- que -360 degrés ou + que +360 degrés)
        if (angleGauche < -360f) angleGauche += 360f;
        if (angleGauche > 360f) angleGauche -= 360f;
        return Mathf.Clamp(angleGauche, minGauche, maxGauche);
    }


    // Permet au perso de bouger quand il est en mode 1re personne
    void Bouge()
    {
        // Détermine si le perso marche ou cours (si marche = vitesse de marche, si course = vitesse de course)
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;


        // Vitesse horizontale du perso
        float vitesseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        // Décalage de vitesse (fluidité) et magnitude des vecteurs de déplacement
        float ecartVitesse = 0.1f;
        float inputMagnitude = 1f;

        // Si la vitesse horizontale est plus petite que la vitesse voulue - l'écart de vitesse OU l'inverse
        if (vitesseHorizontale < vitesseVoulue - ecartVitesse || vitesseHorizontale > vitesseVoulue + ecartVitesse)
        {
            vitesse = Mathf.Lerp(vitesseHorizontale, vitesseVoulue * inputMagnitude, Time.deltaTime * accelerationDeceleration);
            vitesse = Mathf.Round(vitesse * 1000f) / 1000f;
        }

        // Sinon, la vitesse est égale à la vitesse voulue
        else
        {
            vitesse = vitesseVoulue;
        }

        // Normalise les valeurs de mouvements
        Vector3 inputDirection = new Vector3(lesInputs.bouge.x, 0.0f, lesInputs.bouge.y).normalized;

        // Si les valeurs de déplacements du perso ne sont pas égales à 0
        if (lesInputs.bouge != Vector2.zero)
        {
            // Déplace le perso
            inputDirection = transform.right * lesInputs.bouge.x + transform.forward * lesInputs.bouge.y;
        }

        // Permet de bouger le perso selon son Character controller
        controller.Move(inputDirection.normalized * (vitesse * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
    }


    // --------------------------------------------------------------------ANIMS------------------------------------------------------------------------

    // Début de l'animation de saut du perso
    void DebutAnim()
    {
        // Si on appuie sur "espace" pour faire sauter le perso ET que celui-ci touche au sol (valeur à vrai) ET que le booléen "debutAnim" est à faux
        if (lesInputs.saute && toucheSol && debutAnim == false)
        {
            // Rend le booléen "debutAnim" à vrai
            debutAnim = true;

            // Enclenche le trigger "Saut" de l'Animator du perso
            animator.SetTrigger("Saut");
        }
    }


    // Fin de l'animation de saut du perso
    public void FinAnim(float valeur)
    {
        // La valeur de la hauteur du saut du perso est la valeur (valeur) donnée dans le script "GestionAnims"
        hauteurSaut = valeur;

        // Rend le booléen "debutAnim" à faux (pour que la méthode "DebutAnim" puisse être exécutée à nouveau)
        debutAnim = false;
    }


    // ------------------------------------------------------------------------ATTAQUE----------------------------------------------------------------

    // Attaque avec l'arme du perso
    void Attaque()
    {
        // Si le perso tire
        if (lesInputs.tire)
        {
            // Rend le booléen "equipe" à vrai
            equipe = true;

            // Active la caméra 1re personne
            cinemachineFP.SetActive(true);

            // Désactive la caméra 3e personne
            cameraTP.SetActive(false);

            // Active la mire
            mire.SetActive(true);

            // Rend le booléen "Arme" de l'Animator du perso à vrai
            animator.SetBool("Arme", true);

            // Rend l'arme visible
            arme.SetActive(true);

            // Cache le curseur de la souris tout en le mettant au centre de l'écran
            Cursor.lockState = CursorLockMode.Locked;

            // Active le son de l'arme qui tire.
            tire.GetComponent<AudioSource>().enabled = true;

            // Invoque les méthodes "FiniTir" et "Recharge" dans 1 seconde
            Invoke(nameof(FiniTir), 1f);
            Invoke(nameof(Recharge), 1f);

            // Rend le booléen "tire" à faux pour éviter de tirer continuellement
            lesInputs.tire = false;

            // Position de la mire
            Vector3 positionDepart = mire.transform.position;

            // Direction du tir vers l'avant de la mire
            Vector3 directionTir = mire.transform.forward;

            // Délai pour retourner vers la caméra 3e personne
            cooldown = 5f;

            // Récupère tous les colliders touchés par la trajectoire du tir
            RaycastHit[] impacts = Physics.RaycastAll(
                positionDepart,
                directionTir,
                20f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide
            );

            // Trie les impacts du plus proche au plus éloigné
            System.Array.Sort(
                impacts,
                (a, b) => a.distance.CompareTo(b.distance)
            );

            // Parcourt les impacts dans l'ordre de distance
            foreach (RaycastHit impact in impacts)
            {
                // Récupère le GameObject touché
                GameObject objetCollisions = impact.collider.gameObject;

                // Ignore la zone de contact de l'ennemi
                if (objetCollisions.CompareTag("Ennemi"))
                {
                    continue;
                }

                // Si le tir touche le collider vulnérable d'un ennemi
                if (objetCollisions.CompareTag("CollisionsEnnemis"))
                {
                    Debug.Log("Touché");

                    // Détruit l'objet touché (si ce collider est un enfant, seul cet enfant sera détruit)
                    Destroy(objetCollisions);

                    // Augmente de 1 le nombre d'ennemis éliminés
                    infosPerso.nbEnnemis++;

                    // Affiche le nombre d'ennemis éliminés dans la console
                    Debug.Log(infosPerso.nbEnnemis + " ennemis");

                    // Arrête le parcours après avoir touché un ennemi
                    break;
                }

                // Si un autre objet est touché, le tir est bloqué
                break;
            }
        }
    }


    // Recharger l'arme du perso
    void Recharge()
    {
        // Si l'Audio source de tirer est à faux
        if (tire.GetComponent<AudioSource>().enabled == false)
        {
            // Active l'Audio source de recharger
            recharge.GetComponent<AudioSource>().enabled = true;

            // Invoque la méthode "DesactiveRecharge" dans 1 seconde
            Invoke("DesactiveRecharge", 1);
        }
    }


    // Désactive l'Audio source de recharger
    void DesactiveRecharge()
    {
        recharge.GetComponent<AudioSource>().enabled = false;
    }


    // Désactive l'Audio source de tirer
    void FiniTir()
    {
        tire.GetComponent<AudioSource>().enabled = false;
    }


    // Désactive l'Audio source des pièces et de la douleur du perso
    void DesactiveSons()
    {
        piecesJaunes.GetComponent<AudioSource>().enabled = false;
        piecesRouges.GetComponent<AudioSource>().enabled = false;
        ouch.GetComponent<AudioSource>().enabled = false;
    }


    // ------------------------------------------------------------------------DÉSACTIVATION----------------------------------------------------------------

    // Enlève les panneaux actifs
    void EnleverPanneaux()
    {
        panneauDebut.SetActive(false);
        panneauInfos.SetActive(false);
        panneauMaire.SetActive(false);
        panneauLimites.SetActive(false);
    }


    // ------------------------------------------------------------------------COLLISIONS-------------------------------------------------------------------

    // Quand le perso entre en collision avec qq
    void OnTriggerEnter(Collider collision)
    {
        // Pièces jaunes
        if (collision.CompareTag("PiecesJaunes"))
        {
            // Active l'Audio source des pièces
            piecesJaunes.GetComponent<AudioSource>().enabled = true;

            // Détruit la pièce
            Destroy(collision.gameObject);

            // Augmente le nb de pièces du perso de 1
            infosPerso.nbPieces++;

            // Permet de savoir le nb de pièces par le perso dans la console
            Debug.Log(infosPerso.nbPieces + " pièces");

            // Invoque la méthode "DesactiveSons" dans 1 seconde
            Invoke("DesactiveSons", 1);
        }


        // Pièces rouges
        if (collision.CompareTag("PiecesRouges"))
        {
            // Active l'Audio source des pièces
            piecesRouges.GetComponent<AudioSource>().enabled = true;

            // Détruit la pièce
            Destroy(collision.gameObject);

            // Augmente le nb de pièces du perso de 1
            infosPerso.nbPieces++;

            // Augmente le nb de PV du perso de 1
            infosPerso.nbPV++;

            // Permet de savoir le nb de pièces par le perso dans la console
            Debug.Log(infosPerso.nbPieces + " pièces");

            // Invoque la méthode "DesactiveSons" dans 1 seconde
            Invoke("DesactiveSons", 1);
        }


        // Panneau au centre du village
        if (collision.CompareTag("panneauInfos"))
        {
            // Affiche le panneau
            panneauInfos.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }


        // Panneau de la maison du maire
        if (collision.CompareTag("panneauMaire"))
        {
            // Affiche le panneau
            panneauMaire.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }

        // Murs limites du terrain
        if (collision.CompareTag("mursLimites"))
        {
            // Affiche le panneau
            panneauLimites.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }


        // Collision avec les ennemis
        if (collision.CompareTag("Ennemi"))
        {
            // Ignore les dégâts pendant l'invincibilité
            if (Time.time < prochainDegatPossible)
                return;

            // Définit le moment où le personnage pourra subir de nouveaux dégâts
            prochainDegatPossible = Time.time + dureeInvincibilite;

            // Diminue les PV sans passer sous zéro
            infosPerso.nbPV = Mathf.Max(0, infosPerso.nbPV - 1);

            // Active le son de douleur
            ouch.GetComponent<AudioSource>().enabled = true;

            // Affiche les PV mis à jour
            PointsVie();

            // Désactive les sons temporaires après 1 seconde
            Invoke(nameof(DesactiveSons), 1f);

            // Permet de savoir le nb de PV du perso 
            Debug.Log("PV restants : " + infosPerso.nbPV);
        }

        // Si ce qui est touché est un ennemi
        if (collision.CompareTag("CollisionsEnnemis"))
        {

        }

        // Collision avec le diamant
        if (collision.CompareTag("Diamant"))
        {
            AudioSource sonDiamant =
                collision.GetComponent<AudioSource>();

            // Joue le son sans dépendre de l'objet qui va être détruit
            if (sonDiamant != null && sonDiamant.clip != null)
            {
                AudioSource.PlayClipAtPoint(
                    sonDiamant.clip,
                    collision.transform.position
                );
            }

            // Fait disparaître le diamant
            Destroy(collision.gameObject);

            // Lance la victoire après 1 seconde
            Invoke(nameof(SceneVictoire), 1f);
        }
    }

    // Gestion des PV du perso
    void PointsVie()
    {
        // Quand le perso a 4 PV
        if (infosPerso.nbPV == 4)
        {
            PV4.SetActive(true);
            PV4.GetComponent<Animator>().enabled = true;

            PV3.SetActive(true);
            PV3.GetComponent<Animator>().enabled = false;

            PV2.SetActive(true);
            PV2.GetComponent<Animator>().enabled = false;

            PV1.SetActive(true);
            PV1.GetComponent<Animator>().enabled = false;
        }

        // Quand le perso a 3 PV
        if (infosPerso.nbPV == 3)
        {
            PV4.SetActive(false);
            PV4.GetComponent<Animator>().enabled = false;

            PV3.SetActive(true);
            PV3.GetComponent<Animator>().enabled = true;

            PV2.SetActive(true);
            PV2.GetComponent<Animator>().enabled = false;

            PV1.SetActive(true);
            PV1.GetComponent<Animator>().enabled = false;
        }

        // Quand le perso a 2 PV
        if (infosPerso.nbPV == 2)
        {
            PV4.SetActive(false);
            PV4.GetComponent<Animator>().enabled = false;

            PV3.SetActive(false);
            PV3.GetComponent<Animator>().enabled = false;

            PV2.SetActive(true);
            PV2.GetComponent<Animator>().enabled = true;

            PV1.SetActive(true);
            PV1.GetComponent<Animator>().enabled = false;
        }

        // Quand le perso a 1 PV
        if (infosPerso.nbPV == 1)
        {
            PV4.SetActive(false);
            PV4.GetComponent<Animator>().enabled = false;

            PV3.SetActive(false);
            PV3.GetComponent<Animator>().enabled = false;

            PV2.SetActive(false);
            PV2.GetComponent<Animator>().enabled = false;

            PV1.SetActive(true);
            PV1.GetComponent<Animator>().enabled = true;
        }

        // Quand le perso a 0 PV
        if (infosPerso.nbPV == 0)
        {
            PV4.SetActive(false);
            PV4.GetComponent<Animator>().enabled = false;

            PV3.SetActive(false);
            PV3.GetComponent<Animator>().enabled = false;

            PV2.SetActive(false);
            PV2.GetComponent<Animator>().enabled = false;

            PV1.SetActive(false);
            PV1.GetComponent<Animator>().enabled = false;

            // Pour empêcher d'avoir moins que 0 PV
            infosPerso.nbPV = 0;

            // Invoque la méthode "SceneDefaite" dans 1 seconde
            Invoke("SceneDefaite", 1);
        }

        // Pour empêcher d'avoir plus que 4 PV
        if (infosPerso.nbPV > 4)
        {
            infosPerso.nbPV = 4;
        }
    }


    // ------------------------------------------------------------------------VICTOIRE + DÉFAITE--------------------------------------------------------

    // Quand le perso a ramassé toutes les pièces
    void DiamantApparu()
    {
        if (infosPerso.nbPieces == 10)
        {
            // Active le diamant
            diamant.SetActive(true);
        }
    }


    // Aller à la scène "Vitoire"
    void SceneVictoire()
    {
        // Dit à la caméra principale d'arrêter sa musique
        GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

        // Charge la scène "Base"
        SceneManager.LoadScene("Victory");
    }


    // Aller à la scène "Défaite"
    void SceneDefaite()
    {
        // Dit à la caméra principale d'arrêter sa musique
        GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

        // Charge la scène "Base"
        SceneManager.LoadScene("Defeat");
    }


    // ------------------------------------------------------------------------SAUT-------------------------------------------------------------------

    // Détermine si le perso touche au sol ou non (création d'une sphère sous le perso)
    void VerifierSol()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - distanceDuSol, transform.position.z);
        toucheSol = Physics.CheckSphere(spherePosition, GroundedRadius, layerSol, QueryTriggerInteraction.Ignore);
    }


    // Permet au perso de sauter
    void SautEtGravite()
    {
        // Applique la gravité au fil du temps si la vitesse verticale est sous la vitesse finale
        if (vitesseVerticale < vitesseFinale)
        {
            vitesseVerticale += Gravite * Time.deltaTime;
        }

        // Si le perso touche au sol
        if (toucheSol)
        {
            // Réinitialise le délai de chute
            chuteTimeout = delaiChute;

            // Empêche la vélocité de baisser à l'infini quand le perso touche au sol
            if (vitesseVerticale < 0.0f)
            {
                vitesseVerticale = -2f;
            }

            // Si on appuie sur la barre d'espace ET que le délai pour sauter à nouveau est de 0 : Saute
            if (lesInputs.saute && sautTimeout <= 0.0f)
            {
                vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * Gravite);
            }

            // Enclenche le délai pour sauter à nouveau
            if (sautTimeout >= 0.0f)
            {
                sautTimeout -= Time.deltaTime;
            }
        }

        // Sinon
        else
        {
            // Réinitialise le délai pour sauter à nouveau
            sautTimeout = delaiSaut;

            // Réinitialise le délai de chute
            if (chuteTimeout >= 0.0f)
            {
                chuteTimeout -= Time.deltaTime;
            }

            // Si le perso ne touche pas au sol : ne Saute PAS
            lesInputs.saute = false;
        }
    }
}