using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;

namespace Rca.Physical.If97.SeuIf97
{
    /// <summary>
    /// <para>This class can extract and load DLLs from embedded binary resources.<br/>
    /// This can be used with pinvoke, as well as manually loading DLLs your own way. If you use pinvoke, you don't need to load the DLLs, just
    /// extract them. When the DLLs are extracted, the %PATH% environment variable is updated to point to the temporary folder.</para>
    /// <para>How to use</para>
    /// <list type="number">
    /// <item>Add all of the DLLs as binary file resources to the project propeties. Double click Properties/Resources.resx,
    /// Add Resource, Add Existing File. The resource name will be similar but not exactly the same as the DLL file name.</item>
    /// <item>In a static constructor of your application, call <c>EmbeddedDllClass.ExtractEmbeddedDlls()</c> for each DLL that is needed.
    /// <code>EmbeddedDllClass.ExtractEmbeddedDlls("libFrontPanel-pinv.dll", Properties.Resources.libFrontPanel_pinv);</code></item>
    /// <item>Optional: In a static constructor of your application, call EmbeddedDllClass.LoadDll() to load the DLLs you have extracted. This is not necessary for pinvoke.
    /// <code>EmbeddedDllClass.LoadDll("myscrewball.dll");</code></item>
    /// <item>Continue using standard Pinvoke methods for the desired functions in the DLL.</item>
    /// </list>
    /// Source: Mark Lakata (https://stackoverflow.com/a/11038376)
    /// </summary>
    public class DllHelper
    {
        private static string m_TempFolder = string.Empty;

        /// <summary>
        /// Extract DLLs from resources to temporary folder
        /// </summary>
        /// <param name="dllName">Name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceBytes">The resource name (fully qualified)</param>
        public static void ExtractEmbeddedDlls(string dllName, byte[] resourceBytes)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();

            // The temporary folder holds one or more of the temporary DLLs
            // It is made "unique" to avoid different versions of the DLL or architectures.
            m_TempFolder = string.Format("{0}.{1}.{2}", assemblyName.Name, Environment.Is64BitProcess ? "x64" : "x86", assemblyName.Version);

            var dirName = Path.Combine(Path.GetTempPath(), m_TempFolder);
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            // Add the temporary dirName to the PATH environment variable (at the head!)
            var path = Environment.GetEnvironmentVariable("PATH");

            if (path is not null)
                if (!path.Split(';').Any(p => p == dirName))
                    Environment.SetEnvironmentVariable("PATH", dirName + ";" + path);
            else
                Environment.SetEnvironmentVariable("PATH", dirName);

            // See if the file exists, avoid rewriting it if not necessary
            var dllPath = Path.Combine(dirName, dllName);

            if (!(File.Exists(dllPath) && resourceBytes.SequenceEqual(File.ReadAllBytes(dllPath))))
                File.WriteAllBytes(dllPath, resourceBytes);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Managed wrapper around LoadLibrary
        /// </summary>
        /// <param name="dllName"></param>
        static public void LoadDll(string dllName)
        {
            if (m_TempFolder == "")
                throw new Exception("Please call ExtractEmbeddedDlls before LoadDll");

            if (LoadLibrary(dllName) == IntPtr.Zero)
                throw new DllNotFoundException("Unable to load library: " + dllName + " from " + m_TempFolder, new Win32Exception());
        }

    }
}