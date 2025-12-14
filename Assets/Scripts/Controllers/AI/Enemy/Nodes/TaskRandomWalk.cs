using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskRandomWalk : Node {
	private float _minDistance = 3.0f;
	private float _maxDistance = 7.0f;
	private float _maxTravelTime = 7.0f;

	private Vector3 _randomTarget;
	private float _travelTime = 0.0f;

	public override NodeState Evaluate(BaseBehaviourTree tree) {
		ActorType _enemyState = tree.ActorType;
		tree.Agent.speed = tree.Stats.Speed;
		if (_randomTarget == Vector3.zero) {
			_randomTarget = GetRandomPosition(tree.Agent);
		}

		tree.Agent.destination = _randomTarget;
		tree.Movement = (_randomTarget - tree.Agent.transform.position).normalized;
		tree.ActorAnimator.SetFloat("X", tree.Movement.x);
		tree.ActorAnimator.SetFloat("Y", tree.Movement.y);
		if (Vector2.Distance(tree.Agent.transform.position, _randomTarget) < 0.01f) {
			ResetNode();
		}
		else if (_travelTime >= _maxTravelTime) {
			ResetNode();
		}
		else {
			_travelTime += Time.deltaTime;
		}

		return NodeState.RUNNING;
	}

	private void ResetNode() {
		_randomTarget = Vector3.zero;
		_travelTime = 0f;
	}

	private Vector3 GetRandomPosition(NavMeshAgent agent) {
		Vector3 randomPos = RandomPoint(agent.transform);
		if (IsPathPossible(randomPos, agent)) {
			return randomPos;
		}
		else {
			return GetRandomPosition(agent);
		}
	}

	private Vector3 RandomPoint(Transform characterTransform) {
		float randomAngle = Random.Range(0f, Mathf.PI * 2);
		float randomDistance = Random.Range(_minDistance, _maxDistance);


		float offsetX = Mathf.Cos(randomAngle) * randomDistance;
		float offsetY = Mathf.Sin(randomAngle) * randomDistance;

		Vector2 randomPosition = new(characterTransform.position.x + offsetX,
									 characterTransform.position.y + offsetY);
		return randomPosition;
	}

	private bool IsPathPossible(Vector3 point, NavMeshAgent agent) {
		NavMeshPath navMeshPath = new();
		agent.CalculatePath(point, navMeshPath);
		return navMeshPath.status == NavMeshPathStatus.PathComplete;
	}
}