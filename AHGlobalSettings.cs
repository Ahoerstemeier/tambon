using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    internal class AHGlobalSettings
    {
        public static String HTMLCacheDir { get; set; }
        public static String XMLOutputDir { get; set; }
        public static String PDFDir { get; set; }
        public static void LoadSettings()
        {
            try
            {
                HTMLCacheDir = (String)Application.UserAppDataRegistry.GetValue("HTMLCache");
            }
            catch (Exception)
            {
            }
            if (String.IsNullOrEmpty(HTMLCacheDir))
            {
                HTMLCacheDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\cache\\";
            }
            try
            {
                XMLOutputDir = (String)Application.UserAppDataRegistry.GetValue("XMLOutput");
            }
            catch (Exception)
            {
            }
            if (String.IsNullOrEmpty(XMLOutputDir))
            {
                XMLOutputDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\XMLout\\";
            }
            try
            {
                PDFDir = (String)Application.UserAppDataRegistry.GetValue("PDFStorage");
            }
            catch (Exception)
            {
            }
            if (String.IsNullOrEmpty(PDFDir))
            {
                PDFDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\PDF\\";
            }
        }
        public static void SaveSettings()
        {
            Application.UserAppDataRegistry.SetValue("HTMLCache", HTMLCacheDir);
            Application.UserAppDataRegistry.SetValue("XMLOutput", XMLOutputDir);
            Application.UserAppDataRegistry.SetValue("PDFStorage", PDFDir);
        }
    }
}
