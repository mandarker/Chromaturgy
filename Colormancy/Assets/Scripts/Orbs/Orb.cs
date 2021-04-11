﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Orb
{
    [System.Serializable]
    public enum SpellShape
    {
        Jump, Fireball, OrbitingOrbs, Vines, Ink, Cloud, Shockwave, Bolt, ExpandingOrbs
    }

    [System.Serializable]
    public enum Element
    {
        Wrath, Fire, Light, Nature, Water, Poison, Earth, Wind, Darkness
    }

    public Color OrbColor;
    public SpellShape OrbShape;
    public Element OrbElement;
    public float CooldownMod;
    public float ShapeManaMod;
    public GameObject UIPrefab;
    protected float ModAmount;
    protected float SpellEffectMod;

    //SpellTest will just be the player controller
    public delegate void GreaterCast(GameObject hit, int orbAmount, float spellEffectMod);
    public delegate void LesserCast(GameObject hit, int orbAmount, float spellEffectMod);

    public abstract void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition);
    public abstract void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod);
    //Will have to do something different and send over server for this one since most are for allies
    public abstract void CastLesserEffect(GameObject hit, int orbAmount, float spellEffectMod);
    public abstract void RevertHeldEffect(SpellTest test);
    public abstract void AddHeldEffect(SpellTest test);
}