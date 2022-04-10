using System;
using Assets.Scripts.Window;
using UnityEngine.Events;

namespace Assets.Scripts.Alert
{
  public class AlertActions : WindowActions
  {

    public UnityAction ReturnedButton(Action returnedNegative = null)
    {
      void Ection()
      {
        returnedNegative?.Invoke();
        CloseWindow();
      }
      
      return Ection;
    }
  }
}
