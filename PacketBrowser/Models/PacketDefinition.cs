using PacketBrowser.Enums;
using siof.Common.Wpf;
using System;

namespace PacketBrowser.Models
{
    public class PacketDefinition: ViewModelBase
    {
        private PacketDirection _direction;

        public PacketDirection Direction
        {
            get => _direction;

            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    OnPropertyChanged(nameof(Direction));
                }
            }
        }

        private string _packetName;

        public string PacketName
        {
            get => _packetName;

            set
            {
                if (_packetName != value)
                {
                    _packetName = value;
                    OnPropertyChanged(nameof(PacketName));
                }
            }
        }

        private string _packetHash;

        public string PacketHash
        {
            get => _packetHash;

            set
            {
                if (_packetHash != value)
                {
                    _packetHash = value;
                    OnPropertyChanged(nameof(PacketHash));
                }
            }
        }

        private int _length;

        public int Length
        {
            get => _length;

            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        private int _connIdx;

        public int ConnIdx
        {
            get => _connIdx;

            set
            {
                if (_connIdx != value)
                {
                    _connIdx = value;
                    OnPropertyChanged(nameof(ConnIdx));
                }
            }
        }

        private DateTime _time;

        public DateTime Time
        {
            get => _time;

            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        private int _number;

        public int Number
        {
            get => _number;

            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        private string _packetData;

        public string PacketData
        {
            get => _packetData;

            set
            {
                if (_packetData != value)
                {
                    _packetData = value;
                    OnPropertyChanged(nameof(PacketData));
                }
            }
        }

        public string PacketHeader => $"{Direction}: {PacketName} ({PacketHash}) Length: {Length} ConnIdx: {ConnIdx} Time: {Time} Number: {Number}";

        public override string ToString()
        {
            return $"{PacketHeader}{Environment.NewLine}{PacketData}";
        }
    }
}
