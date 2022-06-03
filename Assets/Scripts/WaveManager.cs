using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float MeleeWeight;
    [SerializeField] private float GunWeight;
    [SerializeField] private float BouncerWeight;
    [SerializeField] private float enemies = 30f;
    [SerializeField] private bool infinite = false;
    [SerializeField] private bool lockCam = false;
    [SerializeField] private List<EnemySpawner> UsableSpawners;
    [SerializeField] private GameObject goPanel; 
    [SerializeField] private float postFightDelay = 2f;

    private int completeSpawners = 0;
    private CinemachineController Cam;
    private bool spawning = false;
    private bool Fighting = false;
    [SerializeField]private bool DialogueAfterFight;
    [SerializeField] private DialogueTrigger DT;

    // Start is called before the first frame update
    void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("Cmvcam").GetComponent<CinemachineController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Player2")
        {
            //Debug.Log("start wave");
            activateSpawners();
            this.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void activateSpawners()
    {
        if (lockCam)
        {
            if(goPanel.activeSelf)
            {
                goPanel.SetActive(false); 
            }
            //locks camera to current position, commented out because this inadvertedly disables screenshake
            Cam.Snap();
        }
        float tempEnemies = 0;
        foreach(EnemySpawner spawner in UsableSpawners)
        {
            spawner.setEnemyWeights(MeleeWeight, GunWeight, BouncerWeight);
            spawner.setMaxEnemies(Mathf.Ceil(enemies / UsableSpawners.Count));
            spawner.setInfinite(infinite);
            spawner.startSpawning(this);
            spawning = true;
            tempEnemies += spawner.getMaxEnemies();
        }
        enemies = tempEnemies;
    }

    public void completeSpawning()
    {
        completeSpawners++;
        
        if(completeSpawners >= UsableSpawners.Count)
        {
            UsableSpawners.Clear();
            resetSpawners();
            spawning = false;
            Fighting = true;
        }
        else if(infinite)
        {
            resetSpawners();
            spawning = false;
            Fighting = true;
            UsableSpawners.Clear();
        }
    }

    public void removeEnemy()
    {
        enemies--;
        checkIfDoneFighting();
    }

    public void checkIfDoneFighting()
    {
        if(enemies <= 0)
        {
            //Debug.Log("No more fighting");
            StartCoroutine(PostFightDelay());
            
        }
    }

    IEnumerator PostFightDelay()
    {
        yield return new WaitForSeconds(postFightDelay);
        Cam.Unsnap();
        PlayerController[] PC = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in PC)
        {
            player.restoreHalfHealth();
        }
        goPanel.SetActive(true);
        Fighting = false;
        if (DialogueAfterFight)
        {
            if (DT)
            {
                DT.triggerDialogue();
            }
        }
    }

    private void resetSpawners()
    {
        //Debug.Log("Reset");
        foreach (EnemySpawner spawner in UsableSpawners)
        {
            spawner.resetSpawner();
        }
    }

    public bool getDoneFighting()
    {
        return Fighting;
    }
}
