using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace MyProj
{
    public enum StateVisionEnemy
    {
        None, ByHalf, HadSeen
    }

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
        [SerializeField, Tooltip("Время, через которое враг замечает и бежит на игрока")]
        private float afterHowLongDoINoticeThePlayer = 1f;
        [SerializeField, Tooltip("Время, через которое враг пойдёт посмотреть на место где он видел силуэт игрока")]
        private float investigateSilhouetteDelay = 0.5f;
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
        public EnemySoundTrigger SoundTrigger => soundTrigger;
        public Vector3 CheckPosition { get; set; }
        public Vector3 LastMoveDirection { get; private set; }

        private Vector3 previousSeenPosition;

        private FsmAiEnemy fsmEnemy;
        private float nextVisionScanTime;
        private StateVisionEnemy hasTargetCached;
        private float currentPlayerVisibleTime; // Время, в течение которого игрок был видим врагу
        //private byte countPlayerVisibleFrames; // Количество кадров, в течение которых игрок был видим врагу

        private void Awake()
        {
            MyTransform = transform;
            NoisePosition = Vector3.zero;
            fsmEnemy = new FsmAiEnemy();
            //countPlayerVisibleFrames = (byte)(afterHowLongDoINoticeThePlayer / visionScanRate);

            fsmEnemy.AddState(new BaseStatePatrol(this, patrolPoints));
            fsmEnemy.AddState(new BaseStateChase(this, killDistance));
            fsmEnemy.AddState(new BaseStateTrafficOnNoise(this, distanceToNoise, whatTimeIsWatchingSide));
            fsmEnemy.AddState(new BaseStateSearching(this, distanceToNoise, whatTimeIsWatchingSide));
            fsmEnemy.AddState(new BaseStateCheckPosition(this, distanceToNoise, whatTimeIsWatchingSide));
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
            nextVisionScanTime = Time.time;
        }

        public StateVisionEnemy TryFindTargetCached(out Transform currentTarget)
        {
            currentTarget = this.CurrentTarget;
            if (Time.time < nextVisionScanTime)
                return hasTargetCached;

            nextVisionScanTime = Time.time + visionScanRate;

            var oldHasTargetCached = CurrentTarget;
            var hasVision = TryFindTarget();
           

            if (hasVision == true && CurrentTarget != null && CurrentTarget == oldHasTargetCached)
            {
                currentPlayerVisibleTime += visionScanRate;

                if (currentPlayerVisibleTime >= afterHowLongDoINoticeThePlayer)
                {
                    hasTargetCached = StateVisionEnemy.HadSeen;
                }
            }
            else if (CurrentTarget != oldHasTargetCached)
            {
                if (currentPlayerVisibleTime >= afterHowLongDoINoticeThePlayer)
                {
                    hasTargetCached = StateVisionEnemy.HadSeen;
                }
                else if (currentPlayerVisibleTime >= investigateSilhouetteDelay)
                {
                    hasTargetCached = StateVisionEnemy.ByHalf;
                }
                else
                {
                    hasTargetCached = StateVisionEnemy.None;
                }
                currentTarget = oldHasTargetCached;
                currentPlayerVisibleTime = visionScanRate;
            }
            else
            {
                if (currentPlayerVisibleTime >= investigateSilhouetteDelay)
                {
                    hasTargetCached = StateVisionEnemy.ByHalf;
                }
                else
                {
                    hasTargetCached = StateVisionEnemy.None;
                }
                currentTarget = oldHasTargetCached;

                currentPlayerVisibleTime = 0f;
            }

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
                        Vector3 currentPos = player.position;

                        if (previousSeenPosition != Vector3.zero)
                        {
                            Vector3 delta = currentPos - previousSeenPosition;

                            if (delta.sqrMagnitude > 0.001f)
                            {
                                LastMoveDirection = delta.normalized;
                            }
                        }

                        previousSeenPosition = currentPos;

                        CurrentTarget = player;
                        LastKnownPlayerPosition = currentPos;

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
                case EnemyState.CheckPosition:
                    fsmEnemy.ChangeState<BaseStateCheckPosition>();
                    break;
                default:
                    Debug.LogWarning($"State {newState} not implemented yet");
                    goto case EnemyState.Patrol;
            }
        }
    }
}
