using Microsoft.Win32;
using PacketBrowser.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfCommons;
using WpfCommons.Extensions;

namespace PacketBrowser.ViewModels
{
    public class PacketBrowserViewModel : ViewModelBase
    {
        public PacketBrowserViewModel() :
            base()
        {
            _packetDefinitions = new DispatchedObservableCollection<PacketDefinition>();
            PacketDefinitionsView = CollectionViewSource.GetDefaultView(PacketDefinitions);
            PacketDefinitionsView.Filter = PacketListFilter;
        }

        private string _searchText;

        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(() => SearchText);
                }
            }
        }

        private DispatchedObservableCollection<PacketDefinition> _packetDefinitions;

        public DispatchedObservableCollection<PacketDefinition> PacketDefinitions
        {
            get
            {
                return _packetDefinitions;
            }
        }

        public ICollectionView PacketDefinitionsView { get; private set; }

        private PacketDefinition _selectedPacket;

        public PacketDefinition SelectedPacket
        {
            get
            {
                return _selectedPacket;
            }

            set
            {
                if (_selectedPacket != value)
                {
                    _selectedPacket = value;
                    OnPropertyChanged(() => SelectedPacket);
                }
            }
        }

        private bool PacketListFilter(object obj)
        {
            return (obj as PacketDefinition).IfNotNull(definition =>
            {
                if (SearchText.IsEmptyOrWhiteSpace())
                    return true;

                if (definition.ToString().ToLower().Contains(SearchText.ToLower()))
                    return true;

                return false;
            }, false);
        }

        #region Search

        private RelayCommand<object> _searchCommand;

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<object>(param => SearchCommandExecute(param), param => SearchCommandCanExecute(param));
                }
                return _searchCommand;
            }
        }

        private void SearchCommandExecute(object param)
        {
            PacketDefinitionsView.Refresh();
        }

        private bool SearchCommandCanExecute(object param)
        {
            return true;
        }

        #endregion Search

        #region LoadPacket

        private RelayCommand<object> _loadPacketsCommand;

        public ICommand LoadPacketsCommand
        {
            get
            {
                if (_loadPacketsCommand == null)
                {
                    _loadPacketsCommand = new RelayCommand<object>(param => LoadPacketCommandExecute(param), param => LoadPacketCommandCanExecute(param));
                    _loadPacketsCommand.UpdateCanExecuteState();
                }
                return _loadPacketsCommand;
            }
        }

        private enum StreamMode
        {
            Header,
            Data
        }

        private void LoadPacketCommandExecute(object param)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                Task.Run(() =>
                {
                    IsLoading = true;
                    PacketDefinitions.Clear();
                    try
                    {
                        using (var stream = dialog.OpenFile())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                var definition = new PacketDefinition();
                                StreamMode mode = StreamMode.Header;
                                StringBuilder dataBuilder = new StringBuilder();

                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();

                                    if (line.StartsWith("#"))
                                        continue;

                                    if (line.IsEmptyOrWhiteSpace())
                                    {
                                        definition.PacketData = dataBuilder.ToString();
                                        dataBuilder.Clear();

                                        if (definition.PacketName.IsNotEmptyOrWhiteSpace())
                                            PacketDefinitions.Add(definition);

                                        definition = new PacketDefinition();
                                        mode = StreamMode.Header;
                                        continue;
                                    }

                                    switch (mode)
                                    {
                                        case StreamMode.Header:
                                            {
                                                var data = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                                definition.Direction = data[0].Trim(':') == "ServerToClient" ? PacketDirection.ServerToClient : PacketDirection.ClientToServer;
                                                definition.PacketName = data[1];
                                                definition.PacketHash = data[2].Trim('(').Trim(')');
                                                definition.Length = int.Parse(data[4]);
                                                definition.ConnIdx = int.Parse(data[6]);
                                                definition.Time = DateTime.ParseExact(data[8], "mm/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture) + TimeSpan.Parse(data[9]);
                                                definition.Number = int.Parse(data[11]);
                                                mode = StreamMode.Data;
                                                break;
                                            }
                                        case StreamMode.Data:
                                            {
                                                dataBuilder.AppendLine(line);
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                });
            }
        }

        private bool LoadPacketCommandCanExecute(object param)
        {
            return true;
        }

        #endregion LoadPacket
    }
}
