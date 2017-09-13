using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MonoSymbolicateHelper.Core
{
    public class ArchiveScanner
    {
        public string ArchivePath { get; }
        private IDictionary<string, ArchiveInfo> _archives;

        public ArchiveScanner(string archivePath)
        {
            ArchivePath = archivePath;
        }


        public bool Scan()
        {
            _archives = new Dictionary<string, ArchiveInfo>();
            foreach (var dateFolder in Directory.EnumerateDirectories(ArchivePath))
            {
                foreach (var projectFolder in Directory.EnumerateDirectories(dateFolder))
                {
                    var info = ReadArchiveInfo(projectFolder);
                    // check if existing archive for this key and only keep latest creation date
                    if (_archives.ContainsKey(info.Key))
                    {
                        var exist = _archives[info.Key];
                        if (exist.CreationDate > info.CreationDate)
                        {
                            continue;
                        }
                    }
                    _archives[info.Key] = info;
                }
            }

            return true;
        }

        public ArchiveInfo GetArchiveInfo(string packageName, string versionCode)
        {
            return _archives[ArchiveInfo.GetKey(packageName, versionCode)];
        }

        private ArchiveInfo ReadArchiveInfo(string projectFolder)
        {
            var filePath = Path.Combine(projectFolder, "archive.xml");
            var info = new ArchiveInfo {ArchivePath = projectFolder};

            using (var fs = File.OpenRead(filePath))
            using (var reader = XmlReader.Create(fs))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Name":
                                    info.Name = reader.ReadElementContentAsString();
                                    break;
                                case "PackageName":
                                    info.PackageName = reader.ReadElementContentAsString();
                                    break;
                                case "PackageVersionCode":
                                    info.PackageVersionCode = reader.ReadElementContentAsString();
                                    break;
                                case "PackageVersionName":
                                    info.PackageVersionName = reader.ReadElementContentAsString();
                                    break;
                                case "CreationDate":
                                    info.CreationDate = Convert.ToInt64(reader.ReadElementContentAsString());
                                    break;
                            }
                            break;
                    }
                }
            }
            return info;
        }
    }
}