using Diplom.PupilFolder.Pages;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    public partial class VariantsPage : Page
    {
        readonly DiplomEntities entities = new DiplomEntities();
        User currentUser;
        public VariantsPage(User user)
        {
            InitializeComponent();
            PupilNameTB.Text = user.FullName;
            PupilClassTB.Text = user.Pupils.First().Class.Title;
            currentUser = user;
            Pupil currentPupil = user.Pupils.FirstOrDefault();

            PupilsVariantsDG.ItemsSource = entities.PupilsVariants.Where(x => x.PupilId == currentPupil.ID).ToList();
            VariantsDG.ItemsSource = entities.Variants.ToList();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e) //Кнопка выхода
        {
            new AuthWindow().Show();
            Window.GetWindow(this).Close();
        }

        private void TrainingBtn_Click(object sender, RoutedEventArgs e) //Кнопка, сменяющая текущую страницу на страницу теста в режиме подготовки
        {
            Button button = (Button)sender;
            Window.GetWindow(this).Title = "Подготовка";
            NavigationService.Navigate(new TestPage((Variant)button.DataContext, currentUser, false));
        }

        private void ControlBtn_Click(object sender, RoutedEventArgs e) //Кнопка, сменяющая текущую страницу на страницу теста в режиме контрольной
        {
            Button button = (Button)sender;
            Window.GetWindow(this).Title = "Контрольная";
            NavigationService.Navigate(new TestPage((Variant)button.DataContext, currentUser, true));
        }
    }
}
