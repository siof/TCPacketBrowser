using System;
using WpfCommons;

namespace PacketBrowser.Models
{
    public class PacketDefinition : ViewModelBase
    {
        private PacketDirection _direction;

        public PacketDirection Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    OnPropertyChanged(() => Direction);
                }
            }
        }

        private string _packetName;

        public string PacketName
        {
            get
            {
                return _packetName;
            }

            set
            {
                if (_packetName != value)
                {
                    _packetName = value;
                    OnPropertyChanged(() => PacketName);
                }
            }
        }

        private string _packetHash;

        public string PacketHash
        {
            get
            {
                return _packetHash;
            }

            set
            {
                if (_packetHash != value)
                {
                    _packetHash = value;
                    OnPropertyChanged(() => PacketHash);
                }
            }
        }

        private int _length;

        public int Length
        {
            get
            {
                return _length;
            }

            set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(() => Length);
                }
            }
        }

        private int _connIdx;

        public int ConnIdx
        {
            get
            {
                return _connIdx;
            }

            set
            {
                if (_connIdx != value)
                {
                    _connIdx = value;
                    OnPropertyChanged(() => ConnIdx);
                }
            }
        }

        private DateTime _time;

        public DateTime Time
        {
            get
            {
                return _time;
            }

            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged(() => Time);
                }
            }
        }

        private int _number;

        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(() => Number);
                }
            }
        }

        private string _packetData;

        public string PacketData
        {
            get
            {
                return _packetData;
            }

            set
            {
                if (_packetData != value)
                {
                    _packetData = value;
                    OnPropertyChanged(() => PacketData);
                }
            }
        }

        public string PacketHeader
        {
            get
            {
                return string.Format("{0}: {1} ({2}) Length: {3} ConnIdx: {4} Time: {5} Number: {6}", Direction, PacketName, PacketHash, Length, ConnIdx, Time, Number);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                PacketHeader, Environment.NewLine, PacketData);
        }
    }
}
