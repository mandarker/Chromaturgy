﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;
    private const Orb.Element element = Orb.Element.Poison;

    [Space]

    private float starttime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private bool debug;

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.position + Vector3.down, OrbValueManager.getPaintRadius(element));
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - starttime > lifetime && !debug)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
            greaterCast(collider.gameObject, spellEffectMod, null);
        else if (collider.gameObject.CompareTag("Player"))
            lesserCast(collider.gameObject, spellEffectMod, null);
    }
}
