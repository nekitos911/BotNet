using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace Builder
{
    class Compil
    {
        public static bool Compiler(string source, string saveFileName, string outVersion, string resource, string icon = null)
        {
            CompilerParameters compil = new CompilerParameters();
            compil.GenerateExecutable = true;
            compil.OutputAssembly = saveFileName;

            string options = "/optimize+ /platform:x86 /target:winexe /unsafe";
            if (icon != null)
                options += " /win32icon:\"" + icon + "\"";

            compil.CompilerOptions = options;

            compil.ReferencedAssemblies.Add("System.dll");
            compil.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compil.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            compil.EmbeddedResources.Add(resource);

            Dictionary<string, string> ProviderOptions = new Dictionary<string, string>();
            ProviderOptions.Add("CompilerVersion", outVersion);
            CompilerResults Results = new CSharpCodeProvider(ProviderOptions).CompileAssemblyFromSource(compil, source);

            if (Results.Errors.Count > 0)
            {
                MessageBox.Show(string.Format("Имеются {0} ошибок",
                        Results.Errors.Count), "Ошибка компиляции", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                foreach (CompilerError Err in Results.Errors)
                {
                    MessageBox.Show(string.Format("{0}\nLine: {1} - Column: {2}\nFile: {3}", Err.ErrorText,
                            Err.Line, Err.Column, Err.FileName), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
