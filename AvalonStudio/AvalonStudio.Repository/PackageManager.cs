﻿using AvalonStudio.Platforms;
using AvalonStudio.Repositories;
using AvalonStudio.Utils;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvalonStudio.Packages
{
    public enum PackageEnsureStatus
    {
        Unknown,
        NotFound,
        Found,
        Installed
    }

    public class PackageManager
    {
        private ILogger _logger;
        private const int DefaultFilePermissions = 0x755;
        private static List<IPackageAssetLoader> _assetLoaders = new List<IPackageAssetLoader>();

        public PackageManager(ILogger logger = null)
        {
            _logger = logger;

            if (logger == null)
            {
                _logger = new ConsoleNuGetLogger();
            }
        }

        private static List<Lazy<INuGetResourceProvider>> s_providers = new List<Lazy<INuGetResourceProvider>>(Repository.Provider.GetCoreV3());            

        private static readonly IEnumerable<SourceRepository> s_sourceRepositories = new List<SourceRepository> {
                    new SourceRepository(new PackageSource("http://nuget1.vitalelement.co.uk/repository/AvalonStudio/"), s_providers),
                    new SourceRepository(new PackageSource("http://nuget2.vitalelement.co.uk/repository/AvalonStudio/"), s_providers),
                    new SourceRepository(new PackageSource("https://nuget.vitalelement.co.uk/repository/AvalonStudio/"), s_providers)
            };

        public static NuGetFramework GetFramework()
        {
            return new NuGetFramework("AvalonStudio1.0");
        }

        public static InstalledPackagesCache GetCache()
        {
            return new InstalledPackagesCache(Path.Combine(Platform.ReposDirectory, "cachedPackages.xml"), Path.Combine(Platform.ReposDirectory, "installedPackages.xml"), false);
        }

        public static void RegisterAssetLoader(IPackageAssetLoader loader)
        {
            if (!_assetLoaders.Contains(loader))
            {
                _assetLoaders.Add(loader);
            }
        }

        public static async Task LoadAssetsAsync()
        {
            var installedPackages = ListInstalledPackages();

            foreach (var package in installedPackages)
            {
                var location = GetPackageDirectory(package);

                if (!string.IsNullOrEmpty(location))
                {
                    var files = Directory.EnumerateFiles(location, "*.*", SearchOption.AllDirectories);

                    await LoadAssetsFromFilesAsync(package.Id, package.Version.ToNormalizedString(), files);
                }
            }
        }

        private static async Task LoadAssetsFromFilesAsync(string package, string version, IEnumerable<string> files)
        {
            foreach (var assetLoader in _assetLoaders)
            {
                await assetLoader.LoadAssetsAsync(package, version, files);
            }
        }

        /// <summary>
        /// Ensures a package is installed and installs it isnt and is available.
        /// </summary>
        /// <param name="packageId">The package Id to install.</param>
        /// <param name="packageVersion">The package version to insall.</param>
        /// <param name="console">Console instance to log output to.</param>
        /// <param name="chmodFileMode">file mode to chmod files without extensions (unix platform only)</param>
        /// <returns>true if the package was already installed.</returns>
        public static Task<PackageEnsureStatus> EnsurePackage(string packageId, string packageVersion, IConsole console, int chmodFileMode = DefaultFilePermissions, bool ignoreRid = false)
        {
            return EnsurePackage(packageId, packageVersion, new AvalonConsoleNuGetLogger(console), chmodFileMode, ignoreRid);
        }

        /// <summary>
        /// Ensures a package is installed and installs it isnt and is available.
        /// </summary>
        /// <param name="packageId">The package Id to install.</param>
        /// <param name="console">Console instance to log output to.</param>
        /// <param name="chmodFileMode">file mode to chmod files without extensions (unix platform only)</param>
        /// <returns>true if the package was already installed.</returns>
        public static Task<PackageEnsureStatus> EnsurePackage(string packageId, IConsole console, int chmodFileMode = DefaultFilePermissions, bool ignoreRid = false)
        {
            return EnsurePackage(packageId, null, new AvalonConsoleNuGetLogger(console), chmodFileMode, ignoreRid);
        }

        /// <summary>
        /// Ensures a package is installed and installs it isnt and is available.
        /// </summary>
        /// <param name="packageId">The package Id to install.</param>
        /// <param name="packageVersion">The package version to insall.</param>
        /// <param name="console">Console instance to log output to.</param>
        /// <param name="chmodFileMode">file mode to chmod files without extensions (unix platform only)</param>
        /// <returns>true if the package was already installed.</returns>
        private static async Task<PackageEnsureStatus> EnsurePackage(string packageId, string packageVersion, ILogger console, int chmodFileMode = DefaultFilePermissions, bool ignoreRid = false)
        {
            var identity = new PackageIdentity(ignoreRid ? packageId : packageId + "." + Platform.AvalonRID,
                string.IsNullOrEmpty(packageVersion) ? null : new NuGetVersion(packageVersion));

            bool installed = false;

            if (!identity.HasVersion)
            {
                installed = GetPackageDirectory(packageId) != string.Empty;
            }
            else
            {
                installed = GetPackageDirectory(identity) != string.Empty;
            }

            if (!installed)
            {
                console.LogInformation($"Package: {packageId} will be installed.");
                console.LogInformation($"This may take some time...");

                var packages = await FindPackages(ignoreRid ? packageId : packageId + "." + Platform.AvalonRID);

                var package = packages.FirstOrDefault();

                if (package == null)
                {
                    console.LogInformation($"Unable to find package: {packageId}");

                    return PackageEnsureStatus.NotFound;
                }
                else
                {
                    string installVersion = package.Identity.Version.ToNormalizedString();

                    if (!string.IsNullOrEmpty(packageVersion))
                    {
                        var requestedVersion = NuGetVersion.Parse(packageVersion);

                        var versions = await package.GetVersionsAsync();
                        var matchingVersion = versions.FirstOrDefault(v => v.Version == requestedVersion);

                        if (matchingVersion == null)
                        {
                            console.LogInformation($"Unable to find package: {packageId} with version: {packageVersion}");
                            console.LogInformation($"It maybe this package is not available on {Platform.AvalonRID}");
                            return PackageEnsureStatus.NotFound;
                        }

                        installVersion = matchingVersion.Version.ToNormalizedString();
                    }

                    await InstallPackage(package.Identity.Id, installVersion, console, chmodFileMode);

                    return PackageEnsureStatus.Installed;
                }

            }
            else
            {
                return PackageEnsureStatus.Found;
            }
        }

        public static async Task InstallPackage(string packageId, string version, ILogger logger = null, int chmodFileMode = DefaultFilePermissions)
        {
            if (logger == null)
            {
                logger = new ConsoleNuGetLogger();
            }

            PackageIdentity identity = new PackageIdentity(packageId, new NuGet.Versioning.NuGetVersion(version));            

            var settings = NuGet.Configuration.Settings.LoadDefaultSettings(Platform.ReposDirectory, null, new MachineWideSettings(), false, true);

            ISourceRepositoryProvider sourceRepositoryProvider = new SourceRepositoryProvider(settings, s_providers);  // See part 2

            using (var installedPackageCache = GetCache())
            {
                var project = new AvalonStudioExtensionsFolderProject(GetFramework(), installedPackageCache, Platform.ReposDirectory);

                if (!project.PackageExists(identity))
                {
                    var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, Platform.ReposDirectory)
                    {
                        PackagesFolderNuGetProject = project,
                    };

                    bool allowPrereleaseVersions = true;
                    bool allowUnlisted = false;

                    ResolutionContext resolutionContext = new ResolutionContext(
                        DependencyBehavior.Lowest, allowPrereleaseVersions, allowUnlisted, VersionConstraints.None);

                    INuGetProjectContext projectContext = new ProjectContext(logger);                    

                    await packageManager.InstallPackageAsync(packageManager.PackagesFolderNuGetProject,
                        identity, resolutionContext, projectContext, s_sourceRepositories,
                        Array.Empty<SourceRepository>(),  // This is a list of secondary source respositories, probably empty
                        CancellationToken.None);

                    var packageDir = GetPackageDirectory(identity);

                    var files = Directory.EnumerateFiles(packageDir, "*.*", SearchOption.AllDirectories);

                    if (Platform.PlatformIdentifier != Platforms.PlatformID.Win32NT)
                    {
                        foreach (var file in files)
                        {
                            Platform.Chmod(file, chmodFileMode);
                        }
                    }

                    await LoadAssetsFromFilesAsync(packageId, version, files);
                }
                else
                {
                    logger.LogInformation("Package is already installed.");
                }
            }
        }

        public static async Task UninstallPackage(string packageId, string version, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = new ConsoleNuGetLogger();
            }

            PackageIdentity identity = new PackageIdentity(packageId, new NuGet.Versioning.NuGetVersion(version));

            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();

            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support

            var settings = NuGet.Configuration.Settings.LoadDefaultSettings(Platform.ReposDirectory, null, new MachineWideSettings(), false, true);

            ISourceRepositoryProvider sourceRepositoryProvider = new SourceRepositoryProvider(settings, providers);  // See part 2

            using (var installedPackageCache = GetCache())
            {
                var project = new AvalonStudioExtensionsFolderProject(GetFramework(), installedPackageCache, Platform.ReposDirectory);

                var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, new SolutionManager(), new DeleteOnRestartManager())
                {
                    PackagesFolderNuGetProject = project,
                };

                INuGetProjectContext projectContext = new ProjectContext(logger);

                var uninstallationContext = new UninstallationContext(true, true);

                await packageManager.UninstallPackageAsync(project, packageId, uninstallationContext, projectContext, CancellationToken.None);
            }
        }

        public static async Task<IEnumerable<PackageMetaData>> ListPackagesAsync(int max = 20)
        {
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support            

            var defaultRepo = s_sourceRepositories.FirstOrDefault();
            var packageMetadataResource = await defaultRepo.GetResourceAsync<PackageMetadataResource>();

            var prov = new V2FeedListResourceProvider();
            var feed = await prov.TryCreate(defaultRepo, CancellationToken.None);
            var lister = (V2FeedListResource)feed.Item2;

            var results = await lister.ListAsync(string.Empty, true, false, false, new ConsoleNuGetLogger(), CancellationToken.None);

            var enumerator = results.GetEnumeratorAsync();

            var result = new List<IPackageSearchMetadata>();

            while (max > 0)
            {
                await enumerator.MoveNextAsync();

                if (enumerator.Current == null)
                {
                    break;
                }

                result.Add(enumerator.Current);

                max--;
            }

            return result.Select(pmd => new PackageMetaData(pmd));
        }

        public static async Task<IEnumerable<IPackageSearchMetadata>> FindPackages(string packageName, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = new ConsoleNuGetLogger();
            }

            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support

            var defaultRepo = s_sourceRepositories.FirstOrDefault();

            var packageMetadataResource = await defaultRepo.GetResourceAsync<PackageMetadataResource>();

            var searchResource = await defaultRepo.GetResourceAsync<PackageSearchResource>();

            return await searchResource.SearchAsync(packageName, new SearchFilter(true), 0, 100, logger, CancellationToken.None);
        }

        public static IEnumerable<PackageIdentity> ListInstalledPackages()
        {
            using (var installedPackageCache = GetCache())
            {
                return installedPackageCache.GetInstalledPackagesAndDependencies();
            }
        }

        private static string GetPackageDirectory(PackageIdentity identity)
        {
            string result = string.Empty;

            using (var installedPackageCache = GetCache())
            {
                var project = new AvalonStudioExtensionsFolderProject(GetFramework(), installedPackageCache, Platform.ReposDirectory);

                result = project.GetInstalledPath(identity);
            }

            return result;
        }

        public static string GetPackageDirectory(string genericPackageId, string version = null, bool ignoreRid = false)
        {
            var result = string.Empty;

            var expectedFolder = ignoreRid ? genericPackageId : genericPackageId + "." + Platform.AvalonRID;

            IEnumerable<PackageIdentity> packageIds;

            if (string.IsNullOrEmpty(version))
            {
                packageIds = ListInstalledPackages().Where(s => s.Id.StartsWith(expectedFolder));
            }
            else
            {
                packageIds = ListInstalledPackages().Where(s => s.Id.StartsWith(expectedFolder));
                packageIds = packageIds.Where(s => s.Version.ToNormalizedString() == version);
            }

            var latest = packageIds.OrderByDescending(id => id.Version).FirstOrDefault();

            if (latest != null)
            {
                result = GetPackageDirectory(latest);
            }

            return result;
        }

        private static async Task<NuGetVersion> GetLatestMatchingVersion(string packageId, NuGetFramework currentFramework, VersionRange versionRange, SourceRepository sourceRepository, ILogger logger)
        {
            try
            {
                DependencyInfoResource dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackages(
                    packageId, NullSourceCacheContext.Instance, logger, CancellationToken.None);
                return dependencyInfo
                    .Select(x => x.Identity.Version)
                    .Where(x => x != null && (versionRange == null || versionRange.Satisfies(x)))
                    .DefaultIfEmpty()
                    .Max();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Could not get latest version for package {packageId} from source {sourceRepository}: {ex.Message}");
                return null;
            }
        }
    }
}