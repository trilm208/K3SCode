using System;
using Acr.UserDialogs;
namespace Extensions
{
    public static class UI
    {
        public static void ShowError(string message)
        {
            IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
            Dialogs.ShowError(message, 2000);
        }
        public static void ShowError(string message,int timeoutMillis)
        {
        	IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
        	Dialogs.ShowError(message,timeoutMillis );
        }

        public static void ShowSuccess(string message,int timeoutMillis)
        {
            IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
            Dialogs.ShowSuccess(message, timeoutMillis);
        }

        public static void ShowSuccess(string message)
        {
        	IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
        	Dialogs.ShowSuccess(message, 2000);
        }
      
   }
}
