using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aurora.IO
{
    /// <summary>
    /// Provides a set of IO methods.
    /// </summary>
    public static class IOUtility
    {
        /// <summary>
        /// Gets the drive's available free space.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The available free space of the drive that contains the specified path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The drive containing <paramref name="path"/> cannot be found, or <paramref name="path"/> is a UNC (for example \\server\share) path.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the drive information is denied.</exception>
        /// <exception cref="IOException">A drive error.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetAvailableFreeSpaceOnDrive(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            return InternalGetAvailableFreeSpaceOnDrive(path, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long InternalGetAvailableFreeSpaceOnDrive(string path, out DriveInfo driveInfo)
        {
            var rootDirectoryPath = Path.GetPathRoot(path);
            if (string.IsNullOrEmpty(rootDirectoryPath))
            {
                throw new ArgumentException();
            }
            driveInfo = new DriveInfo(rootDirectoryPath);
            if (!driveInfo.IsReady)
            {
                return 0L;
            }
            var availableFreeSpace = driveInfo.AvailableFreeSpace;
            if (availableFreeSpace < 0L)
            {
                throw new IOException();
            }
            return availableFreeSpace;
        }

        /// <summary>
        /// Throws <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> if the drive's available free space is less than the specified value.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="neededAvailableFreeSpace">The required available free space.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="neededAvailableFreeSpace"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The drive containing <paramref name="path"/> cannot be found, or <paramref name="path"/> is a UNC (for example \\server\share) path.</exception>
        /// <exception cref="NotEnoughAvailableFreeSpaceOnDriveException">The drive's available free space is less than <paramref name="neededAvailableFreeSpace"/>.</exception>
        /// <exception cref="IOException">A drive error.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNotEnoughAvailableFreeSpaceOnDrive(string path, long neededAvailableFreeSpace)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (neededAvailableFreeSpace < 0L)
            {
                throw new ArgumentOutOfRangeException(nameof(neededAvailableFreeSpace), neededAvailableFreeSpace, null);
            }
            InternalThrowIfNotEnoughAvailableFreeSpaceOnDrive(path, neededAvailableFreeSpace);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalThrowIfNotEnoughAvailableFreeSpaceOnDrive(
            string path,
            long   neededAvailableFreeSpace)
        {
            var availableFreeSpaceOnDrive = InternalGetAvailableFreeSpaceOnDrive(path, out var driveInfo);
            if (availableFreeSpaceOnDrive >= neededAvailableFreeSpace)
            {
                return;
            }
            throw new NotEnoughAvailableFreeSpaceOnDriveException(driveInfo, availableFreeSpaceOnDrive);
        }

        /// <summary>
        /// Creates a file with the specified length.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="length">The length.</param>
        /// <returns><see langword="false"/> if the file already exists and its length equals the specified length; otherwise, <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than 0.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateEmptyFile(string path, long length)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (length < 0L)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, null);
            }
            var fileInfo = new FileInfo(path);
            return InternalCreateEmptyFile(fileInfo, length);
        }

        /// <summary>
        /// Creates a file with the specified length.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="length">The length.</param>
        /// <returns><see langword="false"/> if the file already exists and its length equals the specified length; otherwise, <see langword="true"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileInfo"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than 0.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateEmptyFile(FileInfo fileInfo, long length)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
            if (length < 0L)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, null);
            }
            return InternalCreateEmptyFile(fileInfo, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalCreateEmptyFile(FileInfo fileInfo, long length)
        {
            var fileExists = fileInfo.Exists;
            if (fileExists)
            {
                var existingFileLength = fileInfo.Length;
                if (existingFileLength == length)
                {
                    return false;
                }
                var neededAvailableFreeSpace = Math.Max(existingFileLength - length, 0L);
                InternalThrowIfNotEnoughAvailableFreeSpaceOnDrive(fileInfo.FullName, neededAvailableFreeSpace);
            }
            else
            {
                InternalThrowIfNotEnoughAvailableFreeSpaceOnDrive(fileInfo.FullName, length);
            }
            if (fileInfo.Directory is { Exists: false } directoryInfo)
            {
                directoryInfo.Create();
            }
            using var fileStream = fileInfo.Create();
            if (length > 0L)
            {
                fileStream.SetLength(length);
                fileStream.Flush(true);
            }
            return true;
        }
    }
}
