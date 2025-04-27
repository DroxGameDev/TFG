using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct LineObject
{
   public GameObject Line;
   public LineRenderer lineRenderer ;
   public float iValue;

   public LineObject (GameObject line)
   {
      Line = line;
      lineRenderer = Line.GetComponent<LineRenderer>();
      iValue = 0f;
   }
   
}
