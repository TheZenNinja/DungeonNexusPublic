using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Health : MonoBehaviour
{
    [TabGroup("Values")]
    [SerializeField]
    private int maxHP;
    public int GetMaxHP => maxHP;
    [TabGroup("Values")]
    [SerializeField]
    private int _hp;
    public int HP => _hp;
    [TabGroup("Values")]
    [SerializeField]
    private int _armor;
    public int Armor => _armor;
    [TabGroup("Values")]
    [SerializeField]
    private bool _hasBarrier;
    public bool HasBarrier => _hasBarrier;
    [TabGroup("Values")]
    [SerializeField]
    private bool _isInvulnerable;
    public bool IsInvulnerable => IsInvulnerable;

    public float hpPercent => (int)HP / maxHP;

    [TabGroup("References")]
    public Healthbar healthbar;

    // UnityActions dont show currently, so disabling them

    [TabGroup("Events")]
    public UnityEvent<Health> onHit;

    [TabGroup("Events")]
    public UnityEvent<Health> onTakeDamage;

    [TabGroup("Events")]
    public UnityEvent<Health> onTakeDamageToHealth;

    [TabGroup("Events")]
    public UnityEvent<Health> onDie;

    [TabGroup("Events")]
    public UnityEvent<Health> onHeal;

    //add events for hit while invul and hit while barrier-ed? could always just determine it in the func where its called

    void Start() => Initialize();    

    public void Initialize()
    {
        _hp = maxHP;
        UpdateUI();
    }

    public void UpdateUI()
    {
        //if (healthbar != null)
        healthbar?.UpdateHealthbar(this);
    }

    public void AddArmor(int amount)
    {
        _armor += amount;

        UpdateUI();
    }
    public void AddArmor(int amount, int maxArmor)
    {
        //if existing armor is bigger than what we're trying to add
        if (_armor >= maxArmor)
            return;
        //if existing armor is less than max, but adding to it goes over
        else if (_armor + amount >= maxArmor)
            _armor = amount;
        //existing + new armor doesnt go over max
        else
            _armor += amount;

        UpdateUI();
    }

    public void AddHealth(int amount)
    {
        _hp += amount;
        
        if (_hp > maxHP)
            _hp = maxHP;

        onHeal?.Invoke(this);

        UpdateUI();
    }
    public void TakeDamage(int baseDmg)
    {
        onHit?.Invoke(this);
        //onHitU?.Invoke(this);

        if (_isInvulnerable || _hasBarrier)
            baseDmg = 0;

        if (baseDmg <= 0)
            return;

        int totalDmg = baseDmg;

        //account for armor
        if (_armor > 0)
        {
            totalDmg -= _armor;
            _armor -= baseDmg;
            if (_armor < 0)
                _armor = 0;
        }

        UpdateUI();

        onTakeDamage?.Invoke(this);
        //onTakeDamageU?.Invoke(this);

        // if armor absorbed all dmg
        if (totalDmg <= 0)
            return;

        _hp -= totalDmg;

        UpdateUI();

        onTakeDamageToHealth?.Invoke(this);
        //onTakeDamageToHealthU?.Invoke(this);

        if (_hp <= 0)
        {
            onDie?.Invoke(this);
            //onDieU?.Invoke(this);
        }
    }

    public void InstantKill()
    {
        _hp = 0;
        onDie?.Invoke(this);
    }

    public void SetBarrier(bool hasBarrier) => _hasBarrier = hasBarrier;
    public void SetInvulnerable(bool isInvuln) => _isInvulnerable = isInvuln;

    public void SetMaxHP(int amt, bool resetHP = false)
    {
        maxHP = amt;
        if (_hp > maxHP)
            _hp = maxHP;

        if (resetHP)
            _hp = maxHP;
    }

}
