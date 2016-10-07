//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Windows.Input;
//using Windows.Networking.Proximity;
//using Windows.Storage.Streams;

//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.IO;
//using System.Collections;

//namespace NfcEditor.ViewModels
//{
//    class MainWindowViewModel : INotifyPropertyChanged
//    {
//        private string _Url;
//        private bool _NfcDetected;
//        private ProximityDevice _proximityDevice;
//        private long _MessageType;
//        string MyPath = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Ny mapp"), "ytterligare");
//        string MyPath2 = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Ny mapp"), "ytterligareIgen");








//        //public RelayCommand Play { get; set; }

//        //private MediaElement media;

//        //// Bound media property with notification
//        //public MediaElement Media
//        //{
//        //    get
//        //    {
//        //        return this.media;
//        //    }
//        //    set
//        //    {
//        //        this.media = value;

//        //        // Modify following line as needed
//        //        // to any custom Property changed handler
//        //        //this.Changed("Media");
//        //    }
//        //}









//        public MainWindowViewModel()
//        {
//            _proximityDevice = Windows.Networking.Proximity.ProximityDevice.GetDefault();
//            if (_proximityDevice != null)
//            {
//                _proximityDevice.DeviceArrived += _proximityDevice_DeviceArrived;
//                _proximityDevice.DeviceDeparted += _proximityDevice_DeviceDeparted;
//                _MessageType = _proximityDevice.SubscribeForMessage("WindowsUri", MessageReceivedHandler);
//            }
//            Console.WriteLine("hÄR ÄR litetext2FGHJKJHBGHJGYU(YTGYGTHYG");
//            try
//            {
//                Directory.CreateDirectory(MyPath);
//                Directory.CreateDirectory(MyPath2);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Failed: {0}", e.ToString());
//            }
//            finally { }



//           // this.Play = new RelayCommand(this.ExecutePlayCommand);

//            //// The Media element
//            //this.Media = new MediaElement();

//            //// This is where we need to set Loaded and Unloaded behavior to Manual
//            //// This means video won't play right-away after loading
//            //// You will have to call .Play() method to start playback
//            //// Add a play button if you don't have one and bind the Play command
//            //this.Media.LoadedBehavior = MediaState.Manual;
//            //this.Media.UnloadedBehavior = MediaState.Manual;

//            //// Following is just an example. Doesn't need to be here
//            //this.Media.Source = new Uri("test.mp4", UriKind.Relative);






//        }








//        //public void ExecutePlayCommand()
//        //{
//        //    this.Media.Play();
//        //}












//        void _proximityDevice_DeviceDeparted(ProximityDevice sender)
//        {
//            NfcDetected = false;
//            Url = "http://";
//            Console.WriteLine("Här skall det ha oregistrerats");
//        }

//        void _proximityDevice_DeviceArrived(ProximityDevice sender)
//        {
//            NfcDetected = true;
//            Console.WriteLine("Här skall det ha registrerats");
//            SetMedia();
//            //MediaElement MediaElementObject = new MediaElement();
//           // MediaElementObject.Source = new Uri("C:\\Users\\001272\\Music\\Ny mapp\\wer.mp3");
//           // MediaElementObject.Play();
//            //PlayMedia();
//            //PlayMedia("System.Windows.Controls.MediaElement");
//            //MyMediaElement.Source = VideoToPlay;
//            //MyMediaElement.Play();
//        }

//        private void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
//        {
//            try
//            {
//                using (var reader = DataReader.FromBuffer(message.Data))
//                {
//                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
//                    string receivedString = reader.ReadString(reader.UnconsumedBufferLength / 2 - 1);
//                    Debug.WriteLine("Received message from NFC: " + receivedString);
//                    Url = receivedString;
//                }

//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine(e.StackTrace);
//            }

//        }


//        private void DoWriteTag()
//        {
//            try
//            {
//                using (var writer = new DataWriter { UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE })
//                {
//                    Debug.WriteLine("Writing message to NFC: " + Url);
//                    writer.WriteString(Url);
//                    long id = _proximityDevice.PublishBinaryMessage("WindowsUri:WriteTag", writer.DetachBuffer());
//                    _proximityDevice.StopPublishingMessage(id);
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine(e.StackTrace);
//            }

//        }

//        RelayCommand _writeCommand;
//        public ICommand WriteCommand
//        {
//            get
//            {
//                if (_writeCommand == null)
//                {
//                    _writeCommand = new RelayCommand(p => this.DoWriteTag(), p => this.NfcDetected);
//                }
//                return _writeCommand;
//            }
//        }

//        public string Url
//        {
//            get { return _Url; }
//            set
//            {
//                if (value.Equals(_Url)) return;
//                OnPropertyChanged();
//                _Url = value;
//            }
//        }

//        public bool NfcDetected
//        {
//            get { return _NfcDetected; }
//            set
//            {
//                _NfcDetected = value;
//                OnPropertyChanged("NfcDetected");
//                OnPropertyChanged("NfcSearching");
//            }
//        }

//        public bool NfcSearching
//        {
//            get { return !_NfcDetected; }
//        }



//        public event PropertyChangedEventHandler PropertyChanged;

//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChangedEventHandler handler = PropertyChanged;
//            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
//        }



//        //public void Element_MediaElementEnded(object sender, RoutedEventArgs e)
//        //{
//        //    NfcEditor.Views.MainWindow.MediaElement.Stop(); //Stops and resets media to be played again
//        //}

//        //Eventhanterare för det fall mediefilen inte kan läsas.
//        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
//        {
//            Console.WriteLine();
//            Console.WriteLine("GetFilderPath: {0}");//,
//            //filen2());
//        }
//        //public static void Mede()
//        //{
//        //    Console.WriteLine();
//        //    Console.WriteLine("GetFilderPath: {0}");//,
//        //    //filen2());
//        //}

//        void SetMedia()
//        {
//            var somefiles = Directory.EnumerateFiles(
//                MyPath, "*.*", SearchOption.AllDirectories);
//            if (somefiles.Count() != 0)
//            {
//                VideoToPlay = new Uri(somefiles.ElementAt(new Random().Next(0, somefiles.Count())));
//                //MediaElement.Source = new System.Uri(file);
//                return;
//            }
//            else return;

//        }





        
//        //Stuff for mediaelement, hopfully.
//        //private MediaElement MyMediaElement;

//        private Uri _videoToPlay;

//        public Uri VideoToPlay
//        {
//            get { return _videoToPlay; }
//            set
//            {
//                _videoToPlay = value;
//                OnPropertyChanged("VideoToPlay");
//            }
//        }

//        //RelayCommand _playMediaCommand;
//        //public ICommand PlayMediaCommand
//        //{
//        //    get
//        //    {
//        //        if (_playMediaCommand == null)
//        //        {
//        //            _playMediaCommand = new RelayCommand(p => PlayMedia(p),
//        //                p => true);
//        //        }
//        //        Console.WriteLine("gabbagabba: {0}", _playMediaCommand);
//        //        return _playMediaCommand;
//        //    }
//        //}

//        //void PlayMedia(object param)
//        //{
//        //    Console.WriteLine("detta är PlayMedia() som försöker skriva ut _playmediacommand: {0}", param);
//        //    var paramMediaElement = (MediaElement)param;
//        //    MyMediaElement = paramMediaElement;
//        //    MyMediaElement.Source = VideoToPlay;
//        //    MyMediaElement.Play();
//        //}


//        //protected void OnPropertyChanged(string propertyname)
//        //{
//        //    var handler = PropertyChanged;
//        //    if (handler != null)
//        //        handler(this, new PropertyChangedEventArgs(propertyname));
//        //}

//        //public event PropertyChangedEventHandler PropertyChanged;
//        //public event PropertyChangedEventHandler PropertyChanged;

//        //private MediaElement _mediaElementObject;

//        //public MediaElement MediaElementObject
//        //{
//        //    get { return _mediaElementObject; }
//        //    set { _mediaElementObject = value; RaisePropertyChanged("MediaElementObject"); }
//        //}


//        private void RaisePropertyChanged(string propertyName)
//        {
//            // take a copy to prevent thread issues
//            PropertyChangedEventHandler handler = PropertyChanged;
//            if (handler != null)
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }
//   // MediaElement MediaElementObject = new MediaElement();

//    }
//}
