﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class YellowOrb : Orb
{
    public YellowOrb()
    {
        m_OrbElement = Element.Light;
        m_OrbShape = SpellShape.OrbitingOrbs;
        m_UIPrefab = (GameObject) Resources.Load("Orbs/YellowOrbUI");
    }

    public override void AddHeldEffect(GameObject player)
    {
        StatusEffectScript status
    }

    public override void RevertHeldEffect(GameObject player)
    {

    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        float dmgMultiplier = 1;
        if (hit.GetComponent<StatusEffectScript>().StatusExists(StatusEffect.StatusType.SpellIncreasedDamage))
            dmgMultiplier += OrbValueManager.getGreaterEffectPercentile(Element.Water) / 100f;

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod * dmgMultiplier);

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStatus(StatusEffect.StatusType.AttackLessDamage, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level), 0, -OrbValueManager.getGreaterEffectPercentile(m_OrbElement));
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("Heal", RpcTarget.All, OrbValueManager.getLesserEffectValue(m_OrbElement, m_Level));
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Yellow Orbs"), t.position, t.rotation) as GameObject;
        YellowSpellController spellController = g.GetComponent<YellowSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);

        spellController.playerTransform = t;

        for (int i = 0; i < 3; i++)
        {
            if (i - m_Level >= 0)
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }
    }

    public static object Deserialize(byte[] data)
    {
        YellowOrb result = new YellowOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        YellowOrb o = (YellowOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
