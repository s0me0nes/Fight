using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Visual Settings")]
    public Gradient hpGradient;
    public float smoothTime = 0.1f;
    [Header("Behaviour Settings")]
    public float distanceForAttack = 0.1f;
    public float attackCooldown = 1;

    public BattleCharacteristics Characteristics { get; set; }
    public Platform Platform { get; set; }

    private NavMeshAgent _agent;
    private MeshRenderer _meshRenderer;

    private float _hpVisual;
    
    private float _attackTimer;
    private float _attackOffset;
    
    private int _index;
    
    private bool _targetIsFounded;

    private void Awake()
    {
        TryGetComponent(out _agent);
        TryGetComponent(out _meshRenderer);
    }

    private void OnEnable()
    {
        _attackOffset = Random.Range(0, 1f);
    }

    private void Update()
    {
        _hpVisual = Mathf.Lerp(_hpVisual, Mathf.InverseLerp(0, 100, Characteristics.hp), 
            Time.deltaTime / smoothTime);
        _meshRenderer.material.color = hpGradient.Evaluate(_hpVisual);
        
        if (_targetIsFounded)
        {
            if (_index >= Platform.Enemies.Count)
            {
                _targetIsFounded = false;
                return;
            }

            Enemy enemy = Platform.Enemies[_index];
            
            Vector3 enemyPos = enemy.transform.position;
            
            if (_agent.destination != enemyPos)
                _agent.SetDestination(enemyPos);

            if (_agent.remainingDistance >= _agent.stoppingDistance + distanceForAttack)
                return;
            
            _attackTimer += Time.deltaTime;

            if (_attackTimer < attackCooldown)
                return;
            
            _attackTimer = -_attackOffset;
            if (enemy.IsDead(Characteristics.damage))
                _targetIsFounded = false;
        }
        else
        {
            float minDistance = 10000;
            int index = 0;

            bool founded = false;
            
            for (int i = 0; i < Platform.Enemies.Count; i++)
            {
                Enemy enemy = Platform.Enemies[i];
                float distance = (enemy.transform.position - transform.position).sqrMagnitude;
                
                if (enemy == this)
                    continue;

                if (enemy._targetIsFounded || distance >= minDistance * minDistance)
                    continue;
                
                minDistance = distance;
                index = i;

                founded = true;
            }

            if (!founded)
                return;

            _index = index;
            _targetIsFounded = true;
            Platform.Enemies[_index]._targetIsFounded = true;
        }
    }

    private bool IsDead(float damage)
    {
        Characteristics.hp -= damage;

        if (Characteristics.hp > 0)
            return false;

        Characteristics.hp = 0;
        Platform.Enemies.Remove(this);
        Destroy(gameObject);

        return true;
    }
}