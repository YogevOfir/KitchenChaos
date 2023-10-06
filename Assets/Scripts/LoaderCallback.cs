using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
   

   private bool isfirstUpdate = true;

   private void Update() {
       if (isfirstUpdate) {
           isfirstUpdate = false;
           
           Loader.LoaderCallback();
       }
   }
}
