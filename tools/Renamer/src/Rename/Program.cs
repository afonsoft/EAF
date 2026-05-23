// Decompiled with JetBrains decompiler
// Type: EAFRenamer.Program
// Assembly: EAFRenamer, Version=2.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: FB825B7B-CEB6-4801-BFE2-EC5935950AB3
// Assembly location: C:\AFONSOFT\EAF\Templates\Renamer.exe

using System;
using System.Windows.Forms;

namespace Eaf.Renamer.Win
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}