using UnityEngine;

namespace Assets.Scripts.Demo
{
   public class ExampleAlertsController : MonoBehaviour
   {
      //Events for buttons
      public void DangerAlert()
      {
         WorkSpaceSettings.Instance.Alert("danger", "Danger alert", "This action will destroy all files! Proceed?", DangerYes, DangerNo);
      }
   
      public void WarningAlert()
      {
         WorkSpaceSettings.Instance.Alert("warning", "Warning alert", "All recent actions canceled due to incompatibility of your PC drivers!");
      }
   
      public void InfoAlert()
      {
         WorkSpaceSettings.Instance.Alert("info", "Info alert", "To access the data, you need to hack the server!");
      }
   
      public void QuestionAlert()
      {
         WorkSpaceSettings.Instance.Alert("question", "Question alert", "Do you like this asset?", QuestionYes, QuestionNo);
      }
      
      //answer options for danger alert
      private void DangerYes()
      {
         WorkSpaceSettings.Instance.Alert("question", "Question alert", "Are you reporting to yourself?", DangerDefinitivelyYes, DangerNo);
      }
      
      private void DangerDefinitivelyYes()
      {
         WorkSpaceSettings.Instance.Alert("warning", "Congratulations!", "All data is destroyed irretrievably!");
      }
      
      private void DangerNo()
      {
         WorkSpaceSettings.Instance.Alert("info", "Congratulations!", "You are on the right track!");
      }
      
      //answer options for question alert
      private void QuestionYes()
      {
         WorkSpaceSettings.Instance.Alert("info", "Info alert", "Thanks for the feedback! Hope you leave a review!");
      }
      
      private void QuestionNo()
      {
         WorkSpaceSettings.Instance.Alert("info", "Info alert", "Let me know what you don't like! Perhaps I can help you! denis@proger.xyz");
      }
   }
}
