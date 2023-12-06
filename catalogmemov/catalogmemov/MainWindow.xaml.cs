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
        List<Memchik> temp_memes = new List<Memchik>();

        static string fileName = "MemesCatalog.json";

        public MainWindow()
        {
            InitializeComponent();

            meme_categories.Items.Add("all");

            if (File.Exists(fileName))
            {
                List<Memchik> readed_memes = JsonSerializer.Deserialize<List<Memchik>>(File.ReadAllText(fileName));

                foreach (Memchik mem in readed_memes)
                {
                    memes.Add(mem);

                    meme_list.Items.Add(mem.Name);

                    if (!(meme_categories.Items.Contains(mem.Category)))
                        meme_categories.Items.Add(mem.Category);

                }
            }
        }

        private void meme_down_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (!(bool)dlg.ShowDialog())
                return;

            Uri fileUri = new Uri(dlg.FileName);

            Add_meme add_mem_wnd = new Add_meme();

            if (add_mem_wnd.ShowDialog() == true)
            {
                // перевод картинки в строку байт
                byte[] imageArray = System.IO.File.ReadAllBytes(dlg.FileName);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                Memchik mem = new Memchik(add_mem_wnd.add_name_meme.Text, base64ImageRepresentation, add_mem_wnd.add_tag_meme.Text);

                memes.Add(mem);
                meme_list.Items.Add(mem.Name);

                if (!(meme_categories.Items.Contains(mem.Category)))
                    meme_categories.Items.Add(mem.Category);
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

        private void memes_save_Click(object sender, RoutedEventArgs e)
        {
            string jsonString = JsonSerializer.Serialize(memes);
            File.WriteAllText(fileName, jsonString);
        }

        private void meme_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((meme_categories.SelectedIndex == -1 || meme_categories.SelectedIndex == 0) && (meme_find.Text.Length == 0 && meme_find_by_tag.Text.Length == 0))
            {
                if (meme_list.SelectedIndex != -1)
                    meme_img.Source = ByteToImage(Convert.FromBase64String(memes[meme_list.SelectedIndex].Img));
            }
            else
                if (meme_list.SelectedIndex != -1)
                meme_img.Source = ByteToImage(Convert.FromBase64String(temp_memes[meme_list.SelectedIndex].Img));
        }

        private void meme_del_Click(object sender, RoutedEventArgs e)
        {
            if (meme_list.SelectedIndex != -1 && meme_list.Items.Count == memes.Count)
            {
                memes.Remove(memes[meme_list.SelectedIndex]);
                meme_list.Items.Clear();

                foreach (Memchik mem in memes)
                    meme_list.Items.Add(mem.Name);
            }
            else
                MessageBox.Show("Select category all");
        }

        private void find_mem_Click(object sender, RoutedEventArgs e)
        {
            meme_list.Items.Clear();
            temp_memes.Clear();
            foreach (Mem mem in memes)
            {
                if (mem.Name.ToLower().Contains(meme_find.Text.ToLower()))
                {
                    meme_list.Items.Add(mem.Name);
                    temp_memes.Add(mem);
                }
            }
        }

        private void meme_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (meme_categories.SelectedIndex != -1)
            {
                meme_list.Items.Clear();
                if (meme_categories.SelectedItem.ToString().Equals("all"))
                {
                    foreach (Memchik mem in memes)
                        meme_list.Items.Add(mem.Name);
                    return;
                }

                temp_memes.Clear();

                foreach (Memchik mem in memes)
                {
                    if (mem.Category == meme_categories.SelectedItem.ToString())
                    {
                        meme_list.Items.Add(mem.Name);
                        temp_memes.Add(mem);
                    }
                }
            }
        }

        private void meme_url_down_Click(object sender, RoutedEventArgs e)
        {
            Add_url_meme add_url_mem_wnd = new Add_url_meme();

            if (add_url_mem_wnd.ShowDialog() == true)
            {
                WebClient client = new WebClient();

                string imageUrl = add_url_mem_wnd.url.Text;

                byte[] imageArray = client.DownloadData(imageUrl);

                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                Memchik mem = new Memchik(add_url_mem_wnd.name.Text, base64ImageRepresentation, add_url_mem_wnd.tag.Text);

                memes.Add(mem);
                meme_list.Items.Add(mem.Name);

                if (!(meme_categories.Items.Contains(mem.Category)))
                    meme_categories.Items.Add(mem.Category);
            }
        }

        private void find_mem_by_tag_Click(object sender, RoutedEventArgs e)
        {
            meme_list.Items.Clear();
            temp_memes.Clear();
            foreach (Memchik mem in memes)
            {
                foreach (string tag in mem.Tags)
                {
                    if (tag.ToLower().Equals(meme_find_by_tag.Text.ToLower()))
                    {
                        meme_list.Items.Add(mem.Name);
                        temp_memes.Add(mem);
                    }
                }
            }
        }

        private void add_tag_Click(object sender, RoutedEventArgs e)
        {
            if (meme_list.SelectedIndex != -1 && meme_list.Items.Count == memes.Count && meme_add_tag.Text.Length > 0)
            {
                memes[meme_list.SelectedIndex].add_tag(meme_add_tag.Text);
            }
            else
                MessageBox.Show("Select category all or fill in the tag field");
        }
    }
}
