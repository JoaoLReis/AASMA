using UnityEngine;
using System.Collections;

public abstract class PerceptionInterface : MonoBehaviour, ReactiveInterface
{
    public abstract bool fireSensor(GameObject obj);

    public abstract void setCollided(bool v);

    public abstract bool getCollided();

    public abstract void setReadyToMove(bool v);

    public abstract bool getReadyToMove();

    public abstract void recalculate();

    public abstract void recalculateRight();

    public abstract bool refillWater(Vector3 vector3);

    public abstract void OnTriggerEnter(Collider col);

    public abstract void OnTriggerStay(Collider col);

    public abstract void OnTriggerExit(Collider col);

    public abstract void isNightTime(bool night);

    public abstract void setResting(bool resting);

    public abstract void reset();

    public abstract int numFiresPutOut();

    public abstract void refillPurificant();

    public abstract void updateNodes(Vector3 pos);
}
