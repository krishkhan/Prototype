
namespace BizApplication
{

    /// <summary>
    /// Used to parse the application-xml file when service startup.
    /// </summary>
    public class ApplicationRecordInfo
    {
        //### The unique name of the application
        public string Name { get; set; }
        //### The file where the application is saved
        public string AssemblyPath { get; set; }
        //### The fullname of the applicationConfiguration class
        public string ApplicationSettingsClass { get; set; }
        //### The fullname of the application class
        public string ApplicationClass { get; set; }
    }
}
