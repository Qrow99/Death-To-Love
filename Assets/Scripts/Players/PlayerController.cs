// Contributor(s): Nathan More
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))] // Ensures object will have a Rigidbody2D
[RequireComponent(typeof(Animator))] // Ensures object will have an Animator
public class PlayerController : MonoBehaviour {
    //ScriptableObjects
    [SerializeField]
    private CharacterData _characterData;
    [SerializeField]
    private FloatAsset _health;
    private float MaxHealth;

    [SerializeField]
    private bool Invincible;

    [SerializeField]
    private float InvincibilitySeconds = 1f;
    [SerializeField]
    private float InvincibilityFlickerRate = 0.2f;
    private float InvincibilityflickerTimer;

    [SerializeField]
    private Transform _attackPos;
    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private LayerMask _environmentLayer;
    [SerializeField]
    private LayerMask _BulletLayer;

    //UI Elements
    [SerializeField]
    private GameObject _pausePanel;
    
    [SerializeField]
    private GameObject _playerUIPanel;
    [SerializeField]
    private DashUI dashUI;
    [SerializeField] 
    private Canvas DashCanvas;


    [SerializeField]
    private Image portrait;


    //[SerializeField]
    private string _characterName;
    //[SerializeField]
    private float _characterSpeed = 5.0f;
    //[SerializeField]
    private float _attackRange;
    //[SerializeField]
    private int _lightDamage = 1;
    //[SerializeField]
    private float _lightHitstun = 1f;
    //[SerializeField]
    private int _heavyDamage = 4;
    //[SerializeField]
    private float _heavyHitstun = 1f;
    //[SerializeField]
    private float _lightAtkCooldown = 0.5f;
    //[SerializeField]
    private float _heavyAtkCooldown = 1.5f;
    //[SerializeField] 
    private GameObject _hitParticleEmitter;
    //[SerializeField]
    private GameObject _slideEffect;

    private float _SlideCooldown;
    [SerializeField]
    private float SlideFlickerRate = 0.2f;
    private float SlideflickerTimer;
    private Color SlideColor;
    private bool White = true;

    private DialogueManager _DialogueManager;

    



    private Animator _playerAnim;
    private SpriteRenderer _playerSpriteRenderer;
    private Vector2 _movementInput = Vector2.zero;
    private bool _facingRight = true;
    private bool _sliding = false;
    private bool _canSlide = true;
    private Vector2 _slideVector = new Vector2(1, 0);
    private bool _canAttack = true;
    private bool _isPaused = false; 
    private PlayerInput _input;
    private bool _canMove = true;
    private bool _Downed = false;
    private float _Slideduration;
    //Heat System: 
    public HeatManager _heatManager; 

    private Vector3 _startPos;
    private Vector3 _centerPos;
    private Vector3 _endPos;

    public CharacterData CharacterData { get { return _characterData; } set { _characterData = value; } }

    [Header("Down Settings")]
    [SerializeField] private float mashGoal = 100;
    [SerializeField] private float IncreasePerPress = 5;
    [SerializeField] private float DecreaseRate = 5;
    [SerializeField] private Canvas DownCanvas;
    private Slider ReviveBar;
    private float mashProgress = 0;
    private void Start()
    {
        GameObject _DialogueManagerObj = GameObject.FindGameObjectWithTag("DialogueManager");
        if(_DialogueManagerObj)
        {
            _DialogueManager = _DialogueManagerObj.GetComponent<DialogueManager>();
        }
        ReviveBar = GetComponentInChildren<Slider>();
        ReviveBar.maxValue = mashGoal;
        DownCanvas.gameObject.SetActive(false);
        _playerAnim = gameObject.GetComponent<Animator>();

        _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _input = gameObject.GetComponent<PlayerInput>();

        //Debug.Log(_input.currentControlScheme);
        _health.Value = 60;
        SetupCharacter();
        InvincibilityflickerTimer = InvincibilityFlickerRate;
        SlideflickerTimer = SlideFlickerRate;
        SlideColor = new Color(254f/255f, 236f / 255f, 44f / 255f, 255f / 255f);
        MaxHealth = _health.Value;
        EnableUI(); // Activates player UI if player is spawned in
    }

    public void SetupCharacter()
    {
        _characterName = _characterData.CharacterName;
        _characterSpeed = _characterData.CharacterSpeed;
        _attackRange = _characterData.AttackRange;
        _lightDamage = _characterData.LightAtkDamage;
        _lightHitstun = _characterData.LightHitStun;
        _heavyDamage = _characterData.HeavyAtkDamage;
        _heavyHitstun = _characterData.HeavyHitStun;
        _lightAtkCooldown = _characterData.LightAtkCooldown;
        _heavyAtkCooldown = _characterData.HeavyAtkCooldown;
        _hitParticleEmitter = _characterData.HitParticleEmitter;
        _slideEffect = _characterData.SlideEffect;
        _SlideCooldown = _characterData.SlideCooldown;
        
        if(dashUI)
        {
            dashUI.gameObject.SetActive(false);
        }

        if (_playerAnim == null)
        {
            _playerAnim = gameObject.GetComponent<Animator>();
        }
        if (_playerSpriteRenderer == null)
        {
            _playerSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        _playerAnim.runtimeAnimatorController = _characterData.AnimatorController;
        _playerSpriteRenderer.sprite = _characterData.DefaultSprite;
        _playerAnim.SetFloat("LightAttackSpeed", _lightAtkCooldown);
        _Slideduration = _playerAnim.GetFloat("Slide Duration") / 2f;
    }

    // Gets direction from player input
    public void OnMove(InputAction.CallbackContext context)
    {
        if (_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            if (_canMove)
            {
                // not 100% sure why, but if you hold another direction while sliding,
                // the player will keep moving in the same direction
                // will reset when you let go of a direction/input a different direction
                //if (!_sliding && _canSlide) {
                if (!_sliding)
                {
                    _movementInput = context.ReadValue<Vector2>();
                }
            }
            else
            {
                _movementInput = Vector2.zero;
            }
        }
    }

    // Called when player presses light attack button
    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if(_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            if (!_Downed)
            {
                if (context.performed) // Ensures functions only performed once on button press
                {
                    //Debug.Log("light attack input");
                    if (_canAttack)
                    {
                        _playerAnim.SetTrigger("Light Attack");
                        AkSoundEngine.PostEvent("Player_Attack", gameObject);
                        _canSlide = false;
                        DisableAttack();
                        //I know this looks weird, but we're calling the attack function in animator on the frames damage is done
                    }
                }
            }
            else
            {
                if (context.performed) // Ensures functions only performed once on button press
                {
                    mashProgress += IncreasePerPress;
                    ReviveBar.value = mashProgress;
                    if (mashProgress >= mashGoal)
                    {
                        GetUp();
                    }
                }
            }
        }
        else if (_DialogueManager && context.performed) // Ensures functions only performed once on button press
        {
            _DialogueManager.OnPressNextButton();
        }
        
    }

    /*
    // Called when player presses heavy attack button
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if(!_Downed)
        {
            if (context.performed) // Ensures functions only performed once on button press
            {
                if (_canAttack)
                {
                    //Debug.Log("Heavy Attack!");
                    _playerAnim.SetTrigger("Heavy Attack");
                    AttackEnemies(_heavyDamage, _heavyHitstun);
                    StartCoroutine(AttackCooldown(_heavyAtkCooldown));
                }
            }
        }
    }
    */

    public void OnSlide(InputAction.CallbackContext context) {
        if (_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            if (context.performed && _canSlide && !_Downed) // Ensures functions only performed once on button press
            {
                //Debug.Log("Slide");
                //_playerAnim.SetTrigger("Slide");
                
                _playerAnim.SetBool("Sliding", true);
                AkSoundEngine.PostEvent("Player_Dash", gameObject);
                _movementInput = _slideVector;
                _sliding = true;
                _canSlide = false;
                //_canAttack = false;
                _characterSpeed *= 4f;
                _startPos = this.gameObject.transform.position;
                
                StartCoroutine(SlideSpeedReset());
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (_DialogueManager && _DialogueManager.IsDialogueOver() == false)
        {
            if (context.performed)
            {
                if (!_isPaused)
                {
                    _pausePanel.GetComponent<PauseMenuManager>().pause();
                }
                else
                {
                    _pausePanel.GetComponent<PauseMenuManager>().Resume(); //I put these into their own functions because we need the continue button on the pause menu to Resume and set _isPaused to false
                }
            }
        }
    }

   

    private IEnumerator SlideSpeedReset() 
    {
        yield return new WaitForSeconds(_Slideduration);
        dashUI.Dash();
        dashUI.gameObject.SetActive(true);
        _playerAnim.SetBool("Sliding", false);
        _characterSpeed /= 4f;
        _sliding = false;
        _endPos = this.gameObject.transform.position;
        _centerPos = (_startPos + _endPos) / 2;
        GameObject trail = Instantiate(_slideEffect, _centerPos, Quaternion.identity);
        FireTrail fire = trail.GetComponent<FireTrail>();
        Lightning lightning = trail.GetComponent<Lightning>();
        if(fire)
        {
            fire.setHeatManager(_heatManager);
        }
        else if(lightning)
        {
            lightning.setHeatManager(_heatManager);
        }
        float scaleX = Vector3.Distance(new Vector3(_startPos.x, 0, 0), new Vector3(_endPos.x, 0, 0));
        trail.transform.localScale = new Vector3(scaleX/5, 1, 1);
        _startPos = Vector3.zero;
        _endPos = Vector3.zero;
        _canAttack = true;

        _movementInput = Vector2.zero;
        // extra delay so a player can't spam sliding?
        yield return new WaitForSeconds(_SlideCooldown);
        //Debug.Log("Slide disabled");
        dashUI.gameObject.SetActive(false);
        _canSlide = true;
    }

    void FixedUpdate()
    {
        if(_Downed)
        {
            if(mashProgress >= 0)
            {
                mashProgress -= DecreaseRate * Time.deltaTime;
                ReviveBar.value = mashProgress;
            }
            
        }
        else 
        {
            PlayerMovement(); // Calls method to handle movement
        }

        if(_DialogueManager && _DialogueManager.IsDialogueOver() == true)
        {
            _movementInput = Vector2.zero;
        }

        if (_sliding)
        {
            SlideflickerTimer -= Time.deltaTime;
            if (White && SlideflickerTimer <= 0)
            {
                _playerSpriteRenderer.color = SlideColor;
                //_playerSpriteRenderer.color = Color.green;
                SlideflickerTimer = SlideFlickerRate;
                White = false;
            }
            else if (!White && SlideflickerTimer <= 0)
            {
                _playerSpriteRenderer.color = Color.white;
                SlideflickerTimer = SlideFlickerRate;
                White = true;
            }
        }
        else
        {
            _playerSpriteRenderer.color = Color.white;
        }
        
        if (Invincible && !_sliding)
        {
            InvincibilityflickerTimer -= Time.deltaTime;
            if (_playerSpriteRenderer.enabled == true && InvincibilityflickerTimer <= 0)
            {
                _playerSpriteRenderer.enabled = false; //deactivate sprite renderer
                InvincibilityflickerTimer = InvincibilityFlickerRate;
            }
            else if (_playerSpriteRenderer.enabled == false && InvincibilityflickerTimer <= 0)
            {
                _playerSpriteRenderer.enabled = true; //activate sprite renderer
                InvincibilityflickerTimer = InvincibilityFlickerRate;
            }
        }
        else
        {
            _playerSpriteRenderer.enabled = true;
        }

        
    }

    // Handles buffs that the player receives from Heat stacks, when a 0 is passed it resets the player's stats to normal
    public void BuffPlayer(int level)
    {
        if(level == 1 && _lightAtkCooldown == CharacterData.LightAtkCooldown)
        {
            _lightAtkCooldown *= 1.15f; 
        }
        else if (level >= 2 && _attackRange == CharacterData.AttackRange)
        {
            _attackRange *= 1.25f; 
        }
        else
        {
            _lightAtkCooldown = CharacterData.LightAtkCooldown;
            _attackRange = CharacterData.AttackRange; 
        }
        _playerAnim.SetFloat("LightAttackSpeed", _lightAtkCooldown);
    }



    // Handles player movement and animations
    public void PlayerMovement()
    {
        if (!_playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            // Adjusts vertical movement speed (halves it) and multiplies vector by time and speed
            Vector2 velocity = (new Vector2(_movementInput.x, _movementInput.y / 2)) * Time.deltaTime * _characterSpeed;

            if (velocity != Vector2.zero && _sliding == false) // Controls walking animation
            {
                _playerAnim.SetBool("Walking", true);
            }
            else
            {
                _playerAnim.SetBool("Walking", false);
            }
            if (velocity.x < 0 && _facingRight) // Controls direction player is facing
            {
                Flip();
            }
            else if (velocity.x > 0 && !_facingRight)
            {
                Flip();
            }

            // Moves character
            transform.position += new Vector3(velocity.x, velocity.y, 0);
        }
    }

    // Flips character horizontally
    public void Flip()
    {
        _facingRight = !_facingRight;
        _slideVector *= -1;
        transform.Rotate(Vector3.up * 180);
        DownCanvas.gameObject.GetComponent<RectTransform>().LookAt(Camera.main.transform.position);
        DashCanvas.gameObject.GetComponent<RectTransform>().LookAt(Camera.main.transform.position);
    }

    // Finds enemies in range and calls their TakeDamage() method.
    public void AttackEnemies()
    {
        //Debug.Log("damage: " + _lightDamage);
        Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(_attackPos.position, _attackRange, _enemyLayer);
        Collider2D[] destructablestoHit = Physics2D.OverlapCircleAll(_attackPos.position, _attackRange, _environmentLayer);
        Collider2D[] bulletstoHit = Physics2D.OverlapCircleAll(_attackPos.position, _attackRange, _BulletLayer);

        for(int i = 0; i < bulletstoHit.Length; i++)
        {
            if(bulletstoHit[i].GetComponent<KnifeDamage>().isHostile() == true)
            {
                bulletstoHit[i].GetComponent<KnifeDamage>().Reflect();
            }
        }

        for (int i = 0; i < destructablestoHit.Length; i++)
        {
            AkSoundEngine.PostEvent("Enemy_Hit", gameObject);
            destructablestoHit[i].GetComponent<EnemyAgent>().TakeDamage(_lightDamage, _lightHitstun);
            Instantiate(_hitParticleEmitter, destructablestoHit[i].gameObject.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
        }

        for (int i = 0; i < enemiesToHit.Length; i++)
        {
            AkSoundEngine.PostEvent("Enemy_Hit", gameObject);
            enemiesToHit[i].GetComponent<EnemyAgent>().TakeDamage(_lightDamage, _lightHitstun);
            Instantiate(_hitParticleEmitter, enemiesToHit[i].gameObject.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
            _heatManager.increaseHeat();
        }
        _canSlide = true;
    }

   
        // Creates a gizmo for attack area in editor
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPos.position, _attackRange);
    }

    public void TakeDamage(int damage)
    {
        if (!_sliding && !Invincible) 
        {
            AkSoundEngine.PostEvent("Player_Hit", gameObject);
            _health.Value -= damage;
            if (_health.Value <= 0)
            {
                PlayerDown();
            }
            StartCoroutine(InvincibilityFrames(InvincibilitySeconds));
            if(!_canAttack)
            {
                _canAttack = true;
            }
        }
    }

    public IEnumerator InvincibilityFrames(float time)
    {
        Invincible = true;
        yield return new WaitForSeconds(time);
        Invincible = false;
    }

    public void PlayerDown()
    {
        _Downed = true;
        AkSoundEngine.PostEvent("Player_Death", gameObject);
        _playerAnim.SetBool("Walking", false);
        DownCanvas.gameObject.SetActive(true);
        ReviveBar.value = 0;
        Debug.Log(this.tag + " Down!");
    }

    public void GetUp()
    {
        _Downed = false;
        ReviveBar.value = 0;
        mashProgress = 0;
        DownCanvas.gameObject.SetActive(false);
        _health.Value = 15f;
        Debug.Log("Player got back up!");
    }

    public bool checkDown()
    {
        return _Downed;
    }
    public IEnumerator AttackCooldown(float cooldown)
    {
        _canAttack = false;
        yield return new WaitForSeconds(cooldown);
        _canAttack = true;
    }

    public IEnumerator AnimationTime(float cooldown)
    {
        _canMove = false;
        yield return new WaitForSeconds(cooldown);
        _canMove = true;
    }

    public void EnableUI()
    {
        TextMeshProUGUI nametag = _playerUIPanel.GetComponentInChildren<TextMeshProUGUI>();
        //nametag.text = _characterName;
        portrait.sprite = _characterData.PortraitSprite;
        if(dashUI)
        {
            dashUI.setSlideDuration(_Slideduration);
            dashUI.setSlideCooldown(_SlideCooldown);
        }
        else
        {
            Debug.LogError("No Dash UI");
        }
        _playerUIPanel.SetActive(true);
    }

    public void EnableAttack()
    {
        _canAttack = true;
    }

    public void DisableAttack()
    {
        _canAttack = false;
    }

    public void setPaused(bool pause)
    {
        _isPaused = pause;
    }

    public void footstepSound()
    {
        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
    }

    public bool isInvincible()
    {
        return Invincible;
    }

    public void restoreHalfHealth()
    {
        if(_health.Value <= _health.Value/2f)
        {
            _health.Value += _health.Value / 2f;
        }
        else
        {
            _health.Value = MaxHealth;
        }
    }
}
