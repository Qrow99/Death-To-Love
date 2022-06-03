using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundGenerator : MonoBehaviour
{

    [SerializeField] private int SpritesToSpawn;
    [SerializeField] private List<GameObject> backgroundSprites;
    [SerializeField] private List<GameObject> rareSprites;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newSprite;
        float posX;
        float posY;
        float width = Screen.width;
        float height = Screen.height;
        GameObject SpriteToSpawn;
        int maxattempts = 10;
        int attempts;
        for (int i = 0; i < SpritesToSpawn; i++)
        {
            SpriteToSpawn = backgroundSprites[Random.Range(0, backgroundSprites.Count - 2)];
            if (Random.Range(0, 200) <= 1) //makes Liz's head a rare spawn
            {
                int coinFlip = Random.Range(0,2);
                if(coinFlip == 0)
                {
                    SpriteToSpawn = backgroundSprites[5];
                }
                else if (coinFlip == 1)
                {
                    SpriteToSpawn = backgroundSprites[6];
                }
            }

            posX = Random.Range(0, width);
            posY = Random.Range(0, height);
            newSprite = Instantiate(SpriteToSpawn, new Vector3(posX, posY, 0f), Quaternion.identity);
            BoxCollider2D col = newSprite.GetComponent<BoxCollider2D>();
            col.enabled = false;
            attempts = 0;
            while (Physics2D.OverlapCircle(newSprite.transform.position, 1f) && attempts < maxattempts)
            {
                Destroy(newSprite);
                posX = Random.Range(0, width);
                posY = Random.Range(0, height);
                newSprite = Instantiate(SpriteToSpawn, new Vector3(posX, posY, 0f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
                col = newSprite.GetComponent<BoxCollider2D>();
                col.enabled = false;
                attempts++;
            }
            col.enabled = true;
            newSprite.transform.localScale *= 0.5f;
            newSprite.transform.parent = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
