using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private List<EnemySpawner> spawners;
    // Start is called before the first frame update
    void Start()
    {
        spawners = new List<EnemySpawner>();
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("EnemySpawner"))
        {
            spawners.Add(spawner.GetComponent<EnemySpawner>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemySpawner")
        {
            collision.gameObject.GetComponent<EnemySpawner>().setGrounded(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "EnemySpawner")
        {
            collision.gameObject.GetComponent<EnemySpawner>().setGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "EnemySpawner")
        {
            other.gameObject.GetComponent<EnemySpawner>().setGrounded(false);
        }
    }
}
