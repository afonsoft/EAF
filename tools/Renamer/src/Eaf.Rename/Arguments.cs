using System;

namespace Eaf.Renamer.Lib
{
    public class Arguments : EventArgs
    {
        public readonly string Filter = ".cs,.cshtml,.js,.ts,.csproj,.sln,.xml,.config,.DotSettings,.json,.xaml,.txt,.html,.gitignore,.ps1,.md,.plist,.tpl";

        public string OldCompanyName { get; set; } = "Eaf.ProjectName";

        public string OldProjectName { get; set; } = "eaf-ProjectName-ui";

        public string NewCompanyName { get; set; } = "Eaf.ProjectName";

        public string NewProjectName { get; set; } = "eaf-ProjectName-ui";

        public string OldAreaName { get; set; } = "AppAreaName";

        public string NewAreaName { get; set; } = "App";

        public string RootDir { get; set; } = "";
        public bool ChangeAreaName { get; set; } = true;
    }
}