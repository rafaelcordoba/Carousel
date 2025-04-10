using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Carousel.Editor
{
    [InitializeOnLoad]
    public static class CarouselVersionChecker
    {
        private const string PACKAGE_NAME = "com.rafaelcordoba.carousel";
        private const string GITHUB_API_URL = "https://api.github.com/repos/rafaelcordoba/Carousel/contents/Packages/com.rafaelcordoba.carousel/package.json";
        private static readonly HttpClient httpClient = new HttpClient();
        
        static CarouselVersionChecker()
        {
            // Add a small delay to ensure Unity is fully initialized
            EditorApplication.delayCall += () =>
            {
                CheckForUpdates();
            };
        }

        private static async void CheckForUpdates()
        {
            try
            {
                // Get the current version from the package manifest
                string currentVersion = GetCurrentVersion();
                if (string.IsNullOrEmpty(currentVersion))
                {
                    Debug.LogWarning("[Carousel] Could not determine current package version.");
                    return;
                }

                // Get the latest version from GitHub
                string latestVersion = await GetLatestVersion();
                if (string.IsNullOrEmpty(latestVersion))
                {
                    Debug.LogWarning("[Carousel] Could not check for updates. Please check your internet connection.");
                    return;
                }

                // Compare versions
                if (IsNewerVersion(currentVersion, latestVersion))
                {
                    Debug.Log($"[Carousel] A new version is available: {latestVersion} (current: {currentVersion})");
                    
                    // Show update notification in the Package Manager
                    EditorApplication.delayCall += () =>
                    {
                        EditorUtility.DisplayDialog(
                            "Carousel Update Available",
                            $"A new version of Carousel is available: {latestVersion}\n\nCurrent version: {currentVersion}\n\nWould you like to update?",
                            "Update",
                            "Later"
                        );
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Carousel] Error checking for updates: {ex.Message}");
            }
        }

        private static string GetCurrentVersion()
        {
            try
            {
                // Get the package manifest path
                string packagePath = Path.GetFullPath("Packages/manifest.json");
                if (!File.Exists(packagePath))
                {
                    return null;
                }

                // Read the manifest file
                string manifestContent = File.ReadAllText(packagePath);
                
                // Parse the JSON to find our package
                // This is a simple approach - for production, consider using a proper JSON parser
                int packageIndex = manifestContent.IndexOf($"\"{PACKAGE_NAME}\":");
                if (packageIndex == -1)
                {
                    return null;
                }

                // Find the version string
                int versionStart = manifestContent.IndexOf("\"version\":", packageIndex);
                if (versionStart == -1)
                {
                    return null;
                }

                versionStart = manifestContent.IndexOf("\"", versionStart) + 1;
                int versionEnd = manifestContent.IndexOf("\"", versionStart);
                
                return manifestContent.Substring(versionStart, versionEnd - versionStart);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Carousel] Error getting current version: {ex.Message}");
                return null;
            }
        }

        private static async Task<string> GetLatestVersion()
        {
            try
            {
                // Set up the request
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Unity Package Manager");
                
                // Make the request
                HttpResponseMessage response = await httpClient.GetAsync(GITHUB_API_URL);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogWarning($"[Carousel] GitHub API returned status code: {response.StatusCode}");
                    return null;
                }

                // Parse the response
                string responseContent = await response.Content.ReadAsStringAsync();
                
                // Extract the content and decode it (GitHub API returns base64 encoded content)
                // This is a simplified approach - for production, consider using a proper JSON parser
                int contentStart = responseContent.IndexOf("\"content\":\"") + 11;
                int contentEnd = responseContent.IndexOf("\"", contentStart);
                string base64Content = responseContent.Substring(contentStart, contentEnd - contentStart);
                
                // Decode the base64 content
                byte[] contentBytes = Convert.FromBase64String(base64Content);
                string packageJson = System.Text.Encoding.UTF8.GetString(contentBytes);
                
                // Extract the version
                int versionStart = packageJson.IndexOf("\"version\":\"") + 11;
                int versionEnd = packageJson.IndexOf("\"", versionStart);
                
                return packageJson.Substring(versionStart, versionEnd - versionStart);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Carousel] Error getting latest version: {ex.Message}");
                return null;
            }
        }

        private static bool IsNewerVersion(string currentVersion, string latestVersion)
        {
            try
            {
                Version current = Version.Parse(currentVersion);
                Version latest = Version.Parse(latestVersion);
                
                return latest > current;
            }
            catch
            {
                // If version parsing fails, do a simple string comparison
                return string.Compare(latestVersion, currentVersion, StringComparison.Ordinal) > 0;
            }
        }
    }
} 