using System.Collections.Generic;
[System.Serializable]

public class SpawnableObjectByLevel<T>
{
    public DungeonLevelSO dungeonLevel;
    public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
}
