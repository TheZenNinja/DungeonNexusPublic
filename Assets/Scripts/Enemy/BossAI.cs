using DG.Tweening;
using Enemy;
using System.Collections;
using UnityEngine;

public class BossAI : EnemyBase
{
    public int index;
    public Transform goalPointsParent;
    Transform[] goalPoints;

    public float moveSpeed;
    [Range(0f, 1f)]
    public float rotationRatio = .8f;

    public float waitTime = 3;

    [Space]
    [Header("Damage Stuff")]
    public Transform turret;
    public EnemyProjectile projectilePref;

    public int damage;
    public float delayBetweenShots;
    public float bulletSpeed;

    public Entity player;

    public void StartMovement(Transform goalParent)
    {
        this.goalPointsParent = goalParent;
        this.goalPoints = new Transform[goalPointsParent.childCount];

        for (int i = 0; i < goalPoints.Length; i++)
            goalPoints[i] = goalPointsParent.GetChild(i);

        StartCoroutine(BehaviourLoop());
    }

    private void Update()
    {
        if (player != null)
            turret.LookAt(player.position, Vector3.up);
    }

    void FixedUpdate()
    {
        //if (!isMoving)
        //{
        //    var goal = goalPoints[index];
        //    float duration = Vector3.Distance(transform.position, goal.position) / moveSpeed;
        //    StartCoroutine(MoveTo(goal.position, goal.eulerAngles, duration));
        //}
    }

    private IEnumerator BehaviourLoop()
    {
        while (true)
        {
            Vector3 endPos = goalPoints[index].position;
            Vector3 endRotation = goalPoints[index].eulerAngles;
            float duration = Vector3.Distance(transform.position, endPos) / moveSpeed;

            transform.DOMove(endPos, duration);
            transform.DORotate(endRotation, duration * .8f);
            yield return new WaitForSeconds(duration);

            GoToNextNextTarget();

            yield return new WaitForSeconds(waitTime);
        }

    }


    IEnumerator MoveTo(Vector3 endPos, Vector3 endRotation, float duration)
    {
        Debug.Log(duration);

        transform.DOMove(endPos, duration);
        transform.DORotate(endRotation, duration * .8f);
        yield return new WaitForSeconds(duration);

        GoToNextNextTarget();
    }
    public void GoToNextNextTarget()
    {
        index++;
        if (index >= goalPoints.Length)
            index = 0;
    }

    private void OnDrawGizmos()
    {
        if (goalPoints.Length == 0)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(goalPoints[0].position, goalPoints[goalPoints.Length - 1].position);
        Gizmos.DrawSphere(goalPoints[goalPoints.Length - 1].position, .25f);

        for (int i = 0; i < goalPoints.Length-1; i++)
        {
            Gizmos.DrawSphere(goalPoints[i].position, .25f);
            Gizmos.DrawLine(goalPoints[i].position, goalPoints[i+1].position);
        }
    }

    public override void SetTarget(Entity target)
    {
        player = target;
        StartCoroutine(ShootRoutine());
    }

    public override void SetStats(int hp, int dmg)
    {
        entity.Health.SetMaxHP(hp, true);
        damage = dmg;
        entity.Health.onDie.AddListener((_) => Die());
    }

    public IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenShots);

            var shotType = Random.Range(0, 5);


            Vector3 dirToTarget = (player.position - turret.position).normalized;


            if (shotType == 0)
                StartCoroutine(ShootBurst());
            else if (shotType == 1)
                ShootSpread();
            else
                ShootNormal();
        }
    }
       
    public void ShootNormal() => ShootNormal((player.position - turret.position).normalized);
    public void ShootNormal(Vector3 direction)
    { 
        var p = Instantiate(projectilePref, turret.position, Quaternion.identity);
        p.gameObject.SetActive(true);
        p.SetVeloctiy(direction * bulletSpeed);
        p.SetDamage(damage);
    }
    public IEnumerator ShootBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            ShootNormal();
            yield return new WaitForSeconds(.2f);
        }
    }
    public void ShootSpread()
    {
        float angle = 15;
        
        Vector3 dirL = DirFromAngle(turret.transform.eulerAngles.y - angle);
        Vector3 dirR = DirFromAngle(turret.transform.eulerAngles.y + angle);

        ShootNormal();
        ShootNormal(dirL);
        ShootNormal(dirR);
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void Die()
    {
        DOTween.Kill(transform);
        Destroy(gameObject);
    }
}
