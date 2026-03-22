using Microsoft.Win32;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Linq.Expressions;




namespace WpfVcardEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string? _currentFile = null;

        private bool _hasChanges = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory =Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.Filter = "Afbeeldingen|*jpg;*.jpeg";

            if (dialog.ShowDialog() == true) 
            {
                tblPhotoName.Text = dialog.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                bitmap.EndInit();
                imgPhoto.Source = bitmap;
            }

        }

        private void Card_Changed(object sender, EventArgs e) {

            _hasChanges = true;
            UpdatePercentage();
        }


        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            if (_hasChanges) 
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn onopgeslagen wijzigingen. ben je zeker dat je wil doorgaan?",
                    "Nieuwe kaart",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.OK)
                    return;
            }

            ClearFields();
            _currentFile = null;
            _hasChanges = false;
            mnuSave.IsEnabled = false;
            tblStatus.Text = "huidige kaart: (geen geopend)";
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "Vcard bestanden|*.vcf";

            if (dialog.ShowDialog() == true)
            {

                string filePath = dialog.FileName;

                try
                {
                    string[] regels = File.ReadAllLines(filePath);

                    ClearFields();
                    tblPercentage.Text = "percentage ingevuld: n.a";
                    UpdatePercentage();

                    foreach (string regel in regels)
                    {
                        if (regel.StartsWith("N;"))
                        {

                            string waarde = regel.Substring(regel.IndexOf(":") + 1);
                            string[] delen = waarde.Split(';');
                            if (delen.Length > 0) txtLastname.Text = delen[0];
                            if (delen.Length > 1) txtFirstname.Text = delen[1];
                        }
                        else if (regel.StartsWith("BDAY"))
                        {
                            string waarde = regel.Substring(5);
                            if (DateTime.TryParseExact(waarde, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                                dateBirthday.SelectedDate = dt;
                        }

                        else if (regel.StartsWith("GENDER"))
                        {
                            string waarde = regel.Substring(7);
                            if (waarde == "M") radMan.IsChecked = true;
                            else if (waarde == "F") radVrouw.IsChecked = true;
                            else radOnbekend.IsChecked = true;
                        }


                        else if (regel.StartsWith("EMAIL") && regel.Contains("HOME"))
                        {
                            txtEmailHome.Text = regel.Substring(regel.IndexOf(":") + 1);
                        }


                        else if (regel.StartsWith("EMAIL") && regel.Contains("WORK"))
                        {
                            txtEmailWork.Text = regel.Substring(regel.IndexOf(":") + 1);
                        }


                        else if (regel.StartsWith("TEL") && regel.Contains("HOME"))
                        {
                            txtPhoneHome.Text = regel.Substring(regel.IndexOf(":") + 1);
                        }


                        else if (regel.StartsWith("TEL") && regel.Contains("WORK"))
                        {
                            txtPhoneWork.Text = regel.Substring(regel.IndexOf(":") + 1);
                        }

                        else if (regel.StartsWith("ORG"))
                        {
                            txtOrg.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }

                        else if (regel.StartsWith("TITLE"))
                        {
                            txtTitle.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }

                        else if (regel.StartsWith("X-SOCIALPROFILE") && regel.Contains("linkedin"))
                        {
                            txtLinkedIn.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }

                        else if (regel.StartsWith("X-SOCIALPROFILE") && regel.Contains("facebook"))
                        {
                            txtFacebook.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }

                        else if (regel.StartsWith("X-SOCIALPROFILE") && regel.Contains("instagram"))
                        {
                            txtInstagram.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }

                        else if (regel.StartsWith("X-SOCIALPROFILE") && regel.Contains("youtube"))
                        {
                            txtYoutube.Text = regel.Substring(regel.IndexOf(':') + 1);
                        }





                    }



                    _currentFile = filePath;
                    _hasChanges = false;
                    mnuSave.IsEnabled = true;
                    tblStatus.Text = $" huidige kaart:{filePath}";

                }






                catch (FileNotFoundException)
                {
                    MessageBox.Show($"Bestand {filePath} niet gevonden.", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show($" Kan Bestand {filePath} niet lezen.", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($" Onbekende fout:  {ex.Message}", "FOUT", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }


        

        private void ClearFields()
        {

            txtFirstname.Text = "";
            txtLastname.Text = "";
            dateBirthday.SelectedDate = null;
            radOnbekend.IsChecked = true;
            txtEmailHome.Text = "";
            txtPhoneHome.Text = "";
            txtEmailWork.Text = "";
            txtPhoneWork.Text = "";
            txtOrg.Text = "";
            txtTitle.Text = "";
            txtLinkedIn.Text = "";
            txtFacebook.Text = "";
            txtInstagram.Text = "";
            txtYoutube.Text = "";
            imgPhoto.Source = null;
            tblPhotoName.Text = "(geen geselecteerd)";
            

        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveCard(_currentFile);
            MessageBox.Show("Bestand opgeslagen!", "Opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void mnuSaveAs_Click(object sender , RoutedEventArgs e) 
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "Vcard bestanden|*vcf";
            dialog.FileName = "card.vcf";

            if(dialog.ShowDialog() == true)
            {
                _currentFile = dialog.FileName;
                SaveCard(_currentFile);
                mnuSave.IsEnabled = true;
                tblStatus.Text = $"huidige kaart: {_currentFile}";
            }

        }

        private void mnuExit_Click(object sender, RoutedEventArgs e) 
        {
            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je de aplicatie wil afsluiten?",
                "Toepassing sluiten", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK) { 
            Application.Current.Shutdown();
            
            }    
        }

        private void SaveCard(string filePath) 
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");

            if(!string.IsNullOrEmpty(txtFirstname.Text) || !string.IsNullOrEmpty(txtLastname.Text)) 
            {
                sb.AppendLine($"FN;CHARSET=UTF-8:{txtLastname.Text} {txtFirstname.Text}");
                sb.AppendLine($"N;CHARSET=UTF-8:{txtLastname.Text} ; {txtFirstname.Text};;;");

            }

            if (dateBirthday.SelectedDate != null)
                sb.AppendLine($"BDAY:{dateBirthday.SelectedDate.Value.ToString("yyyyMMdd")}");

            if (radMan.IsChecked == true) sb.AppendLine("GENDER:M");
            else if (radVrouw.IsChecked == true) sb.AppendLine("GENDER:F");

            if (!string.IsNullOrEmpty(txtEmailHome.Text))
                sb.AppendLine($"EMAIL;CHARSET=UTF8;type=HOME,INTERNET:{txtEmailHome.Text}");

            if (!string.IsNullOrEmpty(txtEmailWork.Text))
                sb.AppendLine($"EMAIL;CHARSET=UTF8;type=WORK,INTERNET:{txtEmailWork.Text}");

            if (!string.IsNullOrEmpty(txtPhoneHome.Text))
                sb.AppendLine($"TEL;TYPE=HOME,VOICE:{txtPhoneHome.Text}");

            if (!string.IsNullOrEmpty(txtPhoneWork.Text))
                sb.AppendLine($"TEL;TYPE=WORK,VOICE:{txtPhoneWork.Text}");

            if (!string.IsNullOrEmpty(txtOrg.Text))
                sb.AppendLine($"ORG;CHARSET=UTF-8:{txtOrg.Text}");

            if (!string.IsNullOrEmpty(txtTitle.Text))
                sb.AppendLine($"TITLE;CHARSET=UTF-8:{txtTitle.Text}");

            if (!string.IsNullOrEmpty(txtLinkedIn.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=linkedin:{txtLinkedIn.Text}");

            if (!string.IsNullOrEmpty(txtFacebook.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=facebook:{txtFacebook.Text}");

            if(!string.IsNullOrEmpty(txtInstagram.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=instagram:{txtInstagram.Text}");

            if (!string.IsNullOrEmpty(txtYoutube.Text))
                sb.AppendLine($"X-SOCIALPROFILE;TYPE=youtube:{txtYoutube.Text}");

            if(imgPhoto.Source != null)
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPhoto.Source));
                using (MemoryStream ms = new MemoryStream()) 
                {
                    encoder.Save(ms);
                    string base64 = Convert.ToBase64String(ms.ToArray());
                    sb.AppendLine($"PHOTO;ENCODING=b;TYPE=JPEG:{base64}");
                }

            }

            sb.AppendLine("END:VCARD");

            File.WriteAllText(filePath, sb.ToString());
            _hasChanges = false;




        }

        private void UpdatePercentage() {

            if (txtFirstname == null || txtEmailHome == null) return;

            int totaal = 9;
            int ingevuld = 0;

            if (!string.IsNullOrEmpty(txtFirstname.Text)) ingevuld++;
            if(!string.IsNullOrEmpty(txtLastname.Text)) ingevuld++;
            if (dateBirthday.SelectedDate != null) ingevuld++;
            if (radMan.IsChecked == true || radVrouw.IsChecked == true) ingevuld++;
            if(!string.IsNullOrEmpty(txtEmailHome.Text)) ingevuld++;
            if(!string.IsNullOrEmpty(txtPhoneHome.Text)) ingevuld++;
            if(!string.IsNullOrEmpty(txtOrg.Text)) ingevuld++;
            if(!string.IsNullOrEmpty(txtTitle.Text)) ingevuld++;
            if (imgPhoto.Source != null) ingevuld++;

            int percentage = (int)((double)ingevuld / totaal * 100);
            tblPercentage.Text = $"percentage ingevuld : {percentage}%";

        }
        

        private void mnuAbout_Click(object sender, RoutedEventArgs e) 
        { 
             AboutWindow about = new AboutWindow();
             about.Owner = this;
             about.ShowDialog();
        }

        
    }
}