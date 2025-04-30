using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public class LineObject
{
   public GameObject line;
   public LineRenderer lineRenderer ;
   public Collider2D metal;
   public float iValue {get; set;}

   public LineObject (GameObject Line, Collider2D Metal)
   {
      line = Line;
      metal = Metal;
      lineRenderer = Line.GetComponent<LineRenderer>();
      iValue = 0f;
   }


}
