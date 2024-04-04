using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetObject
{
    int BeAttacked(int value);
    int BeHealed(int value);
    void BeMoved(Vector2 pos, FaceDirection rotation);
    void BeTarget();

    bool IsPlayerCharacter();

    FaceDirection GetFaceDirection();

    CharacterData GetCharacterData();
}
