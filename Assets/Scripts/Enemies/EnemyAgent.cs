// Contributor(s): Ben, Nathan More
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    [SerializeField] public int Health = 5;

    //the object this agent is moving towards
    [SerializeField] protected Vector3 target;
    [SerializeField] protected float attackRange = 1f; //distance the enemy's attack can reach
    [SerializeField] protected float MoveSpeed = 1.5f; //speed that the agent moves at
    [SerializeField] protected float AttackSpeed = 3f; //rate at which enemy can attack, higher number means less frequent attacks
    [SerializeField] protected float BackupDist = 3f; //rate at which enemy can attack, higher number means less frequent attacks
    [SerializeField] private Transform attackPos; // Location of attack range circle
    [SerializeField] private LayerMask playerLayer; // Player layer mask
    [SerializeField] private int attackDamage; // Damage dealt
    [SerializeField] private float attackArea; // Area of circle for melee attacks
    [SerializeField] private float knockbackDist = 1f;
    [SerializeField] protected float KnockbackVelocity = 1f;
    [SerializeField] protected float tauntChance = 1f;
    [SerializeField] protected GameObject hitParticles;



    // Determines if enemy is facing right
    protected bool facingRight = false;

    //reference to the NavmeshAgent component on this gameobject
    protected NavMeshAgent agent;

    //references the sprite renderer for the enemy
    protected SpriteRenderer Sprite;

    //Controls the animations for the AI
    [SerializeField] protected Animator Anim;

    //bool that determines if the AI is chasing the player or not, in theory the AI should chase if the player is outside of attack range
    protected bool chase = true;

    //if true, AI can attack, if false, AI is in a state where he cannot attack
    protected bool Attack = true;

    //if true, AI can attack, if false, AI is in a state where he cannot attack
    protected bool Staggered = false;

    //if true, AI is walking somewhere
    protected bool walking = false;

    //reference to the player
    protected GameObject player;
    
    protected CinemachineController Cam;

    protected Vector3 knockbackVector;
    protected Vector3 knockbackVelocityVector;

    protected bool dying = false;

    private EnemySpawner _spawner;

    private DialogueManager _DialogueManager;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        GameObject _DialogueManagerObj = GameObject.FindGameObjectWithTag("DialogueManager");
        if (_DialogueManagerObj)
        {
            _DialogueManager = _DialogueManagerObj.GetComponent<DialogueManager>();
        }
        if (GameManagerScript.Instance.IsMultiplayer == true)
        {
            int targetPlayer = Random.Range(0, 2); //randomly picks either player 1 or player 2 as a target
            if (targetPlayer == 0) //if player1 is selected, or we are in single player mode (add or statement when gamemanager is ready)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player2");
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        if(player)
        {
            target = player.transform.position; //get the player's transform
        }
        knockbackVelocityVector = new Vector3(KnockbackVelocity, 0f, 0f);
        Anim = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        Cam = GameObject.FindGameObjectWithTag("Cmvcam").GetComponent<CinemachineController>();

        agent.speed = MoveSpeed; //sets the movement speed of the agent

        //need these to keep the agent from moving weirdly, not sure how they work but the guy
        //in the tutorial used them 
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            float taunt = Random.Range(0, 100001);
            if(taunt <= tauntChance)
            {
                Debug.Log("Taunt");
                AkSoundEngine.PostEvent("Enemy_Taunt", gameObject);
            }

            Anim.SetBool("Walking", walking);
            if (!Staggered)
            {

                if (target != null) //if a player was found
                {
                    checkSide(); //make sure the enemy is oriented to be facing the target

                    if (Attack) //if we're able to attack and we are targeting a player
                    {
                        attack();
                    }
                    else
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target);
                        if (distanceToTarget < 0.5)
                        {
                            idle();
                        }
                    }



                }
                else
                {
                    //If we can't find a player, we should keep looking for one
                    int targetPlayer = Random.Range(0, 2); //randomly picks either player 1 or player 2 as a target
                    if (targetPlayer == 0) //if player1 is selected, or we are in single player mode (add or statement when gamemanager is ready)
                    {
                        player = GameObject.FindGameObjectWithTag("Player");
                    }
                    else
                    {
                        player = GameObject.FindGameObjectWithTag("Player2");
                    }


                    if (player)
                    {
                        target = player.transform.position; //get the player's transform
                    }
                    else
                    {
                        idle();
                    }
                }
            }
            else
            {
                //play stagger animation
                //Debug.Log("Staggered");
                Anim.SetTrigger("Staggered");
                this.transform.position += new Vector3(knockbackVector.x * Time.deltaTime, 0f, 0f); //Vector3.SmoothDamp(transform.position, , ref knockbackVelocityVector, 1f);
            }
        }
    }

    //when the agent isnt attacking, make them run away by the backup dist
    protected virtual void backup()
    {
        walking = false;
        Anim.SetBool("BackingUp", true);
        int headsOrTails = Random.Range(0, 2);
        if (headsOrTails == 0)
        {
            //Debug.Log("Minus");
            target -= new Vector3(BackupDist, 0f, 0f);
        }
        else
        {
            //Debug.Log("Plus");
            target += new Vector3(BackupDist, 0f, 0f);
        }
        agent.SetDestination(target); 
    }

    //I imagine flipping an enemy will take more than just flipping the sprite, so this flips the enemy
    protected void checkSide()
    {
        //check the sprite's relative distance to the player, if the distance on the X is negative, then the enemy is on the left of the player and we need to flip it
        if ((this.transform.position - player.transform.position).x < 0 && facingRight == false)
        {
            Flip();
        }
        else if ((this.transform.position - player.transform.position).x > 0 && facingRight == true)
        {
            Flip();
        }
    }

    // Flips character horizontally
    public void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(Vector3.up * 180);
    }

    protected virtual void attack()
    {
        
        target = player.transform.position;
        walking = true;
        Anim.SetBool("BackingUp", false);
        if (Vector3.Distance(target, transform.position) < attackRange) //if enemy is in range of player, attack
        {
            //Debug.Log("Enemy attacks!");
            Anim.SetTrigger("Attack");
            //MeleeAttack();
            StartCoroutine(AttackCooldown());
            
        }
        else
        {
            agent.SetDestination(target);
        }
    }

    // Handles incoming attacks
    public virtual void TakeDamage(int damage, float hitstun, bool knockback = true)
    {
        //Debug.Log(gameObject.name + " took " + damage + " damage.");
        Health -= damage; 
        if(knockback)
        {
            if (facingRight)
            {
                knockbackVector = new Vector3(this.transform.position.x, 0f, 0f)- new Vector3(knockbackDist, 0f, 0f);
            }
            else
            {
                knockbackVector = new Vector3(this.transform.position.x, 0f, 0f) + new Vector3(knockbackDist, 0f, 0f);
            }
        }
        else
        {
            knockbackVector = this.transform.position;
        }

        if(Health <= 0)
        {
            //Debug.Log("He's dead, you can stop mashing now");
            Die();
        }
        else
        {
            //stagger the enemy
            StartCoroutine(Stagger(hitstun));
        }
        if(Cam)
        {
            Cam.shake();
        }
    }

    public virtual IEnumerator DamageOverTime(int DPS, HeatManager HM = null)
    {
        while(true)
        {
            Health -= DPS;
            if(HM)
            {
                HM.increaseHeat();
            }
            if (Health <= 0)
            {
                Die();
                yield break;
            }
            knockbackVector = this.transform.position;
            StartCoroutine(Stagger(0.1f));
            yield return new WaitForSeconds(1f);
        }
    }

    public virtual void Die()
    {
        if(!dying)
        {
            dying = true;
            if(_spawner)
            {
                _spawner.removeEnemy(this);
            }
            AkSoundEngine.PostEvent("Enemy_Death", gameObject);
            Destroy(this.gameObject); //destroy actually has the ability to add a delay, so once we get an animation for death we can delay destroying until the animation is done
        }
        
    }

    protected IEnumerator Stagger(float hitstun)
    {
        Staggered = true;
        walking = false;
        Anim.SetBool("BackingUp", false);
        agent.isStopped = true;
        yield return new WaitForSeconds(Mathf.Abs(hitstun)); //the 0.01f converts seconds into an increment of 10 milliseconds
        Staggered = false;
        walking = true;
        agent.isStopped = false;
    }

    //set agent to just stand where it is located
    protected void idle()
    {
        agent.SetDestination(agent.transform.position);
        walking = false;
        Anim.SetBool("BackingUp", false);
    }

    //waits AttackSpeed Seconds between attacks
    protected IEnumerator AttackCooldown()
    {
        backup();
        Attack = false;
        yield return new WaitForSeconds(Mathf.Abs(AttackSpeed));
        Attack = true;
    }

    // Creates a gizmo for attack area in editor
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackArea);
    }

    public void MeleeAttack()
    {
        AkSoundEngine.PostEvent("Enemy_Melee_Attack", gameObject);
        Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(attackPos.position, attackArea, playerLayer);
        for (int i = 0; i < enemiesToHit.Length; i++)
        {
            //Debug.Log("At least one player");
            enemiesToHit[i].GetComponent<PlayerController>().TakeDamage(attackDamage);
            Instantiate(hitParticles, enemiesToHit[i].gameObject.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
        }
    }

    public void setSpawner(EnemySpawner spawner)
    {
        _spawner = spawner;
    }

    public GameObject getParticleEffect()
    {
        return hitParticles;
    }
}
