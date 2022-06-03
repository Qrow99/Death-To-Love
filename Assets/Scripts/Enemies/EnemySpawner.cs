using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Array that stores enemy prefabs, because of the weights it needs to be in a specific order: Henchman, Mugger, Bouncer")]
    [SerializeField] private GameObject[] Enemies;

    [Tooltip("Higher means more will spawn")]
    private float HenchmanWeight = 0f;

    [Tooltip("Higher means more will spawn")]
    private float MuggerWeight = 0f;

    [Tooltip("Higher means more will spawn")]
    private float BouncerWeight = 0f;

    [Tooltip("the time between when this gameobject is activated and when InvokeRepeating gets called the first time")]
    [SerializeField] private float SpawnDelay;

    [Tooltip("the time between enemy spawns")]
    [SerializeField] private float SpawnRate;

    [Tooltip("the max enemies this spawner will produce")]
    private float maxEnemies;

    [Tooltip("if true, keep spawning enemies until it is set to false, or the gameobject is disabled")]
    private bool infinite;

    private bool grounded = false;

    private bool endsWithDialogue;

    private bool complete = false; //if true then this spawner is done spawning
    private float netWeight;
    private Transform spawnpoint;
    private List<GameObject> spawnedEnemies;
    private List<float> weights;
    private CircleCollider2D circle;
    private WaveManager WM;
    private int Spawned;

    private DialogueManager _DialogueManager;
    // Start is called before the first frame update
    void Start()
    {
        GameObject _DialogueManagerObj = GameObject.FindGameObjectWithTag("DialogueManager");
        if (_DialogueManagerObj)
        {
            _DialogueManager = _DialogueManagerObj.GetComponent<DialogueManager>();
        }
        weights = new List<float>();

        spawnedEnemies = new List<GameObject>();
        spawnpoint = this.transform;
        circle = this.GetComponent<CircleCollider2D>();
    }

    public void SpawnEnemy()
    {
        if (_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            if ((Spawned < maxEnemies || infinite) && grounded)
            {
                if (!complete)
                {
                    GameObject newEnemy;

                    float randomWeight = Random.Range(0f, netWeight);
                    for (int i = 0; i < weights.Count; i++)
                    {
                        randomWeight -= weights[i];
                        if (randomWeight < 0)
                        {
                            newEnemy = Instantiate(Enemies[i], spawnpoint.position, Quaternion.identity);
                            spawnedEnemies.Add(newEnemy);
                            Spawned++;
                            newEnemy.GetComponent<EnemyAgent>().setSpawner(this);
                            break;
                        }
                    }
                }
            }
            else if (!infinite && Spawned >= maxEnemies)
            {
                doneSpawning();
            }
        }
    }

    public void doneSpawning()
    {
        CancelInvoke();
        WM.completeSpawning();
        complete = true;
    }

    public void startSpawning(WaveManager waveManager)
    {
        WM = waveManager;
        weights.Add(HenchmanWeight);
        weights.Add(MuggerWeight);
        weights.Add(BouncerWeight);
        netWeight = HenchmanWeight + MuggerWeight + BouncerWeight;
        complete = false;
        InvokeRepeating("SpawnEnemy", SpawnDelay, SpawnRate);
    }

    public void setMaxEnemies(float max)
    {
        maxEnemies = max;
    }

    public void setEnemyWeights(float Henchman, float mugger, float bouncer)
    {
        HenchmanWeight = Henchman;
        MuggerWeight = mugger;
        BouncerWeight = bouncer;
    }

    public void setHencmanWeight(float Henchman)
    {
        HenchmanWeight = Henchman;
    }

    public void setMuggerWeight(float mugger)
    {
        MuggerWeight = mugger;
    }
    public void setBouncerWeight(float bouncer)
    {
        BouncerWeight = bouncer;
    }

    public void setInfinite(bool inf)
    {
        infinite = inf;
    }

    public bool checkGrounded()
    {
       
        return grounded;
    }

    public void setGrounded(bool ground)
    {
        grounded = ground;
    }

    public bool checkComplete()
    {
        return complete;
    }

    public void resetSpawner()
    {
        infinite = false;
        complete = false;
        CancelInvoke();
        spawnedEnemies.Clear();
    }

    public float getMaxEnemies()
    {
        return maxEnemies;
    }

    public void removeEnemy(EnemyAgent enemy)
    {
        WM.removeEnemy();
        spawnedEnemies.Remove(enemy.gameObject);
    }
}
