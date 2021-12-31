using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour {

	public Transform target;
	private NavMeshAgent navMeshAgent;

	public bool playerDetected = false;
	public float speedMin;
	public float speedMax;
	float speed;
	public float angularSpeedMin;
	public float angularSpeedMax;
	float angularSpeed;
	public float accelerationMin;
	public float accelerationMax;
	float acceleration;
	public float awareRangeMin = 6;
	public float awareRangeMax = 10;
	float awareRange;
	public Animator animator;
	Vector3 prePos;
	Vector3 curPos;
	public GameObject createOnDestroy;
	public int damage;

	private void Awake() {
		navMeshAgent = GetComponent<NavMeshAgent>();
		speed = Random.Range(speedMin, speedMax);
		angularSpeed = Random.Range(angularSpeedMin, angularSpeedMax);
		acceleration = Random.Range(accelerationMin, accelerationMax);
	}
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		prePos = transform.position;
		target = GameObject.FindWithTag("Player").transform;
		awareRange = Random.Range(awareRangeMin, awareRangeMax);
	}
	private void Update() {
		curPos = transform.position;
		if(target == null){
			target = GameObject.FindWithTag("Player").transform;
		}
		else if((transform.position - target.position).magnitude * 0.6 > awareRange && !playerDetected){
		}
		else{
			if(curPos != prePos){
				animator.SetInteger("isWalking", 1);
			}else{
				animator.SetInteger("isWalking", 0);
			}
			transform.LookAt(target);
			playerDetected = true;
			navMeshAgent.speed = speed;
			navMeshAgent.acceleration = acceleration;
			navMeshAgent.angularSpeed = angularSpeed;
			// Debug.Log(timeManager.Ts);
			navMeshAgent.SetDestination(target.position);
		}
	}
/// <summary>
/// OnCollisionEnter is called when this collider/rigidbody has begun
/// touching another rigidbody/collider.
/// </summary>
/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.CompareTag("Player")){
			if(gameObject.CompareTag("Mimic")){
				GameObject player = other.gameObject;
            	player.GetComponent<HealthManager>().ApplyDamage(damage);
            	GameObject obj = Instantiate(this.createOnDestroy);
            	obj.transform.position = this.transform.position;
			}
        }
	}
}