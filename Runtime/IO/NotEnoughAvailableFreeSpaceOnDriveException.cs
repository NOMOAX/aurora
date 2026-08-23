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

        private readonly DriveInfo _driveInfo;

        private readonly long? _availableFreeSpaceOnDrive;

        /// <summary>
        /// 初始化 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> 类的新实例。
        /// </summary>
        public NotEnoughAvailableFreeSpaceOnDriveException()
        {
            _driveInfo                 = null;
            _availableFreeSpaceOnDrive = null;
        }

        /// <summary>
        /// 使用指定的驱动器信息初始化 <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> 类的新实例。
        /// </summary>
        /// <param name="driveInfo">驱动器信息。</param>
        public NotEnoughAvailableFreeSpaceOnDriveException(DriveInfo driveInfo)
        {
            _driveInfo                 = driveInfo;
            _availableFreeSpaceOnDrive = null;
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
            _driveInfo                 = driveInfo;
            _availableFreeSpaceOnDrive = availableFreeSpaceOnDrive;
        }

        /// <summary>
        /// 获取从构造函数传入的驱动器信息。
        /// </summary>
        public DriveInfo DriveInfo => _driveInfo;

        /// <summary>
        /// 获取从构造函数传入的驱动器可用空闲空间，若没有传入，则为 <see langword="null"/>。
        /// </summary>
        public long? AvailableFreeSpaceOnDrive => _availableFreeSpaceOnDrive;

        /// <inheritdoc />
        public override string Message => _message ??= CreateMessage();

        private string CreateMessage()
        {
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                var driveInfo = _driveInfo;
                if (driveInfo == null)
                {
                    stringBuilder.Append("Not enough available free space on drive.");
                    var availableFreeSpaceOnDrive = _availableFreeSpaceOnDrive;
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
                    var availableFreeSpaceOnDrive = _availableFreeSpaceOnDrive;
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
