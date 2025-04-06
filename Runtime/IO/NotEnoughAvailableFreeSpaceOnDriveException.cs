using System;
using System.IO;
using Aurora.Pooling;

namespace Aurora.IO
{
    /// <summary>
    /// 当驱动器上没有足够的可用空闲空间时引发的异常。
    /// </summary>
    public sealed class NotEnoughAvailableFreeSpaceOnDriveException : IOException
    {
        private string _message;

        /// <summary>
        /// 初始化 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> 类的新实例。
        /// </summary>
        public NotEnoughAvailableFreeSpaceOnDriveException()
        {
            DriveInfo                 = null;
            AvailableFreeSpaceOnDrive = null;
        }

        /// <summary>
        /// 使用指定的驱动器信息初始化 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> 类的新实例。
        /// </summary>
        /// <param name="driveInfo">驱动器信息。</param>
        public NotEnoughAvailableFreeSpaceOnDriveException(DriveInfo driveInfo)
        {
            DriveInfo                 = driveInfo;
            AvailableFreeSpaceOnDrive = null;
        }

        /// <summary>
        /// 使用指定的驱动器信息和可用空闲空间初始化 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> 类的新实例。
        /// </summary>
        /// <param name="driveInfo">驱动器信息。</param>
        /// <param name="availableFreeSpaceOnDrive">可用空闲空间（以字节为单位）。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="availableFreeSpaceOnDrive"/> 小于 0。</exception>
        public NotEnoughAvailableFreeSpaceOnDriveException(DriveInfo driveInfo, long availableFreeSpaceOnDrive)
        {
            if (availableFreeSpaceOnDrive < 0L)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(availableFreeSpaceOnDrive),
                    availableFreeSpaceOnDrive,
                    null
                );
            }
            DriveInfo                 = driveInfo;
            AvailableFreeSpaceOnDrive = availableFreeSpaceOnDrive;
        }

        /// <summary>
        /// 获取从构造函数传入的驱动器信息。
        /// </summary>
        public DriveInfo DriveInfo { get; }

        /// <summary>
        /// 获取从构造函数传入的驱动器可用空闲空间，若没有传入，则为 <see langword="null"/>。
        /// </summary>
        public long? AvailableFreeSpaceOnDrive { get; }

        /// <inheritdoc />
        public override string Message => _message ??= CreateMessage();

        private string CreateMessage()
        {
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                var driveInfo = DriveInfo;
                if (driveInfo == null)
                {
                    stringBuilder.Append("Not enough available free space on drive.");
                    var availableFreeSpaceOnDrive = AvailableFreeSpaceOnDrive;
                    if (availableFreeSpaceOnDrive.HasValue)
                    {
                        stringBuilder.Append(' ');
                        stringBuilder.AppendFormat(
                            "The available free space on drive is {0}.",
                            availableFreeSpaceOnDrive.Value
                        );
                    }
                }
                else
                {
                    stringBuilder.AppendFormat("Not enough available free space on drive \"{0}\".", driveInfo.Name);
                    var availableFreeSpaceOnDrive = AvailableFreeSpaceOnDrive;
                    if (!availableFreeSpaceOnDrive.HasValue && driveInfo.IsReady)
                    {
                        GetAvailableFreeSpaceNoThrow(ref availableFreeSpaceOnDrive, driveInfo);
                    }
                    if (availableFreeSpaceOnDrive.HasValue)
                    {
                        stringBuilder.Append(' ');
                        stringBuilder.AppendFormat(
                            "The available free space on drive is {0}.",
                            availableFreeSpaceOnDrive.Value
                        );
                    }
                }
                return stringBuilder.ToString();
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static void GetAvailableFreeSpaceNoThrow(ref long? availableFreeSpaceOnDrive, DriveInfo driveInfo)
        {
            try
            {
                availableFreeSpaceOnDrive = driveInfo.AvailableFreeSpace;
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
        }
    }
}
