using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registry_Change_Display
{
    internal class SharedTypes
    {
        internal Process process;

        //init is on first run
        internal string HKCU_Init_FilePath { get; }
        internal string HKLM_Init_FilePath { get; }
        internal string HKCU_Init_Command { get; }
        internal string HKLM_Init_Command { get; }

        // today is current information
        internal string HKCU_Today_FilePath { get; }
        internal string HKLM_Today_FilePath { get; }
        internal string HKCU_Today_Dump_Command { get; }
        internal string HKLM_Today_Dump_Command { get; }


        // The files selected in the loader
        internal string File1_Path { get; set; }
        internal string File2_Path { get; set; }


        // The text in the files to diff
        internal string[] File1_string_array { get; set; }
        internal string[] File2_string_array { get; set; }


        // changes is the diff between loaded and base
        internal string changes_FilePath { get; set; }

        internal IEnumerable<string> changes { get; set; }

        // diffCollection is the array of different entries
        internal List<string> diffCollection { get; set; }

        public SharedTypes()
        {
            // the base files from the first time the app is run on a fresh install
            HKCU_Init_FilePath = string.Format(@"{0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKLM_Init_FilePath = string.Format(@"{0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKCU_Init_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKLM_Init_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

            // the registry dump with todays date
            HKCU_Today_FilePath = string.Format(@"{0}\Current-HKCU-{1}.txt",
                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKLM_Today_FilePath = string.Format(@"{0}\Current-HKLM-{1}.txt",
                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKCU_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Current-HKCU-{1}.txt",
                                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKLM_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name >  {0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

        }
    }
}
