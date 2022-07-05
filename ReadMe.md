# nuget Release-Repack
A utility to repack a prerelease nuget-package as release without recompilation.

Please keep in mind, that repackaging destroys the signature of a package. Therefore, repackaged packages are not signed.

## Usage
```
# Repackage My.Package.1.0.4-alpha.22272.1.nupkg to My.Package.1.0.4.nupkg
nuget-release-repack --path-to-prerelease c:\Somewhere\My.Package.1.0.4-alpha.22272.1.nupkg --output-directory c:\TargetDirectory
```
----

(c) 2022 TillW - Licensed to you under the MIT License