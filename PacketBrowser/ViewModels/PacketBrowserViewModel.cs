using Microsoft.Win32;
using PacketBrowser.Models;
using siof.Common.Extensions;
using siof.Common.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PacketBrowser.ViewModels
{
    public class PacketBrowserViewModel : ViewModelBase
    {
        private string _headerSearchText;
        private string _searchText;
        private SearchMode _searchingMode;
        private PacketDefinition _selectedPacket;

        public PacketBrowserViewModel() :
            base()
        {
            PacketDefinitions = new DispatchedObservableCollection<PacketDefinition>();
            PacketDefinitionsView = CollectionViewSource.GetDefaultView(PacketDefinitions);
            PacketDefinitionsView.Filter = PacketListFilter;
        }

        public Array AvailableSearchModes
        {
            get
            {
                return Enum.GetValues(typeof(SearchMode));
            }
        }

        public SearchMode SearchingMode
        {
            get
            {
                return _searchingMode;
            }

            set
            {
                if (_searchingMode != value)
                {
                    _searchingMode = value;
                    OnPropertyChanged(() => SearchingMode);
                }
            }
        }

        public string HeaderSearchText
        {
            get
            {
                return _headerSearchText;
            }

            set
            {
                if (_headerSearchText != value)
                {
                    _headerSearchText = value;
                    OnPropertyChanged(() => HeaderSearchText);
                }
            }
        }

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

        public DispatchedObservableCollection<PacketDefinition> PacketDefinitions { get; private set; }

        public ICollectionView PacketDefinitionsView { get; private set; }

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
                switch (SearchingMode)
                {
                    case SearchMode.SimpleContains:
                        {
                            if (SearchText.IsEmptyOrWhiteSpace())
                                return true;

                            if (definition.ToString().ToLower().Contains(SearchText.ToLower()))
                                return true;

                            break;
                        }

                    case SearchMode.PacketContains:
                        {
                            if (HeaderSearchText.IsEmptyOrWhiteSpace())
                                return true;

                            if (SearchText.IsEmptyOrWhiteSpace())
                                return true;

                            if (definition.PacketHeader.ToLower().Contains(HeaderSearchText.ToLower()))
                            {
                                if (SearchText.IsEmptyOrWhiteSpace() || definition.PacketData.ToLower().Contains(SearchText.ToLower()))
                                    return true;
                            }

                            break;
                        }

                    case SearchMode.Regex:
                        {
                            Regex regex = new Regex(SearchText);
                            try
                            {
                                return regex.IsMatch(definition.ToString());
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        }
                }

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
            return !IsLoading;
        }

        #endregion Search

        #region LoadPacket

        private enum StreamMode
        {
            Header,
            Data
        }

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

        private void LoadPacketCommandExecute(object param)
        {
            string fileName = null;

            if (param != null)
            {
                var data = (param as IDataObject);
                if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    fileName = (data.GetData(DataFormats.FileDrop) as string[])[0];
                }
            }

            if (string.IsNullOrEmpty(fileName))
            {
                OpenFileDialog dialog = new OpenFileDialog();

                var result = dialog.ShowDialog().GetValueOrDefault(false);
                if (!result)
                    return;

                fileName = dialog.FileName;
            }

            Task.Run(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                IsLoading = true;
                IsLoadingInfoText = Properties.Resources.STR_Loading;

                PacketDefinitions.Clear();

                LinkedList<PacketDefinition> definitions = new LinkedList<PacketDefinition>();

                try
                {
                    int counter = 0;
                    using (var stream = File.Open(fileName, FileMode.Open))
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

                                if (mode == StreamMode.Header && line.IsEmptyOrWhiteSpace())
                                    continue;

                                if (line.StartsWith("ServerToClient") || line.StartsWith("ClientToServer"))
                                {
                                    definition.PacketData = dataBuilder.ToString();
                                    dataBuilder.Clear();

                                    if (definition.PacketName.IsNotEmptyOrWhiteSpace())
                                    {
                                        definitions.AddLast(definition);

                                        if (++counter >= 2500)
                                        {
                                            IsLoadingInfoText = string.Format("{0}: {1}", Properties.Resources.STR_Loading, definitions.Count);
                                            counter = 0;
                                        }
                                    }

                                    definition = new PacketDefinition();
                                    mode = StreamMode.Header;
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

                            IsLoadingInfoText = string.Format("{0}: {1}", Properties.Resources.STR_Loaded, definitions.Count);
                        }
                    }

                    IsLoadingInfoText = Properties.Resources.STR_PreparingView;
                    PacketDefinitions.ReplaceAll(definitions);
                    definitions.Clear();
                    watch.Stop();
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

        private bool LoadPacketCommandCanExecute(object param)
        {
            return true;
        }

        #endregion LoadPacket
    }
}
