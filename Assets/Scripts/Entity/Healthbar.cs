using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Healthbar : MonoBehaviour
{
    [TabGroup("References")]
    public RectTransform healthRect;
    [TabGroup("References")]
    public RectTransform armorRect;
    [TabGroup("References")]
    public RectTransform parentRect;
    [TabGroup("References")]
    public GameObject barrierOverlay;

    [TabGroup("Debug Values")]
    [SerializeField] int debugHP;
    [TabGroup("Debug Values")]
    [SerializeField] int debugMaxHP;
    [TabGroup("Debug Values")]
    [SerializeField] int debugArmor;
    [TabGroup("Debug Values")]
    [SerializeField] bool debugBarrier;

    public void UpdateHealthbar(Health hp)
    {
        /*int armor = hp.Armor;
        int health = hp.HP;
        int maxHealth = hp.GetMaxHP;

        float newBarWidth = ClassExtensions.MapNumber(maxHealth, minBarThresh, maxBarThresh, minBarSize, maxBarSize);
        parentRect.sizeDelta = new Vector2(newBarWidth, parentRect.sizeDelta.y);

        float totalWidth = GetComponent<RectTransform>().sizeDelta.x;
        bool hasArmor = armor > 0;
        bool barWillOverflow = armor + health > maxHealth;

        if (barWillOverflow)
            UpdateBar(health + armor);
        else
            UpdateBar(maxHealth);

        void UpdateBar(int maxHP)
        {
            float hpWidth = totalWidth * ((float)health / maxHP);
            healthRect.sizeDelta = new Vector2(hpWidth, healthRect.sizeDelta.y);

            armorRect.gameObject.SetActive(hasArmor);
            if (hasArmor)
            {
                float armorWidth = totalWidth * ((float)armor / maxHP);
                armorRect.sizeDelta = new Vector2(armorWidth, armorRect.sizeDelta.y);
                armorRect.anchoredPosition = Vector2.right * hpWidth;
            }
        }

        barrierOverlay.SetActive(hp.HasBarrier);*/
        UpdateHealthbar(hp.HP, hp.GetMaxHP, hp.Armor, hp.HasBarrier);
    }
    public void UpdateHealthbar(int health, int maxHP, int armor, bool hasBarrier)
    {
        float totalWidth = GetComponent<RectTransform>().sizeDelta.x;
        bool hasArmor = armor > 0;
        bool barWillOverflow = armor + health > maxHP;

        if (barWillOverflow)
            UpdateBar(health, health + armor, totalWidth, armor, hasArmor);
        else
            UpdateBar(health, maxHP, totalWidth, armor, hasArmor);

        

        barrierOverlay.SetActive(hasBarrier);
    }
    void UpdateBar(int health, int maxHP, float totalWidth, int armor, bool hasArmor)
    {
        float hpWidth = totalWidth * ((float)health / maxHP);
        healthRect.sizeDelta = new Vector2(hpWidth, healthRect.sizeDelta.y);

        armorRect.gameObject.SetActive(hasArmor);
        if (hasArmor)
        {
            float armorWidth = totalWidth * ((float)armor / maxHP);
            armorRect.sizeDelta = new Vector2(armorWidth, armorRect.sizeDelta.y);
            armorRect.anchoredPosition = Vector2.right * hpWidth;
        }
    }

    [Button]
    void UpdatePreview() => UpdateHealthbar(debugHP, debugMaxHP, debugArmor, debugBarrier);
}
