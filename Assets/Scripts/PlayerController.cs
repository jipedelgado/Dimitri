using UnityEngine;
using System.Collections;
using Hex = HexGridLib.Hex;

public class PlayerController : MonoBehaviour {

    public float speed = 1.0f;
    public float delayInput = 0.1f;

    private bool InputOK;
    private bool canMove;

    // pour accéder à obstacles de GameController.cs
    private GameController gameController;

    void Start() {
        InputOK = true;
    }
    private void Awake() {
        // attention l'objet GameController et le script associé ont le même nom !!
        GameObject go = GameObject.Find("GameController");
        gameController = go.GetComponent<GameController>();
    }

    void Update() {
        if (InputOK && canMove) {
            if (Input.GetKey(KeyCode.U))
                Move(0);
            if (Input.GetKey(KeyCode.Y))
                Move(1);
            if (Input.GetKey(KeyCode.G))
                Move(2);
            if (Input.GetKey(KeyCode.B))
                Move(3);
            if (Input.GetKey(KeyCode.N))
                Move(4);
            if (Input.GetKey(KeyCode.J))
                Move(5);
            Move(speed);
        }
    }

    public void AllowMove( bool flag ) {
        canMove = flag;
    }
    private IEnumerator wait( float delay ) {
        InputOK = false;
        yield return new WaitForSeconds(delay);
        InputOK = true;
    }

    private void Move( int direction ) {
        Hex g = new Hex(transform.position.x, transform.position.z);
        Hex h = Hex.Add(g, Hex.Direction(direction));
        stepFromTo(g, h);
    }

    private void Move( float speed ) {
        Vector3 movement = transform.position;
        movement.x += Input.GetAxis("Horizontal");
        movement.z += Input.GetAxis("Vertical");
        movement += movement * speed;
        Hex g = new Hex(transform.position.x, transform.position.z);
        Hex h = new Hex(movement.x, movement.z);
        stepFromTo(g, h);
    }

    private void stepFromTo(Hex Origine, Hex destination ) {
        StartCoroutine(wait(delayInput));
        
        // le mouvement n'est possible que si l'Hex de destination est libre d'obstacle
        if (!gameController.Obstacles.Contains(destination)) {
            // rotation
            Vector3 directionVector = Hex.vectorFromHex(Origine, destination);
            transform.rotation = Quaternion.LookRotation(directionVector);
            // déplacement
            Hex.setPosition(this.gameObject, destination);
        }
    }

}
