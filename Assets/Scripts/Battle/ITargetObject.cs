using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetObject
{
    void BeAttacked(int value);
    void BeDefenced(int value);
    void BeMoved(Vector2 pos, FaceDirection rotation);
    void BeTarget();
}
