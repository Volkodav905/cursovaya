using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace catalogmemov
{
    public partial class MainWindow : Window
    {
        List<Memchik> memes = new List<Memchik>();
        List<Memchik> buferMemov = new List<Memchik>();

        static string fileName = "CatalogMemchicov.json";

        public MainWindow()
        {
            InitializeComponent();

            categoriesMemesov.Items.Add("all");

            if (File.Exists(fileName))
            {
                List<Memchik> readed_memes = JsonSerializer.Deserialize<List<Memchik>>(File.ReadAllText(fileName));

                foreach (Memchik mem in readed_memes)
                {
                    memes.Add(mem);

                    listMemesov.Items.Add(mem.Name);

                    if (!(categoriesMemesov.Items.Contains(mem.Category)))
                        categoriesMemesov.Items.Add(mem.Category);

                }
            }
        }

        private void skachatMemchikSCompa_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (!(bool)dlg.ShowDialog())
                return;

            Uri fileUri = new Uri(dlg.FileName);

            dobavlenie_s_kompa add_mem_wnd = new dobavlenie_s_kompa();

            if (add_mem_wnd.ShowDialog() == true)
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(dlg.FileName);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                Memchik mem = new Memchik(add_mem_wnd.add_name_meme.Text, base64ImageRepresentation, add_mem_wnd.add_tag_meme.Text);

                memes.Add(mem);
                listMemesov.Items.Add(mem.Name);

                if (!(categoriesMemesov.Items.Contains(mem.Category)))
                    categoriesMemesov.Items.Add(mem.Category);
            }


        }

        static ImageSource ByteToImage(byte[] imageData)
        {
            var bitmap = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            return (ImageSource)bitmap;
        }

        private void sohranitMemchiki_Click(object sender, RoutedEventArgs e)
        {
            string jsonString = JsonSerializer.Serialize(memes);
            File.WriteAllText(fileName, jsonString);
        }

        private void listMemesov_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((categoriesMemesov.SelectedIndex == 0) && (poisMmemaPoImeni.Text.Length == 0 && poisMmemaPoTaguField.Text.Length == 0))
            {
                if (listMemesov.SelectedIndex != -1)
                    meme_img.Source = ByteToImage(Convert.FromBase64String(memes[listMemesov.SelectedIndex].Img));
            }
            else
                if (listMemesov.SelectedIndex != -1)
                meme_img.Source = ByteToImage(Convert.FromBase64String(buferMemov[listMemesov.SelectedIndex].Img));
        }

        private void udalitMemchik_Click(object sender, RoutedEventArgs e)
        {
            if (listMemesov.SelectedIndex != -1 && listMemesov.Items.Count == memes.Count)
            {
                memes.Remove(memes[listMemesov.SelectedIndex]);
                listMemesov.Items.Clear();

                foreach (Memchik mem in memes)
                    listMemesov.Items.Add(mem.Name);
            }
            else
                MessageBox.Show("Select category all");
        }

        private void findMemchik_Click(object sender, RoutedEventArgs e)
        {
            listMemesov.Items.Clear();
            buferMemov.Clear();
            foreach (Memchik mem in memes)
            {
                if (mem.Name.ToLower().Contains(poisMmemaPoImeni.Text.ToLower()))
                {
                    listMemesov.Items.Add(mem.Name);
                    buferMemov.Add(mem);
                }
            }
        }

        private void categoriesMemesov_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesMemesov.SelectedIndex != -1)
            {
                listMemesov.Items.Clear();
                if (categoriesMemesov.SelectedItem.ToString().Equals("all"))
                {
                    foreach (Memchik mem in memes)
                        listMemesov.Items.Add(mem.Name);
                    return;
                }

                buferMemov.Clear();

                foreach (Memchik mem in memes)
                {
                    if (mem.Category == categoriesMemesov.SelectedItem.ToString())
                    {
                        listMemesov.Items.Add(mem.Name);
                        buferMemov.Add(mem);
                    }
                }
            }
        }

        private void skachatMemchikSSsilki_Click(object sender, RoutedEventArgs e)
        {
            dobavlenie_s_ssilki add_url_mem_wnd = new dobavlenie_s_ssilki();

            if (add_url_mem_wnd.ShowDialog() == true)
            {
                WebClient client = new WebClient();

                string imageUrl = add_url_mem_wnd.url.Text;

                byte[] imageArray = client.DownloadData(imageUrl);

                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                Memchik mem = new Memchik(add_url_mem_wnd.name.Text, base64ImageRepresentation, add_url_mem_wnd.tag.Text);

                memes.Add(mem);
                listMemesov.Items.Add(mem.Name);

                if (!(categoriesMemesov.Items.Contains(mem.Category)))
                    categoriesMemesov.Items.Add(mem.Category);
            }
        }

        private void poiskMmemaPoTagu_Click(object sender, RoutedEventArgs e)
        {
            listMemesov.Items.Clear();
            buferMemov.Clear();
            foreach (Memchik mem in memes)
            {
                foreach (string tag in mem.Tags)
                {
                    if (tag.ToLower().Equals(poisMmemaPoTaguField.Text.ToLower()))
                    {
                        listMemesov.Items.Add(mem.Name);
                        buferMemov.Add(mem);
                    }
                }
            }
        }

        private void dobavitTagMemchiku_Click(object sender, RoutedEventArgs e)
        {
            if (listMemesov.SelectedIndex != -1 && listMemesov.Items.Count == memes.Count && dobavitTagMemu.Text.Length > 0)
            {
                memes[listMemesov.SelectedIndex].add_tag(dobavitTagMemu.Text);
            }
            else
                MessageBox.Show("Select category all or fill in the tag field");
        }
    }
}
