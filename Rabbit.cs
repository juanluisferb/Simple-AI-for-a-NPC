using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rabbit : MonoBehaviour
{

    [SerializeField] float ThresholdDistance;
    NavMeshAgent _agentRabbit;
    private bool _isFollowing;
    Animator _animator;
    GameManager gameManager;

    private bool _bIsCaged;
    private bool _bRandomMovementAct;

    //Instancia de la clase que contiene la lista

    [SerializeField] PlayerController _player;

    public bool SetIsFollowing(bool _updateFollowing)
    {
        _isFollowing = _updateFollowing;
        return _isFollowing;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        _agentRabbit = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _isFollowing = false;
        _bIsCaged = true;
        _bRandomMovementAct = false;
    }
    
    private void FixedUpdate()
    {
        _animator.SetFloat("SpeedMovement", _agentRabbit.velocity.magnitude);

        if (!_bIsCaged)
        {
            FollowPlayer();
        }
        
        if(!_bIsCaged && !_isFollowing)
        {
            if (!IsInvoking("RandomMovement") && !_bRandomMovementAct)
            {
                Invoke("RandomFreeMovement", 2.0f);
                _bRandomMovementAct = true;
            }
        }
    }

    private void RandomFreeMovement()
    {
        _agentRabbit.SetDestination(this.transform.position + new Vector3(Random.Range(-6, 6), 0, Random.Range(-6, 6)));
        _bRandomMovementAct = false;
    }

    private void FollowPlayer()
    {
        //Calculo en cada frame la distancia respecto al player, si ésta es menor que el threshold (radio del agente + distancia de stop), se para
        float distance = Vector3.Distance(_player.transform.position, this.transform.position);
        bool _bIsInThreshold = distance < ThresholdDistance + _agentRabbit.radius;

        if (_bIsInThreshold)
        {
            _agentRabbit.velocity = Vector3.zero;
            Debug.Log("me espero");
        }

        //bool de seguir al player
        if (_isFollowing)
        {
            if(distance < 1f)
            {
                //huir del player
            }
            
            //Con ésto los conejos siguen al player en una pseudo-fila
            _agentRabbit.SetDestination(_player.transform.position); //provisional
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        //Comienzo a seguir al player
        if (coll.CompareTag("Player") && !_isFollowing){
            //_agentRabbit.SetDestination(coll.transform.position);
            _isFollowing = true;
            gameManager.AddAnimalsToCounter();
            _bIsCaged = false;
        }
    }

    public bool GetRabbitIsFollowing()
    {
        return _isFollowing;
    }
}
