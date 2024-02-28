using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace Pr_2
{
    public class Serializ
    {
        public static void Serialize<T>(IEnumerable<T> data, string filePath)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, jsonData);
        }

        public static IEnumerable<T> Deserialize<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonData);
            }
            return null;
        }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public Note(string title, string description, DateTime date)
        {
            Title = title;
            Description = description;
            Date = date;
        }
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        private Note selectedNote;

        public MainWindow()
        {
            InitializeComponent();
            kalendar.SelectedDate = DateTime.Today;
            note.ItemsSource = Notes;
            LoadNotes();
        }

        private void SaveNotes()
        {
            Serializ.Serialize(Notes, "notes.json");
        }

        private void LoadNotes()
        {
            Notes.Clear();
            IEnumerable<Note> loadedNotes = Serializ.Deserialize<Note>("notes.json");
            if (loadedNotes != null)
            {
                foreach (var note in loadedNotes)
                {
                    Notes.Add(note);
                }
                vibordati(null, null);
            }
        }

        private void vibordati(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = kalendar.SelectedDate ?? DateTime.Today;
            var filteredNotes = Notes.Where(n => n.Date.Date == date.Date).ToList();
            note.ItemsSource = filteredNotes;
        }

        private void sozdanie(object sender, RoutedEventArgs e)
        {
            string title = title_note.Text.Trim();
            string description = opisanie.Text.Trim();
            DateTime date = kalendar.SelectedDate ?? DateTime.Today;

            if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(description))
            {
                Note newNote = new Note(title, description, date);
                Notes.Add(newNote);
                vibordati(null, null);
                SaveNotes();
            }
        }

        private void deleteZametki(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                Notes.Remove(selectedNote);
                vibordati(null, null);
                SaveNotes();
            }
        }

        private void saveZametki(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                selectedNote.Title = title_note.Text;
                selectedNote.Description = opisanie.Text;
                note.Items.Refresh();
                SaveNotes();
            }
        }

        private void izmenenie(object sender, SelectionChangedEventArgs e)
        {
            selectedNote = note.SelectedItem as Note;
            if (selectedNote != null)
            {
                title_note.Text = selectedNote.Title;
                opisanie.Text = selectedNote.Description;
            }
        }
    }
}
