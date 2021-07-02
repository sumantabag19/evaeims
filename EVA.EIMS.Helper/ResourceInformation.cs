using System.Resources;

namespace EVA.EIMS.Helper
{
    public static class ResourceInformation
    {
        /// <summary>
        /// This method is used to get the resource string value by name
        /// </summary>
        /// <param name="name">name of the string</param>
        /// <returns></returns>
        public static string GetResValue(string name)
        {
            var rm = new ResourceManager("EVA.EIMS.Helper.StringMessages", System.Reflection.Assembly.GetExecutingAssembly());
            return rm.GetString(name);
        }
    }
}
