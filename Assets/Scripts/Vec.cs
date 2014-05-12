using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public class Vec
    {
        public static Vector2 xy(Vector3 v) 
        {
            return new Vector2(v.x, v.y);
        }
        public static Vector2 yz(Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }
        public static Vector2 xz(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static Vector2 yx(Vector3 v)
        {
            return new Vector2(v.y, v.x);
        }
        public static Vector2 zy(Vector3 v)
        {
            return new Vector2(v.z, v.y);
        }
        public static Vector2 zx(Vector3 v)
        {
            return new Vector2(v.z, v.x);
        }
        public static Vector2 PerpendicularCCW(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }
        public static Vector2 PerpendicularCW(Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }
    }

