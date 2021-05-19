using System;
using System.Windows;

namespace Diplom.TeacherFolder
{
    public partial class AddEditTeacherWindow : Window
    {
        DiplomEntities entities;
        public AddEditTeacherWindow(User user, DiplomEntities entities)
        {
            InitializeComponent();
            this.entities = entities;
            this.DataContext = user;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e) //Кнопка "Сохранить"
        {
            try
            {
                entities.SaveChanges();
                Close();
            }
            catch (Exception)
            { MessageBox.Show("Не удалось добавить учителя", "Неудача", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) //Кнопка "Отмена"
        {Close();}
    }
}
