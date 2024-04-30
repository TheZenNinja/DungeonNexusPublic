using UnityEngine;

public static class MyUtils
{
    public static bool LayerIsInLayermask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}