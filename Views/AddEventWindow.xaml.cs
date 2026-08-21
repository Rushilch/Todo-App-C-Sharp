using System.Windows;
using ProductivityApp.Data.Models;

namespace ProductivityApp.Views
{
    public partial class AddEventWindow : Window
    {
        public CalendarEvent? ResultEvent { get; private set; }

        public AddEventWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ResultEvent = new CalendarEvent
            {
                Title = TxtTitle.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
