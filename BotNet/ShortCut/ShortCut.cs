using System.Runtime.InteropServices;

namespace BotNet.ShortCut
{
    public static class ShortCut
    {
        public static void Create(
            string PathToFile, string PathToLink,
            string Arguments,string Iconpath,int IconNum)
        {
            ShellLink.IShellLinkW shlLink = ShellLink.CreateShellLink();
            Marshal.ThrowExceptionForHR(shlLink.SetPath(PathToFile));
            Marshal.ThrowExceptionForHR(shlLink.SetArguments(Arguments));
            Marshal.ThrowExceptionForHR(shlLink.SetIconLocation(Iconpath, IconNum));

            ((System.Runtime.InteropServices.ComTypes.IPersistFile)shlLink).Save(PathToLink, false);
        }
    }
}
