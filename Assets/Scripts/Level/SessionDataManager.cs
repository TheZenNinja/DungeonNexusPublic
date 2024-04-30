using Player;
using Sirenix.OdinInspector;
using Skills;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SessionDataManager : MonoBehaviour
{
    public static SessionDataManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    public int money;
    public int floor;
    public PlayerEntity player;


    //public int curveFloorMax;
    //public AnimationCurve expCurve;
    //public int expCurveMax;
    //public AnimationCurve aggressiveEnemyCurve;
    //public int aggressiveEnemyCurveMax;
    //public AnimationCurve supportEnemyCurve;
    //public int supportEnemyCurveMax;

    [Space]

    public SkillOrb skillOrbPrefab;
    public SkillOrb[] rewards;
    public Transform[] rewardSpawnPos;
    public SkillRewardDefinition possibleRewards;

    [Space]

    public TextMeshPro floorTxt;
    public GameObject gameoverScreen;
    public TextMeshProUGUI highscoreTxt;

    // public int GetScaledCurveValue(int floor, AnimationCurve curve, int maxVal)
    // {
    //     float floorPercent = (float)floor / curveFloorMax;
    //     return Mathf.RoundToInt(curve.Evaluate(floorPercent) * maxVal);
    // }
    // public int GetExpAmt(int floor) => GetScaledCurveValue(floor, expCurve, expCurveMax);
    public int GetAggrEnemyCount(int floor) //=> GetScaledCurveValue(floor, aggressiveEnemyCurve, aggressiveEnemyCurveMax);
    {
        return Mathf.FloorToInt(Mathf.Sqrt(3 * floor) / 2 + 1);
    }
    public int GetSupportEnemyCount(int floor)// => GetScaledCurveValue(floor, supportEnemyCurve, supportEnemyCurveMax);
    {
        return Mathf.FloorToInt(Mathf.Sqrt(3 * floor / 4 - 2));
    }
    public int GetEnemyHealth(int floor)
    {
        //equation i mapped out in desmos
        const int baseHP = 40;
        const int equationSplit = 20;
        const float coeff = .4f;//.3f;

        float output;

        if (floor > equationSplit)
            output = (floor * coeff) + Mathf.Pow(equationSplit * coeff, 2) - (equationSplit * coeff) + baseHP;
        else
            output = Mathf.Pow(floor * coeff, 2) + baseHP;

        return Mathf.RoundToInt(output);
    }

    public int GetEnemyDamage(int floor)
    {
        return Mathf.RoundToInt((floor - 1) / 3 + 5);
    }
    public int GetNumOfWaves(int floor)
    {
        return Mathf.FloorToInt(floor / 3 + 3);
    }

    [Button]
    void DebugWaves()
    {
        foreach (var wave in GetAggressiveWaves())
        {
            print(wave);
        }
    }

    public int[] GetAggressiveWaves()
    {
        var waves = new int[GetNumOfWaves(floor)];

        for (int i = 0; i < waves.Length; i++)
        {
            float numOfEnemies = GetAggrEnemyCount(floor) * GetWaveMulti(i);
            waves[i] = Mathf.Clamp(Mathf.RoundToInt(numOfEnemies), 1, 32);
        }

        return waves;
    }
    public int[] GetSupportWaves()
    {
        var waves = new int[GetNumOfWaves(floor)];

        for (int i = 0; i < waves.Length; i++)
        {
            float numOfEnemies = GetSupportEnemyCount(floor) * GetWaveMulti(i);
            waves[i] = Mathf.Clamp(Mathf.RoundToInt(numOfEnemies), 0, 32);
        }

        return waves;
    }
    public float GetWaveMulti(int floor)
    {
        if (floor == 0)
            return .5f;
        if (floor == 1)
            return 1;
        if (floor == 2)
            return 2;

        return Mathf.Sqrt(2 * floor) / 3 + 2;
    }

    public int GetBossHP()
    {
        float scalingHP = 100;
        int baseHP = 500;

        float hp = Mathf.Sqrt(scalingHP * floor - 10 * scalingHP) + baseHP;
        return Mathf.RoundToInt(hp);
    }

    public bool IsBossFloor()
    {
        if (floor <= 0)
            return false;

        if (floor % 10 == 0)
            return true;

        return false;
    }


    private void Start()
    {
        LoadRewards();
    }

    public void LoadRewards(int rarityOffset = 0)
    {
        floorTxt.text = "Enter Floor " + floor;

        ClearRewards();

        var rewardsToSpawn = new SkillScriptableObject[3];
        rewards = new SkillOrb[3];
        for (int i = 0; i < 3; i++)
        {
            var rarity = Random.Range(1, 11) + rarityOffset;
            if (rarity >= 9)
                rewardsToSpawn[i] = possibleRewards.GetRareReward();
            else if (rarity >= 6)
                rewardsToSpawn[i] = possibleRewards.GetUncommonReward();
            else
                rewardsToSpawn[i] = possibleRewards.GetCommonReward();
        }

        for (int i = 0; i < 3; i++)
        {
            var orb = Instantiate(skillOrbPrefab, rewardSpawnPos[i]);
            orb.SetSkill(rewardsToSpawn[i]);
            rewards[i] = orb;
            orb.onConsume += ClearRewards;
        }
    }
    public void ClearRewards()
    {
        if (rewards.Length > 0)
            for (int i = 0; i < rewards.Length; i++)
                if (rewards[i] != null)
                    Destroy(rewards[i].gameObject);
    }
    public void IncrFloor()
    {
        HighscoreManager.SaveHighscore(floor);
        floor++;
        LoadRewards(Mathf.FloorToInt(floor / 3f));

    }

    public void PlayerDeath()
    {
        player.GetComponent<PlayerInput>().enabled = false; 
        HighscoreManager.SaveHighscore(floor);
        gameoverScreen.SetActive(true);
        highscoreTxt.text = "Floor reached: " + floor;
        FindObjectOfType<PlayerCamera>().LockControls(false);
    }
}