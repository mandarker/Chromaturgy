﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OrbInfo
{
    public Color color;
    public int spellShape;
    public int spellElement;
    public float cooldownMod;
    public float manaMod;
}

public class SpellTest : MonoBehaviourPun
{
    List<Orb> orbs = new List<Orb>();
    SpellManager manager;
    ManaScript mana;

    Dictionary<(Orb, Orb, Orb), float> spellCooldowns = new Dictionary<(Orb, Orb, Orb), float>();

    SpellManager.Spell currentSpell;

    #region Dummy Player Attributes

    static readonly float BASE_ATTACK_SPEED = 1f;
    static readonly float BASE_HEALTH_REGEN = 1f;

    float attackSpeed = BASE_ATTACK_SPEED;
    float healthRegen = BASE_HEALTH_REGEN;

    float _attackSpeedMod = 1f;
    public float AttackSpeedMod
    {
        get => _attackSpeedMod;
        set
        {
            _attackSpeedMod = value;
            attackSpeed = BASE_ATTACK_SPEED * _attackSpeedMod;
            print("Current attack speed: " + attackSpeed);
        }
    }

    float _healthRegenMod = 1f;
    public float HealthRegenMod
    {
        get => _healthRegenMod;
        set
        {
            _healthRegenMod = value;
            healthRegen = BASE_HEALTH_REGEN * _healthRegenMod;
            print("Current health regen: " + healthRegen);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SpellManager>();
        mana = GetComponent<ManaScript>();
        orbs.Add(new IndigoOrb());
        orbs.Add(new YellowOrb());
        orbs.Add(new VioletOrb());
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
            GetSpellInput();
    }

    void GetSpellInput()
    {
        var input = Input.inputString;
        if (!string.IsNullOrEmpty(input))
        {
            if (int.TryParse(input, out int number))
            {
                number = number - 1; //used for indexing now
                if (number < orbs.Count && number >= 0)
                {
                    photonView.RPC("AddOrb", RpcTarget.All, orbs[number]);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("TryCastSpell", RpcTarget.AllViaServer);
        }
    }
    
    [PunRPC]
    void AddOrb(Orb orb)
    {
        currentSpell = manager.AddOrb(orb);
    }

    [PunRPC]
    void TryCastSpell()
    {
        if (mana.GetEffectiveMana() >= currentSpell.GetManaCost() && !currentSpell.Equals(new SpellManager.Spell()))
        {
            if (spellCooldowns.ContainsKey(currentSpell.GetOrbTuple()))
            {
                if (spellCooldowns[currentSpell.GetOrbTuple()] <= Time.time)
                {
                    CastSpell();
                }
            }
            else
            {
                CastSpell();
            }
        }
    }

    [PunRPC]
    void ClearSpell()
    {
        currentSpell = new SpellManager.Spell();
    }

    void CastSpell()
    {
        currentSpell.Cast(transform);
        spellCooldowns[currentSpell.GetOrbTuple()] = Time.time + currentSpell.GetSpellCooldown();
        mana.ConsumeMana(currentSpell.GetManaCost());
        manager.ClearOrbs();
        photonView.RPC("ClearSpell", RpcTarget.All);
    }
}
