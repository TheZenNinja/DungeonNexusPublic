using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 direction;
    public float speed;
    public float gravity;

    public UnityAction<GameObject> onHitAction;
    public System.Action<GameObject> onHit;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, float speed, float gravity)
    {
        this.direction = direction;
        this.speed = speed;
        this.gravity = gravity;
        velocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        if (gravity != 0)
            velocity += transform.up * gravity;

        rb.velocity = velocity;
        transform.forward = velocity.normalized;
    }

    public void OnTriggerEnter(Collider other)
    {
        onHitAction?.Invoke(other.gameObject);
        onHit?.Invoke(other.gameObject);
    }
}
