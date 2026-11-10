using TMPro;
using UnityEngine;
using UnityEngine.AI;
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

    [Tooltip("Vitesse de décélération du perso")]
    [SerializeField] private float deceleration = 30.0f;


    [Space(5)]
    [Header("Saut")]
    [Tooltip("Hauteur du saut du perso")]
    [SerializeField] private float hauteurSaut = 1.2f;

    [Tooltip("Valeur de gravité (défaut = -9.81f)")]
    [SerializeField] private float Gravite = -15.0f;

    [Tooltip("Temps nécessaire pour sauter à nouveau")]
    [SerializeField] private float delaiSaut = 0.1f;

    // Délai nécessaire pour sauter à nouveau
    private float sautTimeout;

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

    [Tooltip("Largeur de la zone de détection aux bords de l'écran")]
    [SerializeField] private float zoneBordEcran = 20f;

    [Tooltip("Vitesse de rotation de la caméra lorsque la souris atteint un bord")]
    [SerializeField] private float vitesseRotationBord = 100f;

    // Cinémachines pour la 1re et 3e personne (cameraTP) ainsi que la caméra principale (cinemachineTP)
    [SerializeField] private GameObject cinemachineFP;
    [SerializeField] private GameObject cinemachineTP;
    [SerializeField] private GameObject cameraTP;

    // Valeurs permettant d'ajuster la vélocité et le temps de rotation de la caméra 3e personne
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
    [SerializeField] private GameObject panneauDebut;
    [SerializeField] private GameObject panneauInfos;
    [SerializeField] private GameObject panneauMaire;
    [SerializeField] private GameObject panneauLimites;

    [Space(5)]
    [Header("Coeurs")]
    [SerializeField] private GameObject PV4;
    [SerializeField] private GameObject PV3;
    [SerializeField] private GameObject PV2;
    [SerializeField] private GameObject PV1;


    [Space(5)]
    [Header("Vitesse")]
    // Vitesse verticale
    private float vitesseVerticale;

    // Valeur du changement de vitesse du perso
    private float changementVitesse;

    // Vélocité horizontale actuelle du perso
    private Vector3 vitesseHorizontaleActuelle;


    [Space(5)]
    [Header("Sons")]
    // GameObjects avec les effets sonores des pièces
    [SerializeField] private GameObject piecesJaunes;
    [SerializeField] private GameObject piecesRouges;

    // GameObjects avec les effets sonores du perso
    [SerializeField] private GameObject ouch;
    [SerializeField] private GameObject tire;
    [SerializeField] private GameObject recharge;

    // GameObject avec la musique stressante
    [SerializeField] private GameObject stress;

    // Indique si la musique stressante a déjà été activée
    private bool musiqueStressActivee = false;

    // AudioSource du tir, de la recharge, de la caméra principale et de la douleur
    private AudioSource audioTir;
    private AudioSource audioRecharge;
    private AudioSource audioPrincipal;
    private AudioSource audioOuch;

    // Indique si le perso est en train de recharger
    private bool rechargementEnCours = false;


    [Space(5)]
    // Durée d'invincibilité
    [SerializeField] private float dureeInvincibilite = 1.5f;

    // Temps où le perso pourra reprendre des dégâts
    private float prochainDegatPossible = 0f;

    // Indique si la défaite a déjà été programmée
    private bool defaiteEnAttente = false;

    // Indique si la partie est terminée
    private bool partieTerminee = false;



    [Space(5)]
    [Header("Autres")]
    // Character controller du perso
    private CharacterController controller;

    // Script "StarterAssetsInputs" du perso
    private StarterAssetsInputs lesInputs;

    // SO "InfosPerso" du perso
    [SerializeField] private InfosPerso infosPerso;


    // Diamant
    [SerializeField] private GameObject diamant;

    // Indique si le diamant est déjà apparu
    private bool diamantApparu = false;

    // Indique si la victoire est en attente
    private bool victoireEnAttente = false;


    // Zone de texte avec le chrono
    [SerializeField] private TMP_Text temps;

    // Temps actuel du chrono
    float currentTime = 0f;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------

    // Est appelée au lancement de la scène
    void Start()
    {
        // Réinitialise les valeurs du nb de pièces, des ennemis tués, de PV et le chrono au lancement de la scène
        infosPerso.nbPieces = 0;
        infosPerso.nbEnnemis = 0;

        // Met à jour l'affichage initial des PV
        infosPerso.nbPV = 4;
        PointsVie();

        // Met à jour le temps
        currentTime = 120f;

        // Trouve les composantes du Character controller et le script "StarterAssetsInputs" du perso
        controller = GetComponent<CharacterController>();
        lesInputs = GetComponent<StarterAssetsInputs>();

        // Récupère les AudioSources du tir, de la recharge et de la douleur
        audioTir = tire.GetComponent<AudioSource>();
        audioRecharge = recharge.GetComponent<AudioSource>();
        audioOuch = ouch.GetComponent<AudioSource>();

        // Réinitialise les délais de saut
        sautTimeout = delaiSaut;

        // Récupère l'AudioSource de la caméra principale
        GameObject cameraPrincipale = GameObject.Find("Main Camera");
        audioPrincipal = cameraPrincipale.GetComponent<AudioSource>();

        // Désactive la musique stressante au début de la partie
        stress.SetActive(false);

        // Affiche le panneau du début
        panneauDebut.SetActive(true);

        // Invoque la méthode "EnleverPanneaux" dans 3 secondes
        Invoke(nameof(EnleverPanneaux), 3f);
    }


    // Est appelée à chaque frame
    void Update()
    {
        // Arrête l'exécution si la partie est terminée
        if (partieTerminee)
            return;

        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
            Bouge();

        // Sinon (quand on est 3e personne)
        else
            DirectionTP();

        // Lance le chrono
        currentTime -= Time.deltaTime;

        // Empêche le chrono de descendre sous 0
        currentTime = Mathf.Max(0f, currentTime);

        // Met à jour la valeur du temps du perso
        temps.text = "Temps : " + currentTime.ToString("0");
        infosPerso.temps = (int)currentTime;

        // Si le temps est inférieur ou égal à 30 secondes
        if (currentTime <= 30f && currentTime > 0f && !musiqueStressActivee)
        {
            // Arrête la musique principale
            audioPrincipal.enabled = false;

            // Active la musique stressante
            stress.SetActive(true);
            musiqueStressActivee = true;
        }

        // Si le temps est écoulé
        if (currentTime <= 0f)
        {
            // Exécute la méthode de défaite
            SceneDefaite();
            return;
        }

        // Exécute les méthodes suivantes à chaque frame
        Attaque();
        RetourCameraTP();
        VerifierSol();
        SautEtGravite();
        DebutAnim();
        DiamantApparu();
    }


    // Est appelée après que les fcts dans Update soient exécutées
    void LateUpdate()
    {
        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
            RotationCamera();
    }


    // -----------------------------------------------------------------------CINEMACHINES + BOUGE------------------------------------------------------------------

    // Accélération, décélération et déplacement du perso
    void Deplacer(Vector3 direction, float vitesseVoulue)
    {
        // Détermine la vitesse horizontale souhaitée
        Vector3 vitesseVoulueHorizontale = direction * vitesseVoulue;

        // Détermine si le perso change de direction
        bool changeDirection = vitesseVoulueHorizontale.sqrMagnitude > 0.01f && vitesseHorizontaleActuelle.sqrMagnitude > 0.01f &&
            Vector3.Dot(vitesseHorizontaleActuelle.normalized, vitesseVoulueHorizontale.normalized) < 0.95f;

        // Détermine la vitesse de changement
        float vitesseChangement;

        if (vitesseVoulueHorizontale == Vector3.zero || changeDirection)
            vitesseChangement = deceleration;

        else
            vitesseChangement = accelerationDeceleration;

        // Accélère, ralentit ou change la direction du perso
        vitesseHorizontaleActuelle = Vector3.MoveTowards(vitesseHorizontaleActuelle, vitesseVoulueHorizontale, vitesseChangement * Time.deltaTime);

        // Calcule la vitesse actuelle du perso
        float vitesse = vitesseHorizontaleActuelle.magnitude;

        // Rend plus fluide le changement de vitesse de l'animation
        changementVitesse = Mathf.Lerp(changementVitesse, vitesse, Time.deltaTime * accelerationDeceleration);

        // Si le changement de vitesse est plus petit que 0.01, met sa valeur à 0
        if (changementVitesse < 0.01f)
            changementVitesse = 0;

        // Donne la valeur du changement de vitesse à l'Animator
        animator.SetFloat("Vitesse", changementVitesse);

        // Déplace le perso avec son Character Controller
        controller.Move(vitesseHorizontaleActuelle * Time.deltaTime + Vector3.up * (vitesseVerticale * Time.deltaTime));
    }


    // Permet à la caméra 3e personne de bouger
    void DirectionTP()
    {
        // Valeurs des mouvements horizontaux et verticaux de la caméra
        float gaucheDroite = lesInputs.bouge.x;
        float avantArriere = lesInputs.bouge.y;

        // Détermine si le perso marche ou cours
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero)
            vitesseVoulue = 0.0f;

        // Normalise les valeurs de mouvements
        Vector3 direction = new Vector3(gaucheDroite, 0.0f, avantArriere).normalized;

        // Si le perso se déplace
        if (direction != Vector3.zero)
        {
            // Angle voulu pour faire rotater le perso
            float angleVoulu = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cinemachineTP.transform.eulerAngles.y;

            // Angle de rotation du perso
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVoulu, ref ajusteVelocityRotation, ajusteTempsRotation);

            // Rotate le perso
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Direction du mouvement selon l'angle de rotation
            direction = (Quaternion.Euler(0f, angleVoulu, 0f) * Vector3.forward).normalized;
        }

        // Valeurs de déplacement
        Deplacer(direction, vitesseVoulue);
    }


    // Rotation de la caméra quand le perso est en mode 1re personne
    void RotationCamera()
    {
        // Détecte si il y a un mouvement de la caméra
        if (lesInputs.regarder.sqrMagnitude >= seuil)
        {
            // Rotation verticale de la caméra
            niveauCinemachine += lesInputs.regarder.y * vitesseRotation;

            // Rotation horizontale du perso
            float rotationHorizontale = lesInputs.regarder.x * vitesseRotation;

            // Met à jour les valeurs des angles possibles que la Cinemachine peut faire
            niveauCinemachine = GrandeurAngle(niveauCinemachine, maxMouvCamBas, maxMouvCamHaut);

            // Met à jour les valeurs de la Cinemachine
            cibleCinemachine.transform.localRotation = Quaternion.Euler(niveauCinemachine, 0.0f, 0.0f);

            // Permet au perso d'effectuer une rotation à gauche et à droite
            transform.Rotate(Vector3.up * rotationHorizontale);
        }

        // Rotation supplémentaire lorsque la souris atteint les bords de l'écran
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            // Bord gauche
            if (Input.mousePosition.x <= zoneBordEcran)
                transform.Rotate(Vector3.up * (-vitesseRotationBord * Time.deltaTime));

            // Bord droit
            else if (Input.mousePosition.x >= Screen.width - zoneBordEcran)
                transform.Rotate(Vector3.up * (vitesseRotationBord * Time.deltaTime));
        }
    }


    // Détermine les angles possibles que la Cinemachine peut faire
    static float GrandeurAngle(float angle, float angleMin, float angleMax)
    {
        return Mathf.Clamp(angle, angleMin, angleMax);
    }


    // Permet au perso de bouger quand il est en mode 1re personne
    void Bouge()
    {
        // Détermine si le perso marche ou cours
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero)
            vitesseVoulue = 0.0f;

        // Direction du déplacement selon l'orientation du perso
        Vector3 direction = transform.right * lesInputs.bouge.x + transform.forward * lesInputs.bouge.y;

        // Normalise la direction pour conserver une vitesse constante en diagonale
        direction = direction.normalized;

        // Valeurs de déplacement
        Deplacer(direction, vitesseVoulue);
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
        // Si le perso ne tire pas, ne rien faire
        if (!lesInputs.tire)
            return;

        // Réinitialise l'input pour éviter un tir automatique après la recharge
        lesInputs.tire = false;

        // Empêche de tirer pendant la recharge
        if (rechargementEnCours || partieTerminee)
            return;

        // Commence la séquence de tir et de recharge
        rechargementEnCours = true;

        // Active le mode 1re personne
        equipe = true;

        // Active la caméra 1re personne
        cinemachineFP.SetActive(true);

        // Désactive la caméra 3e personne
        cameraTP.SetActive(false);

        // Active la mire
        mire.SetActive(true);

        // Modifie l'animation du perso
        animator.SetBool("Arme", true);

        // Rend l'arme visible
        arme.SetActive(true);

        // Verrouille le curseur
        Cursor.lockState = CursorLockMode.Locked;

        // Active l'AudioSource du tir
        audioTir.enabled = true;

        // Programme la fin du tir
        Invoke(nameof(FiniTir), audioTir.clip.length);

        // Délai pour retourner à la caméra 3e personne
        cooldown = 5f;

        // Position et direction de la mire
        Vector3 positionDepart = mire.transform.position;
        Vector3 directionTir = mire.transform.forward;

        // Récupère tous les colliders touchés par le tir
        RaycastHit[] impacts = Physics.RaycastAll(positionDepart, directionTir, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        // Trie les impacts du plus proche au plus éloigné
        System.Array.Sort(impacts, (a, b) => a.distance.CompareTo(b.distance));

        // Parcourt les impacts dans l'ordre de distance
        foreach (RaycastHit impact in impacts)
        {
            // Collisions
            GameObject objetCollisions = impact.collider.gameObject;

            // Ignore la zone de contact de l'ennemi
            if (objetCollisions.CompareTag("Ennemi"))
                continue;

            // Si le tir touche le collider vulnérable d'un ennemi
            if (objetCollisions.CompareTag("CollisionsEnnemis"))
            {
                // Détruit l'ennemi
                Destroy(objetCollisions);

                // Augmente de 1 le nb d'ennemis tués du perso
                infosPerso.nbEnnemis++;

                // Permet de savoir le nb d'ennemis tués par le perso dans la console
                Debug.Log(infosPerso.nbEnnemis + " ennemis");

                // Arrête la boucle
                break;
            }

            // Si un autre objet est touché, le tir est bloqué
            break;
        }
    }

    // Gestion du retour à la caméra 3e personne
    void RetourCameraTP()
    {
        // Si le perso est en 1re personne
        if (equipe)
        {
            // Diminue le délai de retour
            cooldown -= Time.deltaTime;

            // Si le délai est terminé
            if (cooldown <= 0f)
            {
                // Désactive le mode 1re personne
                equipe = false;

                // Désactive la caméra 1re personne
                cinemachineFP.SetActive(false);

                // Réactive la caméra 3e personne
                cameraTP.SetActive(true);

                // Désactive la mire
                mire.SetActive(false);

                // Déséquipe l'arme
                arme.SetActive(false);

                // Modifie l'animation du perso
                animator.SetBool("Arme", false);

                // Déverrouille le curseur
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }


    // Termine le son du tir
    void FiniTir()
    {
        // Arrête le son du tir
        audioTir.enabled = false;

        // Commence la recharge
        Recharge();
    }


    // Recharger l'arme du perso
    void Recharge()
    {
        // Active l'AudioSource de recharge
        audioRecharge.enabled = true;

        // Attend la durée complète du son
        Invoke(nameof(DesactiveRecharge), audioRecharge.clip.length);
    }


    // Termine la recharge
    void DesactiveRecharge()
    {
        // Arrête le son de recharge
        audioRecharge.enabled = false;

        // Autorise un nouveau tir
        rechargementEnCours = false;
    }


    // Désactive l'Audio source des pièces et de la douleur du perso
    void DesactiveSons()
    {
        piecesJaunes.GetComponent<AudioSource>().enabled = false;
        piecesRouges.GetComponent<AudioSource>().enabled = false;
        audioOuch.enabled = false;
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
            Invoke(nameof(DesactiveSons), 1f);
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

            // Met à jour l'affichage des PV
            PointsVie();

            // Permet de savoir le nb de pièces par le perso dans la console
            Debug.Log(infosPerso.nbPieces + " pièces");

            // Invoque la méthode "DesactiveSons" dans 1 seconde
            Invoke(nameof(DesactiveSons), 1f);
        }


        // Panneau au centre du village
        if (collision.CompareTag("panneauInfos"))
        {
            // Affiche le panneau
            panneauInfos.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 2f);
        }


        // Panneau de la maison du maire
        if (collision.CompareTag("panneauMaire"))
        {
            // Affiche le panneau
            panneauMaire.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 2f);
        }


        // Murs limites du terrain
        if (collision.CompareTag("mursLimites"))
        {
            // Affiche le panneau
            panneauLimites.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 2f);
        }


        // Collision avec les ennemis
        if (collision.CompareTag("Ennemi"))
        {
            // Ignore les dégâts si la victoire est en attente
            if (victoireEnAttente)
                return;

            // Ignore les dégâts pendant l'invincibilité
            if (Time.time < prochainDegatPossible)
                return;

            // Définit le moment où le perso pourra subir de nouveaux dégâts
            prochainDegatPossible = Time.time + dureeInvincibilite;

            // Diminue les PV sans passer sous 0
            infosPerso.nbPV = Mathf.Max(0, infosPerso.nbPV - 1);

            // Active le son de douleur
            audioOuch.enabled = true;

            // Affiche les PV mis à jour
            PointsVie();

            // Désactive les sons temporaires après 1 seconde
            Invoke(nameof(DesactiveSons), 1f);
        }


        // Collision avec le diamant
        if (collision.CompareTag("Diamant"))
        {
            // Empêche de déclencher plusieurs fois la victoire
            if (victoireEnAttente)
                return;

            // Met la victoire en attente
            victoireEnAttente = true;

            // Annule une éventuelle défaite programmée
            CancelInvoke(nameof(SceneDefaite));

            // Arrête les ennemis
            ArreterEnnemis();

            // Récupère l'AudioSource du diamant
            AudioSource sonDiamant = collision.GetComponent<AudioSource>();

            // Joue le son sans dépendre de l'objet qui va être détruit
            if (sonDiamant != null && sonDiamant.clip != null)
                AudioSource.PlayClipAtPoint(sonDiamant.clip, collision.transform.position);

            // Fait disparaître le diamant
            Destroy(collision.gameObject);

            // Lance la victoire après 1 seconde
            Invoke(nameof(SceneVictoire), 1f);
        }
    }

    // Gestion des PV du perso
    void PointsVie()
    {
        // Empêche d'avoir moins de 0 PV ou plus de 4 PV
        infosPerso.nbPV = Mathf.Clamp(infosPerso.nbPV, 0, 4);

        // Tableau contenant les 4 coeurs
        GameObject[] coeurs = { PV1, PV2, PV3, PV4 };

        // Parcourt les 4 coeurs
        for (int i = 0; i < coeurs.Length; i++)
        {
            // Détermine si le coeur doit être visible
            bool actif = i < infosPerso.nbPV;
            coeurs[i].SetActive(actif);

            // Récupère l'Animator du coeur
            Animator animatorCoeur = coeurs[i].GetComponent<Animator>();

            // Anime uniquement le coeur correspondant aux PV actuels
            animatorCoeur.enabled = (i == infosPerso.nbPV - 1);
        }

        // Si le perso n'a plus de PV, charge la scène de défaite
        if (infosPerso.nbPV == 0 && !defaiteEnAttente)
        {
            defaiteEnAttente = true;
            Invoke(nameof(SceneDefaite), 1f);
        }
    }


    // ------------------------------------------------------------------------VICTOIRE + DÉFAITE--------------------------------------------------------

    // Quand le perso a ramassé toutes les pièces et tué tous les ennemis
    void DiamantApparu()
    {
        if (infosPerso.nbPieces == 10 && infosPerso.nbEnnemis == 10 && !diamantApparu && diamant != null)
        {
            // Active le diamant
            diamant.SetActive(true);

            // Empêche de tenter de l'activer à nouveau
            diamantApparu = true;
        }
    }


    // Termine la partie et charge la scène correspondante
    void TerminerPartie(string nomScene)
    {
        // Empêche de terminer la partie plusieurs fois
        if (partieTerminee)
            return;

        partieTerminee = true;

        // Arrête les musiques
        audioPrincipal.enabled = false;
        stress.SetActive(false);

        // Charge la scène correspondante
        SceneManager.LoadScene(nomScene);
    }


    // Arrête les déplacements de tous les ennemis
    void ArreterEnnemis()
    {
        NavMeshAgent[] ennemis = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);

        foreach (NavMeshAgent ennemi in ennemis)
        {
            ennemi.isStopped = true;
            ennemi.ResetPath();
        }
    }


    // Aller à la scène de victoire
    void SceneVictoire()
    {
        TerminerPartie("Victory");
    }


    // Aller à la scène de défaite
    void SceneDefaite()
    {
        // Si la victoire est en attente, ignore cette méthode
        if (victoireEnAttente)
            return;

        TerminerPartie("Defeat");
    }


    // ------------------------------------------------------------------------SAUT-------------------------------------------------------------------

    // Détermine si le perso touche au sol ou non (création d'une sphère sous le perso)
    void VerifierSol()
    {
        // Position de la sphère sous le perso
        Vector3 spherePosition = transform.position + Vector3.up * distanceDuSol;

        // Vérifie si la sphère touche le sol
        toucheSol = Physics.CheckSphere(spherePosition, GroundedRadius, layerSol, QueryTriggerInteraction.Ignore);
    }


    // Permet au perso de sauter
    void SautEtGravite()
    {
        // Vitesse finale
        float vitesseFinale = -53.0f;

        // Applique la gravité sans dépasser la vitesse maximale de chute
        if (vitesseVerticale > vitesseFinale)
            vitesseVerticale = Mathf.Max(vitesseVerticale + Gravite * Time.deltaTime, vitesseFinale);

        // Si le perso touche au sol
        if (toucheSol)
        {
            // Empêche la vélocité de baisser à l'infini quand le perso touche au sol
            if (vitesseVerticale < 0.0f)
                vitesseVerticale = -2f;

            // Si on appuie sur la barre d'espace ET que le délai pour sauter à nouveau est de 0 : Saute
            if (lesInputs.saute && sautTimeout <= 0.0f)
                vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * Gravite);

            // Enclenche le délai pour sauter à nouveau
            if (sautTimeout >= 0.0f)
                sautTimeout -= Time.deltaTime;
        }

        // Sinon
        else
        {
            // Réinitialise le délai pour sauter à nouveau
            sautTimeout = delaiSaut;

            // Si le perso ne touche pas au sol : ne Saute PAS
            lesInputs.saute = false;
        }
    }
}