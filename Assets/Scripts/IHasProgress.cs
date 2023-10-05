using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress 
{
   
    public event EventHandler<onProgressChangedEventArgs> OnProgressChanged;
    public class onProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

}