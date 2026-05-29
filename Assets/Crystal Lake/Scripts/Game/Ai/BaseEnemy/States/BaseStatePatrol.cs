using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyProj
{
    public class BaseStatePatrol : FsmAIEnemyState
    {
        private Transform[] points;
        private Vector3 nextPoint;
        private Queue<Transform> queryPoint;
        private EnemySoundTrigger soundTrigger;

        public BaseStatePatrol(BaseEnemy enemy, Transform[] _points, EnemySoundTrigger _soundTrigger) : base(enemy)
        {
            points = _points;
            soundTrigger = _soundTrigger;

            queryPoint = new Queue<Transform>(_points.Length);
            CreateQuery();
            nextPoint = queryPoint.Dequeue().position;
        }

        public override void EnterState()
        {
            NearestPoint();
            soundTrigger.OnPlayerDetected += OnPlayerDetectedThroughSound;
        }

        public override void ExitState()
        {
            soundTrigger.OnPlayerDetected -= OnPlayerDetectedThroughSound;
        }

        public override void UpdateState()
        {
            //enemy.DrawViewState();
            if (enemy.TryFindTargetCached())
            {
                enemy.SetState(EnemyState.Chase);
            }

            MoveToPoint();
        }

        private void OnPlayerDetectedThroughSound(Vector3 soundPosition)
        {
            enemy.SetNoisePosition(soundPosition);
            enemy.SetState(EnemyState.TrafficOnNoise);
        }

        private void NearestPoint()
        {
            if (points.Length == 0) return;

            Transform nearestPoint = null;
            float minDistance = float.MaxValue;

            foreach (var point in points)
            {
                float distance = Vector3.Distance(point.position, enemy.MyTransform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }

            if (nearestPoint != null)
            {
                nextPoint = nearestPoint.position;

                var newQueue = new Queue<Transform>();
                bool pointRemoved = false;

                foreach (var point in queryPoint)
                {
                    if (!pointRemoved && point == nearestPoint)
                    {
                        pointRemoved = true;
                        continue;
                    }
                    newQueue.Enqueue(point);
                }

                queryPoint = newQueue;
            }
        }

        private void MoveToPoint()
        {
            enemy.SetDestination(nextPoint);

            if (Vector3.Distance(nextPoint, enemy.MyTransform.position) < 0.15f)
            {
                NextPointUpdate();
            }
        }

        private void NextPointUpdate()
        {
            if (queryPoint.Count == 0)
            {
                CreateQuery();
            }

            nextPoint = queryPoint.Dequeue().position;
        }

        private void CreateQuery()
        {
            System.Random rng = new System.Random();
            var shuffledPoints = points.OrderBy(x => rng.Next()).ToArray();

            for (int i = 0; i < points.Length; i++)
            {
                queryPoint.Enqueue(shuffledPoints[i]);
            }
        }
    }
}