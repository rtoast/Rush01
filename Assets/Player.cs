using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator _anim;
    [SerializeField] private GameObject weapon;
    public NavMeshAgent _navMeshAgent;
    private Camera _camera;

    private Vector3 movePos;
    private GameObject _enemy;
    private int freedomLvl = 0;

    private int _health;
    private int _healthMax;

    public int STR = 10;
    public int AGI = 10;
    public int CON = 10;

    [HideInInspector]public int armor;
    [HideInInspector]public int damage;
    [HideInInspector]public int maxDamage;
    public int lvl = 0;
    public int score = 0;
    private int scoreForNextLevel = 100;
    public int money = 0;

    public Image _ImageScore;
    public Image _ImageHealth;
    public TextMeshProUGUI _textLvl;
    public TextMeshProUGUI _textScore;
    public TextMeshProUGUI _textHealth;
    
    void Start()
    {
        damage = STR / 2;
        maxDamage = damage + 4;
        _health = 5 * CON;
        _healthMax = _health;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        SetWeapon(weapon);
        // _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // _ImageScore.fillAmount = (float)(score % scoreForNextLevel)/scoreForNextLevel;
        // _ImageHealth.fillAmount = (float)_health / _healthMax;
        // _textLvl.text = "Lvl : "+lvl.ToString();
        // _textScore.text = score.ToString();
        // _textHealth.text = _health.ToString();
        
        if (Input.GetMouseButtonDown(0))
        {
            freedomLvl = 0;
            Vector3 arr = Input.mousePosition;
            arr.z = 0.5f;
            Vector3 mou1 = _camera.ScreenToWorldPoint(arr);
            arr.z = 1f;
            Vector3 mou2 = _camera.ScreenToWorldPoint(arr);

            Ray auf = new Ray(mou1, mou2 - mou1);
            RaycastHit _hit;
            
            if (Physics.Raycast(auf, out _hit, 10000))
            {
                if (_hit.transform.gameObject.tag == "Enemy")
                {
                    _enemy = _hit.transform.gameObject;
                    freedomLvl = 2;
                }
                else
                {
                    freedomLvl = 1;
                    movePos = _hit.point;
                }
            }
            // Debug.Log(_hit.collider);
        }
        switch (freedomLvl)
        {
            case 1:
                Move();
                break;
            case 2:
                MoveToAttack();
                break;
            case 3:
                Attack();
                break;
            case 0:
                _anim.SetBool("Attack" , false);
                _anim.SetBool("Run", false);
                break;
        }
    }

    void Move()
    {
        _anim.SetBool("Attack" , false);
        _anim.SetBool("Run", true);
        
        _navMeshAgent.SetDestination(movePos);
        // Debug.Log("auf");
        if ((movePos - transform.position).magnitude < 0.7f)
        {
            // Debug.Log("auf");
            freedomLvl = 0;
        }
        
    }

    void MoveToAttack()
    {
        _anim.SetBool("Attack" , false);
        _anim.SetBool("Run", true);
        
        _navMeshAgent.SetDestination(_enemy.transform.position);
        
        // Debug.Log("MoveAttack");

        if ((_enemy.transform.position - transform.position).magnitude < 2f)
        {
            freedomLvl = 3;
        }
    }

    void Attack()
    {
        // Debug.Log("Attack");
        _anim.SetBool("Attack" , true);
        _anim.SetBool("Run", false);
        if (!_enemy)
            freedomLvl = 0;
        if (_enemy && (_enemy.transform.position - transform.position).magnitude > 2.5f)
        {
            freedomLvl = 2;
        }
    }

    public void AttackAnimator()
    {
        // Debug.Log("AttackAnimator");
        if (_enemy)
        {
            int hit = Random.Range(1, 100);
            Enemy arr = _enemy.GetComponentInChildren<Enemy>();
            // Debug.Log(damage+" "+ maxDamage);
            if(hit <= 75 + AGI-arr.AGI)
                arr.GetDamage(Random.Range(damage, maxDamage));
        }
    }
    
    public void GetDamage(int damage)
    {
        if(_health <= 0)
            return;
        _health -= damage * (1 - armor/200);

        // Debug.Log(_health);
        if (_health <= 0)
        {
            Debug.Log("Maya is dead");
            _anim.SetBool("Attack", false);
            _anim.SetBool("Move", false);
            _anim.SetBool("Death", true);
            StartCoroutine(auf());
        }
    }

    IEnumerator auf()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        
        // gameObject.SetActive(false);
        yield break;
    }

    public static void SetWeapon(GameObject weapon)
    {
        var hand = GameObject.FindWithTag("RightHand");
        Debug.Log(weapon);
        Debug.Log(hand);
        Instantiate(weapon, hand.transform);
    }
    
}
