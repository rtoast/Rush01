using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Animator _anim;

    public NavMeshAgent _navMeshAgent;
    private Camera _camera;

    private Vector3 movePos;
    private GameObject _enemy;
    private int freedomLvl = 0;
    private GameObject _player;

    public float distVision;
    private bool find;

    private int _health;
    private int _healthMax;

    public int STR = 10;
    public int AGI = 10;
    public int CON = 10;

    private bool wasHere;

    
    public int armor = 100;
    [HideInInspector]public int damage;
    [HideInInspector]public int maxDamage;
    public int lvl = 0;
    public int score = 0;
    private int scoreForNextLevel = 100;
    public int money = 0;

    private Image EnemyHealth;
    private TextMeshProUGUI EnemyText;

    private void OnMouseEnter()
    {
        EnemyHealth.fillAmount = (float)_health/_healthMax;
        EnemyText.text = transform.parent.name + "(" + lvl + ")";
    }

    private void OnMouseExit()
    {
        EnemyHealth.fillAmount = 0;
        EnemyText.text = "";
    }

    void Start()
    {
        EnemyHealth = GameObject.Find("EnemyHealth").GetComponent<Image>();
        EnemyText = GameObject.Find("EnemyText").GetComponent<TextMeshProUGUI>();
        
        damage = STR / 2;
        maxDamage = damage + 4;
        _health = 5 * CON;
        _healthMax = _health;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        // _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(_health <= 0 || !_player)
            return;

        if (!find && (_player.transform.position - transform.position).magnitude < distVision)
        {
            find = true;
            freedomLvl = 2;
            _enemy = _player;
        }
        
        if(freedomLvl == 1)
            Move();
        else if(freedomLvl == 2)
        {
            MoveToAttack();
        }
        else if(freedomLvl == 3)
        {
            Attack();
        }
        else if(freedomLvl == 0)
        {
            _anim.SetBool("Attack" , false);
            _anim.SetBool("Run", false);
        }
      
        
    }

    void Move()
    {
        _anim.SetBool("Attack" , false);
        _anim.SetBool("Run", true);
        
        _navMeshAgent.SetDestination(movePos);

        if ((movePos - transform.position).magnitude < 0.7f)
        {
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
        if(!_enemy)
            return;
        if ((_enemy.transform.position - transform.position).magnitude > 2.5f)
        {
            freedomLvl = 2;
        }
    }

    public void GetDamage(int damage)
    {
        if(_health <= 0)
            return;
        _health -= damage * (1 - armor/200);

        EnemyHealth.fillAmount = (float)_health/_healthMax;
        EnemyText.text = transform.parent.name + "(" + lvl + ")";
        
        // Debug.Log(_health);
        if (_health <= 0 && !wasHere)
        {
            wasHere  = true;
            EnemyHealth.fillAmount = 0;
            EnemyText.text = "";
            _anim.SetBool("Attack", false);
            _anim.SetBool("Move", false);
            _anim.SetBool("Death", true);
            StartCoroutine(auf());
        }
    }

    IEnumerator auf()
    {
        var auf = _enemy.GetComponentInChildren<Player>();
        auf.score += score;
        auf.money += money;
        
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        
        Destroy(_navMeshAgent.gameObject);
        yield break;
    }
    public void AttackAnimator()
    {
        if (_enemy)
        {
            int hit = Random.Range(1, 100);
            Player arr = _enemy.GetComponentInChildren<Player>();
            if(hit <= 75 + AGI-arr.AGI)
               arr.GetDamage(Random.Range(damage, maxDamage));
        }
    }
}
