using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ClassExtensions
{
    public static Vector3 ToV3(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);

    public static Vector3 LerpXY(this Vector3 from, Vector3 to, float amt)
    {
        float x = Mathf.Lerp(from.x, to.x, amt);
        float z = Mathf.Lerp(from.z, to.z, amt);

        return new Vector3(x, from.y, z);
    }
    public static Vector3 Limit(this Vector3 vector, float x, float y, float z) => Limit(vector, new Vector3(x,y,z));
    public static Vector3 Limit(this Vector3 vector, Vector3 limit)
    {
        var x = Mathf.Clamp(vector.x, -limit.x, limit.x);
        var y = Mathf.Clamp(vector.y, -limit.y, limit.y);
        var z = Mathf.Clamp(vector.z, -limit.z, limit.z);

        return new Vector3(x,y,z);
    }
    public static Vector3 ZeroY(this Vector3 v)
    {
        v.y = 0;
        return v;
    }

    public static float MapNumber(float value, float inStart, float inEnd, float outStart, float outEnd)
    {
        //X between A and B to range C and D
        // y = (x-a)/(b-a)*(d-c)+c
        return (value - inStart) / (outEnd - outStart) * (inEnd - inStart) + outStart;
    }
    public static float MapTo0_1(float value, float min, float max)
    {
        return 0;
    }
    public static float MapRange(float value, float outMin, float outMax)
    {
        return value / (outMax - outMin) * outMax + outMin;
    }

    public static bool LayerIsInMask(int layer, LayerMask mask)
    {
        return (mask == (mask | (1 << layer))); 
    }

    // look at this idiot
    //public static Vector3 ScaledNormalize(this Vector3 vector)
    //{
    //    float scale = vector.magnitude;
    //    Vector3 dir = vector.normalized;
    //    return dir * scale;
    //}
    

    public static List<T> GetRandomListFromPool<T>(this IEnumerable<T> pool, int count, bool allowDuplicates = true)
    {
        var output = new List<T>();

        if (allowDuplicates)
        {
            for (int i = 0; i < count; i++)
            output.Add(pool.GetRandomItem());
        }
        else
        {
            for (int i = 0; i < count; i++)
                for (int r = 0; r < 100; r++)
                {
                    var item = pool.GetRandomItem();
                    if (!output.Contains(item))
                        output.Add(pool.GetRandomItem());
                    
                    break;
                }
        }

        return output;
    }

    public static T GetRandomItem<T>(this IEnumerable<T> pool) => pool.ElementAt(Random.Range(0, pool.Count() - 1));
    public static T GetRandomItem<T>(this IEnumerable<T> pool, int seed)
    {
        var rnd = new System.Random(seed);
        return pool.ElementAt(rnd.Next(0, pool.Count()));
    }
}