using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    [SerializeField] private float LifeTime = 3f;
    [SerializeField] private int DPS = 1;

    private float damage = 0;
    private EnemyAgent enemy;

    private HeatManager HM;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if(LifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            enemy = other.gameObject.GetComponent<EnemyAgent>();
            StartCoroutine(enemy.DamageOverTime(DPS, HM));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(enemy)
        {
            enemy.StopCoroutine("DamageOverTime");
            enemy = null;
        }
    }

    public void setHeatManager(HeatManager Heat)
    {
        HM = Heat;
    }
}
