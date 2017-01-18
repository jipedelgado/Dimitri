using UnityEngine;
using System.Collections.Generic;
using Hex = HexGridLib.Hex;

/*
Associé à gameObject plane
23 septembre 2016 = repris à partir de la version 29 mai 2016
 2 janvier 2017 = ajouté mapHexGameobject et procédures associées
 5 janvier remplacé myHexGrid par GameController
 18 janvier 2017 -> gitHub
 */

public class GameController : MonoBehaviour {
    public GameObject friend;
    public GameObject enemy;
    public GameObject tile;
    public GameObject obstacle;
    public int portée_de_tir;
    public int Rayon;
    public int pourcentagedeRemplissage;
    public GUIText chapitre;
    public GUIText paragraphe;
    public GUIText help;

    // pour accéder à obstacles depuis PlayerController
    private List<Hex> obstacles;
    public List<Hex> Obstacles {
        get { return obstacles; }
    }

    private Vector3 vecteurPosition;
    private List<Hex> plusCourtChemin;
    private List<Hex> Visibles;
    private GameObject currentPlayer;

    // pour accéder au champ canMove de PlayerController.cs
    private PlayerController friendPlayerController;
    private PlayerController enemyPlayerController;

    bool NONE, SETUP, PLAYER1, PLAYER2;
    bool ADDOBSTACLE, DELETEOBSTACLE, PLAY;

    private Dictionary<Hex, GameObject> mapHexGameobject;

    private void Awake() {
        friendPlayerController = friend.GetComponent<PlayerController>();
        enemyPlayerController = enemy.GetComponent<PlayerController>();
    }

    void Start() {
        mapHexGameobject = new Dictionary<Hex, GameObject>();
        obstacles = new List<Hex>();
        plusCourtChemin = new List<Hex>();
        Visibles = new List<Hex>();
        MapGenerator pg = new MapGenerator(Rayon, pourcentagedeRemplissage);
        obstacles = pg.listeObstacles;
        Hex.display(obstacles, obstacle, mapHexGameobject);
        Hex.setColor("Obstacle", Color.yellow);
        chapitre.text = "";
        paragraphe.text = "";

        NONE = true;
        SETUP = false;
        PLAYER1 = false;
        PLAYER2 = false;
        ADDOBSTACLE = false;
        DELETEOBSTACLE = false;
        PLAY = false;
    }

    void Update() {

        // définit les étapes du jeu, NONE, SETUP, PLAYER1, PLAYER2
        {
            if (Input.GetKeyDown(KeyCode.S)) {
                SETUP = true;
                PLAYER1 = false;
                PLAYER2 = false;
                chapitre.text = Messages.messageSETUP1;
                paragraphe.text = Messages.messageSETUP2;
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                SETUP = false;
                PLAYER1 = true;
                PLAYER2 = false;
                currentPlayer = friend;
                chapitre.text = Messages.messagePLAYER1;
                paragraphe.text = "";
                friendPlayerController.AllowMove(true);
                enemyPlayerController.AllowMove(false);
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                SETUP = false;
                PLAYER1 = false;
                PLAYER2 = true;
                currentPlayer = enemy;
                chapitre.text = Messages.messagePLAYER2;
                paragraphe.text = "";
                friendPlayerController.AllowMove(false);
                enemyPlayerController.AllowMove(true);
            }
        }

        // définit la valeur de ADDOBSTACLE si la touche "1" est pressée ou relevée 
        if (Input.GetKeyDown(KeyCode.Keypad1)) { ADDOBSTACLE = true; }
        if (Input.GetKeyUp(KeyCode.Keypad1)) { ADDOBSTACLE = false; }

        // définit la valeur de ADDOBSTACLE si la touche "2" est pressée ou relevée 
        if (Input.GetKeyDown(KeyCode.Keypad2)) { DELETEOBSTACLE = true; }
        if (Input.GetKeyUp(KeyCode.Keypad2)) { DELETEOBSTACLE = false; }

        // durant la phase de SETUP
        if (SETUP) {
            if (ADDOBSTACLE) { plotObstacleWithMouse(); }
            if (DELETEOBSTACLE) { deleteObstacleWithMouse(); }
        }

        // affiche l'aide en pressant la touche "espace"
        if (Input.GetKeyDown(KeyCode.Space)) {
            help.text = Messages.messageHelp;
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            help.text = "";
        }

        // a tout instant, affiche puis efface le plus court chemin entre enemy et friend
        {
            if (Input.GetKeyDown("l")) {
                Hex enemyPosition = new Hex(enemy.transform.position.x, enemy.transform.position.z);
                Hex friendPosition = new Hex(friend.transform.position.x, friend.transform.position.z);
                plusCourtChemin = Hex.shortestPath(friendPosition, enemyPosition, obstacles, portée_de_tir);
                Hex.display(plusCourtChemin, tile, mapHexGameobject);
            }
            if (Input.GetKeyUp("l")) {
                Hex.purge(mapHexGameobject, plusCourtChemin);
            }
        }
        // a tout instant, affiche puis efface les éléments visibles par currentPlayer
        {
            if (Input.GetKeyDown("o")) {
                Hex currentPlayerPosition = new Hex(currentPlayer.transform.position.x, currentPlayer.transform.position.z);
                Visibles = Hex.visibleHex(currentPlayerPosition, obstacles, portée_de_tir);
                Hex.display(Visibles, tile, mapHexGameobject);
            }

            if (Input.GetKeyUp("o")) {
                Hex.purge(mapHexGameobject, Visibles);
            }
        }
    }

    private void plotObstacleWithMouse() {
        vecteurPosition = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                Hex g = new Hex(hit.point.x, hit.point.z);
                Hex.Plot(obstacle, g, mapHexGameobject);
                obstacles.Add(g);
            }
        }
    }

    private void deleteObstacleWithMouse() {
        vecteurPosition = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                Hex g = new Hex(hit.point.x, hit.point.z);
                List<Hex> atomicList = new List<Hex>();
                obstacles.Remove(g);
                atomicList.Add(g);
                Hex.purge(mapHexGameobject, atomicList);
            }
        }
    }
}


