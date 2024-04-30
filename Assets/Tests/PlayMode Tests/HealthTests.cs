using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTests
{
    private Health CreateHealth()
    {
        GameObject g = new GameObject();
        Health health = g.AddComponent<Health>();

        health.SetMaxHP(25);
        health.Initialize();

        return health;
    }

    [Test]
    public void ArmorBehavior()
    {
        bool onHitCB = false;
        bool onDamageCB = false;
        bool onDamageHPCB = false;

        Health health = CreateHealth();
        Assert.NotNull(health);

        health.onHit.AddListener((_) => onHitCB = true);
        health.onTakeDamage.AddListener((_) => onDamageCB = true);
        health.onTakeDamageToHealth.AddListener((_) => onDamageHPCB = true);

        health.AddArmor(10, 10);
        health.TakeDamage(5);

        //armor takes dmg before hp
        Assert.AreEqual(health.Armor, 5);
        Assert.AreEqual(health.HP, 25);
        //check callbacks
        Assert.IsTrue(onHitCB);
        Assert.IsTrue(onDamageCB);
        Assert.IsFalse(onDamageHPCB);

        //armor cap works
        health.AddArmor(10, 10);
        Assert.AreEqual(health.Armor, 10);

        //health getting delt dmg after armor
        health.TakeDamage(15);
        Assert.AreEqual(health.HP, 20);
        Assert.IsTrue(onDamageHPCB);
    }

    [Test]
    public void BarrierBehavior()
    {
        bool onHitCB = false;
        bool onDamageCB = false;

        Health health = CreateHealth();
        Assert.NotNull(health);

        health.onHit.AddListener((_) => onHitCB = true);
        health.onTakeDamage.AddListener((_) => onDamageCB = true);

        health.SetBarrier(true);
        health.TakeDamage(10);
        Assert.AreEqual(health.HP, 25);
        Assert.IsTrue(onHitCB);
        Assert.IsFalse(onDamageCB);

        health.SetBarrier(false);
        health.TakeDamage(10);
        Assert.AreEqual(health.HP, 15);
        Assert.IsTrue(onDamageCB);
    }

    [Test]
    public void InvulnBehavior()
    {
        bool onHitCB = false;
        bool onDamageCB = false;

        Health health = CreateHealth();
        Assert.NotNull(health);

        health.onHit.AddListener((_) => onHitCB = true);
        health.onTakeDamage.AddListener((_) => onDamageCB = true);

        health.SetInvulnerable(true);
        health.TakeDamage(10);
        Assert.AreEqual(health.HP, 25);
        Assert.IsTrue(onHitCB);
        Assert.IsFalse(onDamageCB);

        health.SetInvulnerable(false);
        health.TakeDamage(10);
        Assert.AreEqual(health.HP, 15);
        Assert.IsTrue(onDamageCB);
    }

    [Test]
    public void HealingBehavior()
    {
        bool onHealCB = false;

        Health health = CreateHealth();
        Assert.NotNull(health);

        health.onHeal.AddListener((_) => onHealCB = true);

        health.TakeDamage(10);

        //healing works /w callback
        health.AddHealth(100);
        Assert.AreEqual(health.HP, 25);
        Assert.IsTrue(onHealCB);
    }

    [Test]
    public void DieBehavior()
    {
        bool onDeathCB = false;

        Health health = CreateHealth();
        Assert.NotNull(health);

        health.onDie.AddListener((_) => onDeathCB = true);

        //death callback
        health.TakeDamage(100);
        Assert.IsTrue(onDeathCB);
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    /*[UnityTest]
    public IEnumerator HealthTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }*/
}
