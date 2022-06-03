using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mugger : EnemyAgent
{
    [SerializeField] protected GameObject Knife;

    // Start is called before the first frame update
    override protected void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("Cmvcam").GetComponent<CinemachineController>();
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

        if (player)
        {
            target = player.transform.position; //get the player's transform
        }

        Anim = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = MoveSpeed; //sets the movement speed of the agent

        //need these to keep the agent from moving weirdly, not sure how they work but the guy
        //in the tutorial used them 
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        BackupDist += 5; //we want the mugger to be far away, so whatever the basic backup distance is, make it farther
    }

    // Update is called once per frame
    override protected void Update()
    {
        Anim.SetBool("Walking", walking);
        if(GetComponent<Renderer>().isVisible)
        {
            if (!Staggered)
            {
                if (target != null) //if a player was found
                {
                    target = player.transform.position;
                    checkSide(); //make sure the enemy is oriented to be facing the target

                    if (Attack) //if we're able to attack and we are targeting a player
                    {
                        attack();
                    }
                    else
                    {

                        backup();
                    }
                }
                else
                {
                    //because the player doesnt spawn until input is given, we have to keep checking for a player
                    //This will 100% be removed before the final version
                    player = GameObject.FindGameObjectWithTag("Player");
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
                Anim.SetTrigger("Staggered");
                Debug.Log("Staggered");
            }
        }
        else
        {
            agent.SetDestination(player.transform.position);
        }
        
    }

    protected override void attack()
    {
        walking = true;
        if(Vector3.Distance(target, transform.position) < BackupDist)
        {
            if(Knife)
            {
                GameObject newKnife;
                AkSoundEngine.PostEvent("Enemy_Gun_Fire", gameObject);
                if (facingRight)
                {
                    newKnife = Instantiate(Knife, transform.position + new Vector3(1f, Sprite.bounds.size.y / 2f, 0f), Quaternion.Euler(0f,0f, 90f));
                }
                else
                {
                    newKnife = Instantiate(Knife, transform.position - new Vector3(1f, -1 * (Sprite.bounds.size.y / 2f), 0f), Quaternion.Euler(0f, 0f, 90f));
                }
                KnifeDamage KD  = newKnife.GetComponent<KnifeDamage>();
                if(KD)
                {
                    //Debug.Log("KD found");
                    KD.setThrower(this.gameObject);
                }
                else
                {
                    //Debug.Log("KD not found");
                }
            }
            Anim.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
        }
        else if (GetComponent<Renderer>().isVisible)
        {
            backup();
        }
    }

    protected override void backup()
    {
        Vector3 tempTarget = target;
        if(this.transform.position.x - tempTarget.x > 0)
        {
            //Debug.Log("Target on left");
            tempTarget += new Vector3(BackupDist, 0f, 0f);
        }
        else if (this.transform.position.x - tempTarget.x < 0)
        {
            //Debug.Log("Target on right");
            tempTarget -= new Vector3(BackupDist, 0f, 0f);
        }
        agent.SetDestination(tempTarget);
        walking = true;
    }
}
