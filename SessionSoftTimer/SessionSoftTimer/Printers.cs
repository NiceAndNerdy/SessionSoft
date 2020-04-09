using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionSoftTimer
{
    public enum SelectedPrinter
    {
        Envisionware = 1,
        CirculationDesk = 2
    }

    public class PrinterScripts
    {
        public static string AddCirculationPrinter = "/C RUNDLL32 printui.dll,PrintUIEntry /if /b \"Backup Printer\" /f \"C:\\Canon\\Generic Plus UFRII v1.21 x64\\Driver\\CNLB0MA64.INF\" /r \"IP_10.1.32.201\" /m \"Canon Generic Plus UFR II\"";
        public static string AddCirculationPrinterx86 = "/C RUNDLL32 printui.dll,PrintUIEntry /if /b \"Backup Printer\" /f \"C:\\Canon\\Generic Plus UFRII v1.21\\Driver\\CNLB0M.INF\" /r \"IP_10.1.32.201\" /m \"Canon Generic Plus UFR II\"";
        public static string MakeCirculationDefault = "/C RUNDLL32 PRINTUI.DLL,PrintUIEntry /y /n \"Backup Printer\"";
        public static string MakeEnvisionWareDefault = "/C RUNDLL32 PRINTUI.DLL,PrintUIEntry /y /n \"Black and White\"";
    }

}
