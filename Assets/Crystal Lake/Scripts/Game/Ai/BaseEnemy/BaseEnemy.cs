using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace MyProj
{
    public class BaseEnemy : NetworkBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private EnemySoundTrigger soundTrigger;
        [Header("Vision Settings")]
        [SerializeField] private Transform enemyEye;
        [SerializeField] private float visionScanRate = 0.2f;
        [SerializeField] private float viewAngle = 120f;
        [SerializeField] private float viewDistance = 15f;
        [SerializeField] private float whatTimeIsWatchingSide = 1f;
        [Header("Noise Settings")]
        [SerializeField] private int radiusRotateHeadLeft = -45;
        [SerializeField] private int radiusRotateHeadRight = 45;
        [SerializeField, Tooltip("Погрешность растояния от точки шума до врага")]
        private float distanceToNoise = 5f;
        [SerializeField] private float killDistance = 3f;
        [SerializeField] private LayerMask layerEye;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private Transform[] AllSpotsOnMap;


        [SyncVar(hook = nameof(OnStateChanged))]
        private EnemyState currentState;

        public Transform EnemyEye => enemyEye;
        public float ViewAngle => viewAngle;
        public float ViewDistance => viewDistance;
        public LayerMask LayerEye => layerEye;
        public NavMeshAgent Agent => agent;
        public Transform CurrentTarget { get; private set; }
        public Vector3 NoisePosition { get; private set; }
        public Transform MyTransform { get; private set; }
        public Vector3 LastKnownPlayerPosition { get; private set; }
        public int RadiusRotateHeadLeft => radiusRotateHeadLeft;
        public int RadiusRotateHeadRight => radiusRotateHeadRight; 

        private FsmAiEnemy fsmEnemy;
        private float nextVisionScanTime;
        private bool hasTargetCached;

        private void Awake()
        {
            MyTransform = transform;
            NoisePosition = Vector3.zero;
            fsmEnemy = new FsmAiEnemy();

            fsmEnemy.AddState(new BaseStatePatrol(this, patrolPoints, soundTrigger));
            fsmEnemy.AddState(new BaseStateChase(this, killDistance));
            fsmEnemy.AddState(new BaseStateTrafficOnNoise(this, soundTrigger, distanceToNoise, whatTimeIsWatchingSide));
            fsmEnemy.AddState(new BaseStateSearching(this, distanceToNoise, whatTimeIsWatchingSide));
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetState(EnemyState.Patrol);
        }

        private void Update()
        {
            if (!isServer)
                return;

            fsmEnemy.UpdateState();
            if (CurrentTarget != null)
                Debug.Log($"Current target: {CurrentTarget.name}");
        }

        [Server]
        public void SetState(EnemyState state)
        {
            currentState = state;
        }

        [Server]
        public void SetNoisePosition(Vector3 position)
        {
            NoisePosition = position;
        }

        [Server]
        public void SetDestination(Vector3 destination)
        {
            Agent.SetDestination(destination);
        }

        [Server]
        public void StopDestination()
        {
            Agent.ResetPath();
            CurrentTarget = null;
            nextVisionScanTime = Time.time;
        }

        public bool TryFindTargetCached()
        {
            if (Time.time < nextVisionScanTime)
                return hasTargetCached;

            nextVisionScanTime = Time.time + visionScanRate;

            hasTargetCached = TryFindTarget();
            
            return hasTargetCached;
        }

        [Server]
        public bool TryFindTarget()
        {
            CurrentTarget = null;

            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null)
                    continue;

                Transform player = conn.identity.transform;

                CharacterController controller = player.GetComponent<CharacterController>();

                if (controller == null)
                    continue;

                Vector3 targetPoint = controller.bounds.center;
                Vector3 dir = targetPoint - enemyEye.position;

                float distance = dir.magnitude;

                if (distance > viewDistance)
                    continue;
  
                float angle = Vector3.Angle(enemyEye.forward, dir);

                if (angle > viewAngle * 0.5f)
                    continue;

                //Debug.DrawRay(enemyEye.position, dir.normalized * viewDistance, Color.red);

                if (Physics.Raycast(enemyEye.position, dir.normalized, out RaycastHit hit, viewDistance))
                {
                    if (hit.collider.transform.root.CompareTag("Player"))
                    {
                        CurrentTarget = player;
                        LastKnownPlayerPosition = player.position;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryFindSpecificGoal(Transform target)
        {
            CurrentTarget = null;

            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null)
                    continue;

                Transform player = conn.identity.transform;

                CharacterController controller = player.GetComponent<CharacterController>();

                if (controller == null)
                    continue;

                Vector3 targetPoint = controller.bounds.center;
                Vector3 dir = targetPoint - enemyEye.position;

                float distance = dir.magnitude;

                if (distance > viewDistance)
                    continue;

                float angle = Vector3.Angle(enemyEye.forward, dir);

                if (angle > viewAngle * 0.5f)
                    continue;

                //Debug.DrawRay(enemyEye.position, dir.normalized * viewDistance, Color.red);

                if (Physics.Raycast(enemyEye.position, dir.normalized, out RaycastHit hit, viewDistance))
                {
                    if (hit.collider.transform.root.CompareTag("Player") && player == target)
                    {
                        CurrentTarget = player;
                        return true;
                    }
                }
            }

            return false;
        }

        public void DrawViewState() // Отрисовка лучей для визуализации угла обзора врага в редакторе
        {
            Vector3 left = enemyEye.position + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * 15);
            Vector3 right = enemyEye.position + Quaternion.Euler(-new Vector3(0, viewAngle / 2f, 0)) * (enemyEye.forward * 15);
            Debug.DrawLine(enemyEye.position, left, Color.yellow);
            Debug.DrawLine(enemyEye.position, right, Color.yellow);
        }

        private void OnStateChanged(EnemyState oldState, EnemyState newState)
        {
            switch (newState)
            {
                case EnemyState.Patrol:
                    fsmEnemy.ChangeState<BaseStatePatrol>();
                    break;
                case EnemyState.Chase:
                    fsmEnemy.ChangeState<BaseStateChase>();
                    break;
                case EnemyState.TrafficOnNoise:
                    fsmEnemy.ChangeState<BaseStateTrafficOnNoise>();
                    break;
                case EnemyState.Searching:
                    fsmEnemy.ChangeState<BaseStateSearching>();
                    break;
                default:
                    Debug.LogWarning($"State {newState} not implemented yet");
                    goto case EnemyState.Patrol;
            }
        }
    }
}
