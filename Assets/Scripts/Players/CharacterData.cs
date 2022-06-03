using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "Character Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private float characterSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private int lightAtkDamage;
    [SerializeField] private float lightHitstun;
    [SerializeField] private int heavyAtkDamage;
    [SerializeField] private float heavyHitstun;
    [SerializeField] private float lightAtkCooldown;
    [SerializeField] private float heavyAtkCooldown;
    [SerializeField] private float slideCooldown;
    [SerializeField] private GameObject hitParticleEmitter;
    [SerializeField] private GameObject slideEffect;
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite portraitSprite;

    public string CharacterName { get { return characterName; } }
    public float CharacterSpeed { get { return characterSpeed; } }
    public float AttackRange { get { return attackRange; } }
    public int LightAtkDamage { get { return lightAtkDamage; } }
    public float LightHitStun { get { return lightHitstun; } }
    public int HeavyAtkDamage { get { return heavyAtkDamage; } }
    public float HeavyHitStun { get { return heavyHitstun; } }
    public float LightAtkCooldown { get { return lightAtkCooldown; } }
    public float HeavyAtkCooldown { get{ return heavyAtkCooldown; } }
    public float SlideCooldown { get { return slideCooldown; } }
    public GameObject HitParticleEmitter { get { return hitParticleEmitter; } }
    public GameObject SlideEffect { get { return slideEffect; } }
    public RuntimeAnimatorController AnimatorController { get { return animatorController; } }
    public Sprite DefaultSprite { get { return defaultSprite; } }
    public Sprite PortraitSprite { get { return portraitSprite; } }
}
