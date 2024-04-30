using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class VectorExtensions
{
    public static float DistanceXZ(Vector3 v1, Vector3 v2)
    {
        v1.y = 0;
        v2.y = 0;

        return Vector3.Distance(v1, v2);
    }
}