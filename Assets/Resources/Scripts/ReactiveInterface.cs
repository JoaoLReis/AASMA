using UnityEngine;
using System.Collections;

public interface ReactiveInterface {

    void setCollided(bool v);

    bool getCollided();

    void setReadyToMove(bool v);

    bool getReadyToMove();

    void recalculate();

    void recalculateRight();
}
