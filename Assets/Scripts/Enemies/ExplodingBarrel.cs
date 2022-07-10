using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : EnemyAgent
{
    [Header("Explosive Barrel Properties")]
    [SerializeField]
    private float _explosionTimer = 2f;

    [SerializeField]
    private float _explosionRadius = 2f;

    [SerializeField]
    private int _explosionDamage = 2;

    [SerializeField]
    private float _explosionHitstun = 2f;

    [SerializeField]
    private float _explosionKnockback = 2f;

    [SerializeField]
    private LayerMask _enemyLayer;

    [SerializeField]
    private LayerMask _environmentLayer;

    [SerializeField]
    private Rigidbody2D _RB;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void TakeDamage(int damage, float hitstun, Vector3 attackerPos, bool knockback = true, float knockbackForce = 0f)
    {
        if (knockback)
        {
            Vector2 forceVector = (this.transform.position - attackerPos).normalized * knockbackForce;
            _RB.AddForce(forceVector);
        }
        StartCoroutine(ExplosionTimer());
    }

    public override IEnumerator DamageOverTime(int DPS, HeatManager HM = null)
    {
        StartCoroutine(ExplosionTimer());
        yield return null;
    }

    public IEnumerator ExplosionTimer(bool knockback = true)
    {
        yield return new WaitForSeconds(2f);
        Explode();
    }

    private void Explode()
    {
        Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(this.transform.position, _explosionRadius, _enemyLayer);
        Collider2D[] destructablesToHit = Physics2D.OverlapCircleAll(this.transform.position, _explosionRadius, _environmentLayer);
        foreach (Collider2D col in enemiesToHit)
        {
            EnemyAgent Agent = col.GetComponent<EnemyAgent>();
            if(Agent)
            {
                Agent.TakeDamage(_explosionDamage, _explosionHitstun, this.transform.position, true, _explosionKnockback);
            }
        }

        foreach (Collider2D col in destructablesToHit)
        {
            EnemyAgent Agent = col.GetComponent<EnemyAgent>();
            if (Agent)
            {
                Agent.TakeDamage(_explosionDamage, _explosionHitstun, this.transform.position, true, _explosionKnockback);
            }
        }
        Destroy(this.gameObject);
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _explosionRadius);
    }
}
