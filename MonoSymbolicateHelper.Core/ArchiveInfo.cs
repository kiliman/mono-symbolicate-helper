namespace MonoSymbolicateHelper.Core
{
    public class ArchiveInfo
    {
        public string Name { get; set; }
        public string PackageName { get; set; }
        public string PackageVersionCode { get; set; }
        public string PackageVersionName { get; set; }

        public string ArchivePath { get; set; }
        public string mSymPath => $@"{ArchivePath}\mSYM\{PackageName}.apk.mSYM";
        public string Key => GetKey(PackageName, PackageVersionCode);

        public static string GetKey(string packageName, string versionCode)
        {
            return $"{packageName}-{versionCode}";
        }
    }
}
