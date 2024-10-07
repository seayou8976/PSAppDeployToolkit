﻿using System;

namespace PSADT.Types
{
    /// <summary>
    /// Represents information about a user profile.
    /// </summary>
    public readonly struct UserProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfile"/> struct.
        /// </summary>
        /// <param name="ntAccount">The NT account associated with the user profile.</param>
        /// <param name="sid">The security identifier (SID) for the user profile.</param>
        /// <param name="profilePath">The path to the user's profile directory.</param>
        /// <param name="appDataPath">The path to the user's AppData directory.</param>
        /// <param name="localAppDataPath">The path to the user's LocalAppData directory.</param>
        /// <param name="desktopPath">The path to the user's Desktop directory.</param>
        /// <param name="documentsPath">The path to the user's Documents directory.</param>
        /// <param name="oneDrivePath">The path to the user's OneDrive directory.</param>
        /// <param name="oneDriveCommercialPath">The path to the user's OneDrive for Business directory.</param>
        /// <param name="startMenuPath">The path to the user's Start Menu directory.</param>
        /// <param name="tempPath">The path to the user's Temp directory.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required parameter is null or empty.</exception>
        public UserProfile(
            string ntAccount,
            string sid,
            string profilePath,
            string appDataPath,
            string localAppDataPath,
            string desktopPath,
            string documentsPath,
            string oneDrivePath,
            string oneDriveCommercialPath,
            string startMenuPath,
            string tempPath)
        {
            NTAccount = !string.IsNullOrWhiteSpace(ntAccount) ? ntAccount : throw new ArgumentNullException(nameof(ntAccount), "NT account cannot be null or empty.");
            SID = !string.IsNullOrWhiteSpace(sid) ? sid : throw new ArgumentNullException(nameof(sid), "SID cannot be null or empty.");
            ProfilePath = !string.IsNullOrWhiteSpace(profilePath) ? profilePath : throw new ArgumentNullException(nameof(profilePath), "Profile path cannot be null or empty.");
            AppDataPath = appDataPath ?? string.Empty;
            LocalAppDataPath = localAppDataPath ?? string.Empty;
            DesktopPath = desktopPath ?? string.Empty;
            DocumentsPath = documentsPath ?? string.Empty;
            OneDrivePath = oneDrivePath ?? string.Empty;
            OneDriveCommercialPath = oneDriveCommercialPath ?? string.Empty;
            StartMenuPath = startMenuPath ?? string.Empty;
            TempPath = tempPath ?? string.Empty;
        }

        /// <summary>
        /// Gets the NT account associated with the user profile.
        /// </summary>
        public string NTAccount { get; }

        /// <summary>
        /// Gets the security identifier (SID) for the user profile.
        /// </summary>
        public string SID { get; }

        /// <summary>
        /// Gets the path to the user's profile directory.
        /// </summary>
        public string ProfilePath { get; }
        /// <summary>
        /// Gets the path to the user's AppData directory.
        /// </summary>

        public string AppDataPath { get; }

        /// <summary>
        /// Gets the path to the user's LocalAppData directory.
        /// </summary>
        public string LocalAppDataPath { get; }

        /// <summary>
        /// Gets the path to the user's Desktop directory.
        /// </summary>
        public string DesktopPath { get; }

        /// <summary>
        /// Gets the path to the user's Documents directory.
        /// </summary>
        public string DocumentsPath { get; }

        /// <summary>
        /// Gets the path to the user's OneDrive directory.
        /// </summary>
        public string OneDrivePath { get; }

        /// <summary>
        /// Gets the path to the user's OneDrive for Business directory.
        /// </summary>
        public string OneDriveCommercialPath { get; }

        /// <summary>
        /// Gets the path to the user's Start Menu directory.
        /// </summary>
        public string StartMenuPath { get; }

        /// <summary>
        /// Gets the path to the user's Temp directory.
        /// </summary>
        public string TempPath { get; }

        /// <summary>
        /// Returns a string that represents the current <see cref="UserProfile"/> object.
        /// </summary>
        /// <returns>A string that represents the user profile information.</returns>
        public override string ToString()
        {
            return $"NT Account: [{NTAccount}], SID: [{SID}], Profile Path: [{ProfilePath}].";
        }
    }
}
