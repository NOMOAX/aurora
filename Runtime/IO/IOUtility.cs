using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aurora.IO
{
    /// <summary>
    /// 提供一组 IO 方法。
    /// </summary>
    public static class IOUtility
    {
        /// <summary>
        /// 获取驱动器的可用空闲空间。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <returns>指定的路径所属的驱动器的可用空闲空间。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException">找不到 <paramref name="path"/> 所属的驱动器，或者 <paramref name="path"/> 是 UNC（例如 \\server\share）路径。</exception>
        /// <exception cref="UnauthorizedAccessException">对驱动器信息的访问被拒绝。</exception>
        /// <exception cref="IOException">驱动器错误。</exception>
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
        /// 如果驱动器的可用空闲空间小于指定的值，则抛出 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/>。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="neededAvailableFreeSpace">需要的可用空闲空间。</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="neededAvailableFreeSpace"/> 小于 0。</exception>
        /// <exception cref="ArgumentException">找不到 <paramref name="path"/> 所属的驱动器，或者 <paramref name="path"/> 是 UNC（例如 \\server\share）路径。</exception>
        /// <exception cref="NotEnoughAvailableFreeSpaceOnDriveException">驱动器的可用空闲空间小于 <paramref name="neededAvailableFreeSpace"/>。</exception>
        /// <exception cref="IOException">驱动器错误。</exception>
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
        /// 创建具有指定长度的文件。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="length">长度。</param>
        /// <returns>如果文件已存在，并且该文件的长度等于指定的长度，则为 <see langword="false"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 0。</exception>
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
        /// 创建具有指定长度的文件。
        /// </summary>
        /// <param name="fileInfo">文件信息。</param>
        /// <param name="length">长度。</param>
        /// <returns>如果文件已存在，并且该文件的长度等于指定的长度，则为 <see langword="false"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileInfo"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 0。</exception>
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
            var directoryInfo = fileInfo.Directory;
            if (directoryInfo is { Exists: false })
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
