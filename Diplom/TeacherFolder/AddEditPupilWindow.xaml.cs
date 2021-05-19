using System;
using System.Linq;
using System.Windows;

namespace Diplom.TeacherFolder
{
    public partial class AddEditPupilWindow : Window
    {
        DiplomEntities entities;
        public AddEditPupilWindow(Pupil pupil, DiplomEntities entities)
        {
            InitializeComponent();
            this.entities = entities;
            this.DataContext = pupil;

            ClassCB.ItemsSource = entities.Classes.ToList();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e) //Кнопка "Сохранить"
        {
            try
            {
                entities.SaveChanges();
                Close();
            }
            catch (Exception)
            { MessageBox.Show("Не удалось добавить ученика", "Неудача", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e) //Кнопка "Отмена"
        { Close();}
    }
}
