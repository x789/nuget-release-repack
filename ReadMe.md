# nuget Release-Repack
A utility to repack a prerelease nuget-package as release without recompilation.

Please keep in mind, that repackaging invalidates the signature of a package. Therefore, repackaged packages are not signed.

## Installation
Check the [tool's nuget.org page](https://www.nuget.org/packages/nuget-release-repack/) for precompiled releases and how to install them.

## Usage
```
# To repack mypackage.1.0.0-preview974 as mypackage.1.0.0.nupkg:
nuget-release-repack --path-to-prerelease mypackage.1.0.0-preview974 --output-directory c:\Releases\
```
----

(c) 2022 TillW - Licensed to you under the MIT License