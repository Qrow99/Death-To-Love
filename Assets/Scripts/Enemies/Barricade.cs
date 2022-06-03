using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barricade : EnemyAgent
{
    [SerializeField] WaveManager WM;
    [SerializeField] Slider HealthBar;
    [SerializeField] LevelLoader LL;
    // Start is called before the first frame update
    protected override void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("Cmvcam").GetComponent<CinemachineController>();
    }

    protected override void Update()
    {

    }
    public override void OnDrawGizmosSelected()
    {
    }

    public override void TakeDamage(int damage, float hitstun = 0f, bool knockback = false)
    {
        Debug.Log("Barrier took damage");
        Health -= damage;
        if (Health <= 0)
        {
            Debug.Log("Barricade Destroyed");
            Die();
        }
        if (Cam)
        {
            Cam.shake();
        }
        HealthBar.value = Health;
    }

    
    public override void Die()
    {
        if (!dying)
        {
            WM.completeSpawning();
            dying = true;
            LL.LoadLevel("Start Menu");
            Destroy(this.gameObject); //destroy actually has the ability to add a delay, so once we get an animation for death we can delay destroying until the animation is done
        }

    }
}
