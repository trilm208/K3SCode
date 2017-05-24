
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Extensions
{
    public class SettingHelper
    {
       

        public static async Task SaveSetting(string SettingsKey, String SettingsValue)
        {
            Application.Current.Properties[SettingsKey] = SettingsValue;
            await Application.Current.SavePropertiesAsync();

        }
        public static string GetSetting(string SettingsKey,string DefaultValue)
        {
            try
            {
                object returnValue = Application.Current.Properties[SettingsKey];
                if (Application.Current.Properties[SettingsKey] == null)
                {
                    return DefaultValue;
                }
                else
                {
                    return returnValue.ToString();
                }
            }
            catch
            {
                return DefaultValue;
            }
           
        }


       
    }
}
