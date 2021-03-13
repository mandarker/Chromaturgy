﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class RedOrb : Orb
{
    public RedOrb()
    {
        OrbColor = Color.red;
        OrbShape = SpellShape.Jump;
        CooldownMod = 0.7f;
        ShapeManaMod = 0.8f;
        OrbElement = Element.Wrath;
        ModAmount = .1f;
        SpellEffectMod = 1.5f;
        UIPrefab = (GameObject)Resources.Load("Orbs/RedOrbUI");
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        int trueOrbAmount = orbAmount & 255;
        float launchX = (float)((orbAmount >> 24) & 127) * (((orbAmount >> 31) & 1) == 1 ? -1 : 1);
        float launchY = (float)((orbAmount >> 16) & 127) * (((orbAmount >> 23) & 1) == 1 ? -1 : 1);
        float launchZ = (float)((orbAmount >> 8) & 127) * (((orbAmount >> 15) & 1) == 1 ? -1 : 1);
        Vector3 launchVector = new Vector3(launchX, launchY, launchZ).normalized;

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, trueOrbAmount * 20f * spellEffectMod);

        // apply force is broken because it is using a rigidbody instead of NavMeshAgent.Warp or other methods
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyForce(launchVector + Vector3.up, 500, (trueOrbAmount == 1 ? 2f : (trueOrbAmount == 2 ? 2.5f : 3f)));
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
        Transform wizard = t.GetChild(0);

        Vector3 direction = clickedPosition - t.position;
        wizard.LookAt(wizard.position + new Vector3(direction.x, 0, direction.y).normalized);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Red Area"), t.position + Vector3.down, t.rotation) as GameObject;
        RedSpellController spellController = g.GetComponent<RedSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = SpellEffectMod;

        spellController.endPosition = clickedPosition + Vector3.up * 1.6f;
        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        RedOrb result = new RedOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        RedOrb c = (RedOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }
}
