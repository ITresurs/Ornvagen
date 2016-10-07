
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Networking.Proximity;
using Windows.Storage.Streams;

namespace Ornvagen.Views
{

    public partial class MainWindow : Window
    {
        private bool _NfcDetected;
        private ProximityDevice _proximityDevice;
        private long _MessageType;

        adresslista AdressListan = new adresslista();

        public MainWindow()
        {
            InitializeComponent();
            AdressListan.readdictJson();

            _proximityDevice = Windows.Networking.Proximity.ProximityDevice.GetDefault();
            if (_proximityDevice != null)
            {
                _MessageType = _proximityDevice.SubscribeForMessage("WindowsUri", MessageReceivedHandler);
                _proximityDevice.DeviceArrived += _proximityDevice_DeviceArrived;
                _proximityDevice.DeviceDeparted += _proximityDevice_DeviceDeparted;
            }
        }


        void _proximityDevice_DeviceDeparted(ProximityDevice sender)
        {
            NfcDetected = false;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { showing(); AdressListan.LastCode = null; }));
            Debug.WriteLine("kul, nu blev det null. Det här är lastcode nu: ---"+ AdressListan.LastCode +"---   ---------------------------------------------");
        }

        void _proximityDevice_DeviceArrived(ProximityDevice sender)
        {
            NfcDetected = true;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { showing(); }));
            Debug.WriteLine("Det här är lascode när vår tagg uppmärksammas i proximitydevice arives: "+AdressListan.LastCode+" se där! --------------------------------------------------------");
        }

        private void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {           
            try
            {
                using (var reader = DataReader.FromBuffer(message.Data))
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
                    string receivedString = reader.ReadString(reader.UnconsumedBufferLength / 2 - 1);
                    Debug.WriteLine("Received message from NFC: " + receivedString);
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { winBorder(6); }));'
                    Debug.WriteLine("det här är lastcode innan den ändras av inkommande kod: "+AdressListan.LastCode+" ta da!------------------------------------------------");
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { winBorder(6); navigera(AdressListan.varde(receivedString)); AdressListan.LastCode = receivedString; }));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { AdressListan.LastCode = receivedString; }));
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
         }

        private void DoWriteTag(string adress)
        {
            try
            {
                using (var writer = new DataWriter { UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE })
                {
                    Debug.WriteLine("Writing message to NFC: " + adress);
                    writer.WriteString(adress);
                    long id = _proximityDevice.PublishBinaryMessage("WindowsUri:WriteTag", writer.DetachBuffer());
                    _proximityDevice.StopPublishingMessage(id);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        public bool NfcDetected
        {
            get { return _NfcDetected; }
            set
            {
                _NfcDetected = value;
                OnPropertyChanged("NfcDetected");
                OnPropertyChanged("NfcSearching");
            }
        }

        public bool NfcSearching
        {
            get { return !_NfcDetected; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void winBorder(int x) 
        {
            Thickness ram = window.BorderThickness;
            ram.Bottom = x;
            ram.Left = x;
            ram.Right = x;
            ram.Top = x;
            window.BorderThickness = ram;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {      
            if(_NfcDetected)
            {
                string keyCheck = AdressListan.LastCode;
                Debug.WriteLine("i knapptryck, Adresslistan.lastcode = "+ AdressListan.LastCode +" och keycheck = "+ keyCheck +"---------------------------------------------------------");
                if (AdressListan.exists(keyCheck))
                {
                    AdressListan.remove(keyCheck);
                    AdressListan.adding(keyCheck, adressNu().ToString());
                    AdressListan.writedictJson();
                    showingClosed();
                }
                else
                {
                    String dat = DateTime.Now.ToString();
                    DoWriteTag(dat);
                    AdressListan.adding(dat, adressNu().ToString());
                    AdressListan.writedictJson();
                    showing();
                }
            }
        }

        private void Other_Button_Click(object sender, RoutedEventArgs e)
        {
            navigera("about:blank");

            Thickness marginal = webBrowser.Margin;
            marginal.Top = -200;
            webBrowser.Margin = marginal;

            Thickness margin = HELLOHELLO_.Margin;
            margin.Bottom = 0;
            HELLOHELLO_.Margin = margin;

        }

        public void navigera(string hit)
        {
                webBrowser.Navigate(hit);     
        }

        public Uri adressNu()
        {
            return webBrowser.Source;
        }

        public void showing()
        {
                if (_NfcDetected)
            {
                knapp.IsEnabled = true;
            }
            else
            {
                knapp.IsEnabled = false;
            }
        }

        public void showingClosed()
        {
                knapp.IsEnabled = false;
        }

        private void webbLoaded(object sender, NavigationEventArgs e)
        {
            winBorder(0);
            Debug.WriteLine("sparade följande: " + AdressListan.LastCode);
        }
    }

    public class adresslista
    {

        Dictionary<string, string> tags
                = new Dictionary<string, string>{};

        private string lastCode;
        public string LastCode { get { return lastCode; } set { lastCode = value; } }
        private string goTo = "http://accessyoutube.org.uk/";
        public string GoTo { get { return goTo; } set { goTo = value; } }

        string filpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\NFC.dat";

        public Boolean exists(string x) 
        {
            Debug.WriteLine("i exists---------------------------------------------------------");
                if (x != null && tags.ContainsKey(x))
                {
                    Debug.WriteLine("levererar true i exists---------------------------------------------------------");
                    return true;
                }
            else
            {
                Debug.WriteLine("leverrerar false i exists---------------------------------------------------------");
                return false;
            }
        }

        public string varde(string text)
        {
            string goHit = null;

            if (text !=null && tags.TryGetValue(text, out goHit) && goHit != null)
            {
                
            //if (tags.TryGetValue(text, out goHit))
            //{
            //    if (!goHit.Equals(null))
            //    {
                    return goHit;
            //    }
            //    else return goTo;
            //}
            //else return goTo;
            }
            else return goTo;
        }

        public void remove(string key) 
        {
            tags.Remove(key);
        }

        public void adding(string key, string value) 
        {
            tags.Add(key, value);
        }

        public void readdictJson()
        {
            if (File.Exists(filpath))
            {
                try
                {
                    var dictionary = new JavaScriptSerializer()
    .Deserialize<Dictionary<string, string>>(File.ReadAllText(filpath));
                    
                    Debug.WriteLine("antal i dict" + dictionary.Count());
                    if (dictionary.Count() > 0)
                    {
                        tags = dictionary;
                    }
                    
                }
                catch (Exception)
                {
                    Debug.WriteLine("Fel på läsningen");
                }
            }
            else File.Create(filpath);
        }

        public void writedictJson()
        {
            try
            {
                File.WriteAllText(filpath, new JavaScriptSerializer().Serialize(tags));
            }
            catch (Exception)
            {
                Debug.WriteLine("fel på skrivningen");
            }
        }
    }
}
