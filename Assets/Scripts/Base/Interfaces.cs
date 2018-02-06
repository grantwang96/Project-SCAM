using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SpellCaster
{
    void addToSeductionList(Damageable loser);
    void removeFromSeductionList(Damageable loser);
    void invokeChangeFollowers(Damageable target);
    void fireSpell();
    void pickUpSpell(SpellBook newSpell);
    void dropSpell(SpellBook dropSpell, Vector3 originPos);
    SpellBook returnSpell();
    Transform returnGun();
    Transform returnBody();
    Transform returnHead();
    bool canShoot();
    void setCanShoot(bool can);
}

public interface Interactable
{
    void Interact(SpellCaster spellCaster);
    void Interact();
}