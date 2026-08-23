using System;
using System.IO;
using Aurora.Pooling;

namespace Aurora.IO
{
    /// <summary>
    /// The exception thrown when there is not enough available free space on a drive.
    /// </summary>
    public sealed class NotEnoughAvailableFreeSpaceOnDriveException : IOException
    {
        private string _message;

        private readonly DriveInfo _driveInfo;

        private readonly long? _availableFreeSpaceOnDrive;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> class.
        /// </summary>
        public NotEnoughAvailableFreeSpaceOnDriveException()
        {
            _driveInfo                 = null;
            _availableFreeSpaceOnDrive = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> class with the specified drive information.
        /// </summary>
        /// <param name="driveInfo">The drive information.</param>
        public NotEnoughAvailableFreeSpaceOnDriveException(DriveInfo driveInfo)
        {
            _driveInfo                 = driveInfo;
            _availableFreeSpaceOnDrive = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughAvailableFreeSpaceOnDriveException"/> class with the specified drive information and available free space.
        /// </summary>
        /// <param name="driveInfo">The drive information.</param>
        /// <param name="availableFreeSpaceOnDrive">The available free space (in bytes).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="availableFreeSpaceOnDrive"/> is less than 0.</exception>
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
        /// Gets the drive information passed into the constructor.
        /// </summary>
        public DriveInfo DriveInfo => _driveInfo;

        /// <summary>
        /// Gets the drive's available free space passed into the constructor; if none was passed, it is <see langword="null"/>.
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
