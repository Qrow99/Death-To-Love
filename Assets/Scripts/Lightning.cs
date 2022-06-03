using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] private float LifeTime = 3f;
    [SerializeField] private int damage;
    [SerializeField] private int hitstun;

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
        if (LifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemy = other.gameObject.GetComponent<EnemyAgent>();
            enemy.TakeDamage(damage, hitstun, false);
            HM.increaseHeat();
        }
    }

    public void setHeatManager(HeatManager Heat)
    {
        HM = Heat;
    }
}
