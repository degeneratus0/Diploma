using System.Linq;
using System.Windows;

namespace Diplom.TeacherFolder
{
    public partial class ViewTeachersWindow : Window
    {
        DiplomEntities entities;
        public ViewTeachersWindow()
        {
            InitializeComponent();
            ShowTable();
        }
        private void ShowTable()
        {
            entities = new DiplomEntities();
            TeachersDG.ItemsSource = entities.Users.Where(x => x.Role.Title == "Учитель").OrderBy(x => x.FullName).ToList();
        }
        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            User newUser = new User { RoleId = 2 };
            entities.Users.Add(newUser);
            new AddEditTeacherWindow(newUser, entities).ShowDialog(); //Вызов окна редактирования и добавления учителей
            ShowTable(); //Метод обновляющий таблицу учителей
        }

        private void EditTeacher_Click(object sender, RoutedEventArgs e)
        {
            new AddEditTeacherWindow((User)TeachersDG.SelectedItem, entities).ShowDialog();
            ShowTable();
        }
        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            User row = (User)TeachersDG.SelectedItem;
            if (row == null)
            {
                MessageBox.Show("Выберите строку для удаления", "Удаление");
                return;
            }

            if (MessageBox.Show("Подтвердите удаление", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                entities.Users.Remove(row);
                entities.SaveChanges();
                ShowTable();
            }
        }
    }
}
