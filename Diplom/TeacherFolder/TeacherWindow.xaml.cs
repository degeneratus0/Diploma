using Diplom.TeacherFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    public partial class TeacherWindow : Window
    {
        List<User> pupils = new List<User>();
        DiplomEntities entities;
        public TeacherWindow()
        {
            InitializeComponent();
            entities = new DiplomEntities();
            pupils = entities.Users.Where(x => x.Role.Title == "Ученик").ToList();
            ClassSelectionCB.ItemsSource = entities.Classes.ToList();
            ClassSelectionCB.SelectedIndex = 0;
            ShowTable();
        }
        private void ShowTable()
        {
            entities = new DiplomEntities();
            Class selectedClass = (Class)ClassSelectionCB.SelectedItem; //Получение выбранного класса из поля выбора класса
            pupils = entities.Users.Where(x => x.Role.Title == "Ученик" && x.Pupils.FirstOrDefault().Class.ID == selectedClass.ID).ToList(); //Присваивание списку учеников всех пользователей с ролью "ученик", находящихся в выбранном классе
            
            string seekName = SeekPupilTB.Text.ToLower(); //Получение текста из поля для поиска по имени
            if (seekName != null)
                pupils = pupils.Where(x => x.FullName.ToLower().StartsWith(seekName)).ToList(); //Присваивание списку учеников всех пользователей, имя которых начинается с заданного значения в списке учеников
            PupilsDG.ItemsSource = pupils.OrderBy(x => x.FullName); //Присваивание источника данных таблице учеников, отсортированных по имени
        }
        private void ClassSelectionCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PupilsVariantsDG.ItemsSource = null;
            ShowTable();
        }
        private void SeekPupilTB_TextChanged(object sender, TextChangedEventArgs e)
        {ShowTable();}
        private void PupilsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PupilsDG.SelectedItem != null) //Выбран ли в таблице ученик
            {PupilsVariantsDG.ItemsSource = entities.PupilsVariants.Where(x => x.Pupil.User.ID == ((User)((DataGrid)sender).SelectedItem).ID).ToList();} //Источник данных для таблицы, отображающей решенные варианты = решенные варианты определенного ученика
        }
        private void LogoutBtn_Click(object sender, RoutedEventArgs e) //Кнопка выхода
        {
            new AuthWindow().Show(); //Открытие окна авторизации
            GetWindow(this).Close(); //Закрытие текущего окна
        }
        private void ViewVariants_Click(object sender, RoutedEventArgs e) //Кнопка открывающая окно работы с вариантами
        {new ViewVariantsWindow().ShowDialog();}
        private void AddPupil_Click(object sender, RoutedEventArgs e) //Кнопка открывающая окно добавления ученика
        {
            Pupil newPupil = new Pupil { User = new User{ RoleId = 1 } }; //Создание экземпляра класса ученика, с присваиванием ему роли "ученика"
            entities.Pupils.Add(newPupil); //Добавление ученика в базу данных
            AddEditPupilWindow pupilWindow = new AddEditPupilWindow(newPupil, entities)
            {
                Title = "Добавление ученика"
            };
            pupilWindow.ShowDialog(); //Открытие окна добавления или редактирования ученика, и передача в него экземпляра нового ученика и текущего контекста данных
            ShowTable(); //Метод обновления таблицы на странице учителя
        }
        private void EditPupil_Click(object sender, RoutedEventArgs e) //Кнопка открывающая окно редактирования ученика
        {
            Pupil pupil;
            try
            {
                pupil = ((User)PupilsDG.SelectedItem).Pupils.First();
            }
            catch (Exception)
            { return; }
            AddEditPupilWindow pupilWindow = new AddEditPupilWindow(pupil/*Передача текущего ученика в окно добавления или редактирования*/, entities)
            {
                Title = "Редактирование ученика"
            };
            pupilWindow.ShowDialog();
            ShowTable();
        }
        private void DeletePupil_Click(object sender, RoutedEventArgs e) //Кнопка удаления ученика
        {
            User row = (User)PupilsDG.SelectedItem; //Получение выбранного в таблице ученика
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
        private void ViewTeachers_Click(object sender, RoutedEventArgs e) //Кнопка открывающая окно работы с учителями
        { new ViewTeachersWindow().Show();}

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            new ReportWindow().Show();
        }
    }
}
