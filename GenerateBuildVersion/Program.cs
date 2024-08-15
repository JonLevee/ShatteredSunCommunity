// See https://aka.ms/new-console-template for more information
// this will be called by ShatteredSunCommunity.csproj build
using GenerateBuildVersion;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = new CSSVersionInfoBuilder();
builder.UpdateVersionFile();

namespace GenerateBuildVersion
{
    public class CSSFileVersionInfo
    {
        public string FileHashCode { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;
    }

    public class CSSVersionInfo
    {
        public void UpdateCSSFilesHashCode()
        {
            var sb = new StringBuilder();
            Files.ForEach(f => sb.AppendLine(f.FileHashCode));
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(sb.ToString());
                writer.Flush();
                stream.Position = 0;
                CSSFilesHashCode = GetStreamHash(stream);
            }
        }
        public string CSSFilesHashCode { get; set; } = string.Empty;
        public List<CSSFileVersionInfo> Files { get; set; } = [];

        private static string GetStreamHash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                byte[] checksum = md5.ComputeHash(stream);
                var output = BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                return output;
            }
        }

        public static string GetFileHash(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return GetStreamHash(stream);
            }
        }
    }

    public class ProductVersionInfo
    {
        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 0;
        public int Build { get; set; } = 0;
        public int CSSBuild { get; set; } = 0;

        public CSSVersionInfo VersionInfo { get; set; } = new CSSVersionInfo();

        [JsonIgnore]
        public string BuildVersion => $"{Major}.${Minor}.${Build}";
        [JsonIgnore]
        public string CSSVersion => $"{Major}.${Minor}.${CSSBuild}";
    }


    public class CSSVersionInfoBuilder
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        public string RootDir { get; }
        private ProductVersionInfo currentInfo;

        public string ProductVersionInfoPath => Path.Combine(RootDir, "ProductVersionInfo.json");
        public CSSVersionInfoBuilder() 
        {
            var currentDir = Directory.GetCurrentDirectory();
            var lastDir = string.Empty;
            while (!Directory.Exists(Path.Combine(currentDir, "ShatteredSunCommunity")) && currentDir != lastDir)
            {
                lastDir = currentDir;
                currentDir = Path.GetDirectoryName(currentDir);
            }
            RootDir = Path.Combine(currentDir, "ShatteredSunCommunity");
        }
        public void UpdateVersionFile()
        {
            var current = GetCurrent();
            var calculated = GetCalculated();
            if (current.VersionInfo.CSSFilesHashCode != calculated.VersionInfo.CSSFilesHashCode)
            {
                ++calculated.CSSBuild;
                File.WriteAllText(
                    ProductVersionInfoPath,
                    JsonSerializer.Serialize(calculated, jsonSerializerOptions));
            }
        }

        public ProductVersionInfo GetCurrent()
        {
            if (currentInfo == null)
            {
                if (!File.Exists(ProductVersionInfoPath))
                    File.WriteAllText(
                        ProductVersionInfoPath, 
                        JsonSerializer.Serialize(new ProductVersionInfo(), jsonSerializerOptions));
                var json = File.ReadAllText(ProductVersionInfoPath);
                Debug.Assert(jsonSerializerOptions != null);
                Debug.Assert(json != null);
                currentInfo = JsonSerializer.Deserialize<ProductVersionInfo>(json, jsonSerializerOptions) ?? new ProductVersionInfo();
            }
            Contract.Ensures(currentInfo != null);
            Contract.Ensures(Contract.Result<ProductVersionInfo>() != null);
            Debug.Assert(currentInfo != null);
            return currentInfo;
        }
        public ProductVersionInfo GetCalculated()
        {
            var current = GetCurrent();
            var calculated = new ProductVersionInfo
            {
                Major = current.Major,
                Minor = current.Minor,
                Build = current.Build,
                CSSBuild = current.CSSBuild,
                VersionInfo = new CSSVersionInfo
                {
                    Files = Directory
                        .GetFiles(RootDir, "*.css", SearchOption.AllDirectories)
                        .Select(f => new CSSFileVersionInfo
                        {
                            File = f.Substring(RootDir.Length + 1).Replace('\\', '/'),
                            FileHashCode = CSSVersionInfo.GetFileHash(f),
                        })
                        .ToList()
                        }
            };
            calculated.VersionInfo.UpdateCSSFilesHashCode();
            return calculated;
        }

    }
}