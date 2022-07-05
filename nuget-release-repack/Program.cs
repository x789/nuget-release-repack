// Copyright (c) 2022 - TillW
// Licensed to you under the MIT License

using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

public sealed class Program
{
    /// <summary>
    /// nuget-release-repack - A utility to repack a prerelease nuget-package as release without recompilation.
    /// </summary>
    /// <param name="pathToPrerelease">Location of the prerelease package to repack.</param>
    /// <param name="outputDirectory">Directory that will contain the repackaged file.</param>
    /// <returns>0 on success</returns>
    static int Main(string pathToPrerelease, string outputDirectory)
    {
        try
        {
            Repackage(pathToPrerelease, outputDirectory);
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("A critical error occurred: " + e.Message);
            Console.WriteLine("Use CLI option '--help' to get help.");
            return 255;
        }
    }

    private static void Repackage(string pathToPrerelease, string outputDirectory)
    {
        ArgumentNullException.ThrowIfNull(pathToPrerelease);
        ArgumentNullException.ThrowIfNull(outputDirectory);

        using var prerelease = ZipFile.OpenRead(pathToPrerelease);
        XDocument nuspec = default;
        string releaseVersion = default;
        string packageId = default;
        foreach (var entry in prerelease.Entries)
        {
            if (entry.Name.ToLowerInvariant().EndsWith(".nuspec"))
            {
                using var nuspecStream = entry.Open();
                nuspec = XDocument.Load(nuspecStream);
                packageId = nuspec.Descendants().Where(x => x.Name.LocalName == "id").First().Value;
                var versionNode = nuspec.Descendants().Where(x => x.Name.LocalName == "version").First();
                releaseVersion = RemoveSemVerSuffix(versionNode.Value);
                versionNode.ReplaceWith(new XElement(versionNode.Name, releaseVersion));
                nuspecStream.Close();
            }
        }

        using var release = ZipFile.Open(Path.Combine(outputDirectory, $"{packageId.ToLowerInvariant()}.{releaseVersion}.nupkg"), ZipArchiveMode.Create);
        foreach (var source in prerelease.Entries)
        {
            if (source.Name == ".signature.p7s")
            {
                // Skip the signature because it will not be valid for the repackaged file.
            }
            else if (!source.Name.ToLowerInvariant().EndsWith(".nuspec"))
            {
                var target = release.CreateEntry(source.FullName, CompressionLevel.Optimal);
                using var input = source.Open();
                using var output = target.Open();
                input.CopyTo(output);
                output.Close();
                input.Close();
            }
            else
            {
                var target = release.CreateEntry(source.FullName, CompressionLevel.Optimal);
                var output = target.Open();
                using var writer = XmlWriter.Create(output);
                nuspec.WriteTo(writer);
                writer.Close();
                output.Close();
            }
        }
    }

    private static string RemoveSemVerSuffix(string version)
    {
        var hyphenPosition = version.IndexOf('-');
        return hyphenPosition == -1 ? version : version.Substring(0, hyphenPosition);
    }
}