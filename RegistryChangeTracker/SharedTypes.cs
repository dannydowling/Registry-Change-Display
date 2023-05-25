using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace RegistryChangeTracker
{
    internal class SharedTypes
    {
        internal Process process;

        //init is on first run
        internal string Init_FilePath { get; }
        internal string HKCU_Init_Command { get; }
        internal string HKLM_Init_Command { get; }

        // today is current information

        internal Tuple<string[], string[]> Today_Registry_Contents { get; set; }
        internal string Today_Registry_Image_FilePath { get; }        
        internal string HKCU_Today_Dump_Command { get; }
        internal string HKLM_Today_Dump_Command { get; }


        // The files selected in the loader
        internal List<string> OpenFilePaths { get; set; }
       

        // The text in the files to diff
        internal Tuple<string, string> Unformatted_Powershell_Output_Registry_image_base { get; set; }
        internal Tuple<string, string> Unformatted_Powershell_Output_Registry_image_comparer { get; set; }




        public SharedTypes()
        {
            // the base files from the first time the app is run on a fresh install
            Init_FilePath = string.Format(@"{0}\Base.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKCU_Init_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKLM_Init_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

            // the registry dump with todays date
            Today_Registry_Image_FilePath = string.Format(@"{0}\Registry-Image-{1}.txt",
                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKCU_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Current-HKCU-{1}.txt",
                                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKLM_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name >  {0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

        }
    }
}
