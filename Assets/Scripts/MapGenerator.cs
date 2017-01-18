using System.Collections.Generic;
using UnityEngine;
using Hex = HexGridLib.Hex;

public class MapGenerator {

    public Hex hexOrigine;
    public int radius;
    public List<Hex> plateau;              // tous les Hex
    public List<Hex> listeObstacles;       // tous les Hex occupés par des obstacles
    public string seed;                    // pour random
    public bool useRandomSeed;
    public int randomFillPerCent;

    public MapGenerator( int rayon, int perCentFilling ) {
        listeObstacles = new List<Hex>();
        hexOrigine = new Hex(0, 0, 0);
        radius = rayon;
        plateau = Hex.neighborhood(hexOrigine, radius);
        randomFillPerCent = perCentFilling;
        useRandomSeed = true;
        GenerateMap();
    }

    void GenerateMap() {
        RandomFillMap();
        for (int i = 0; i < 4; i++)
            SmoothMap();
    }

    void RandomFillMap() {
        if (useRandomSeed) {
            seed = Network.time.ToString();
        }
        else {
            seed = "racinecarree1";
        }
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        // des obstacles sur la frontière
        foreach (Hex hex in Hex.Ring(hexOrigine, radius + 1))
            listeObstacles.Add(hex);

        //  remplissage aléatoire de l'intérieur
        foreach (Hex hex in plateau)
            if (pseudoRandom.Next(0, 100) < randomFillPerCent)
                listeObstacles.Add(hex);
    }

    void SmoothMap() {
        foreach (Hex hex in plateau) {
            int nbVoisins = hex.neighbourCount(listeObstacles);
            if (listeObstacles.Contains(hex)) {
                if (nbVoisins < 3) {
                    listeObstacles.Remove(hex);
                }
            }
            else if (nbVoisins > 5)
                listeObstacles.Add(hex);
        }
    }
}
