using Diplom.Misc;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Diplom.TeacherFolder
{
    public partial class ViewVariantsWindow : Window
    {
        DiplomEntities entities = new DiplomEntities();
        public ViewVariantsWindow()
        {
            InitializeComponent();

            VariantsDG.ItemsSource = entities.Variants.ToList();
            TaskNumberCB.ItemsSource = entities.TaskNumbers.ToList();
            TaskTypeCB.ItemsSource = entities.TaskTypes.ToList();

            List<Task> tasks = new List<Task>(); 
            foreach (TasksVariant tasksVariant in entities.TasksVariants.Where(x => x.VariantId == ((Variant)VariantsDG.SelectedItem).ID))
            {
                tasks.Add(tasksVariant.Task);
            }
            TasksDG.ItemsSource = tasks;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entities.SaveChanges();
                TaskUpdate();
                MessageBox.Show("Задание сохранено!", "Сохранено");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить задание", "Неудача");
            }
        }
        private void TasksDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskUpdate(); //Метод обновляющий данные о задании в третьей части окна
        }
        
        private void TaskUpdate()
        {
            if (TasksDG.SelectedItem == null) { return; }
            ImagesSP.Children.Clear();
            FileHyperlink.Inlines.Clear();
            Task task;
            try
            {
                task = (Task)TasksDG.SelectedItem;
            }
            catch (Exception)
            {
                return;
            }
            
            TaskSP.DataContext = task;
            foreach (TaskImage image in task.TaskImages) //Создание кнопки для каждого изображения задания, и присвоение этой кнопке изображения и метода удаления изображения
            {
                Button button = new Button { DataContext = image, Height = ImageWorks.Display(image.Image).Height, Content = new Image { Source = ImageWorks.Display(image.Image) } }; //ImageWorks - класс для отображения изображений
                button.Click += new RoutedEventHandler(DeleteImage);
                ImagesSP.Children.Add(button);
            }
            if (task.TaskFiles.Count() > 0) //Если есть, добавление ссылки на файл
            {
                string fileName = entities.TaskFiles.First(x => x.TaskId == task.ID).FileName;
                FileHyperlink.Inlines.Add(fileName);
            }

            switch (task.TaskTypeId) //Удаление поля ответа в зависимости от типа задания
            {
                case 1:
                    if (!AnswerSPContainer.Children.Contains(AnswerSP))
                        AnswerSPContainer.Children.Add(AnswerSP);
                    break;
                case 2:
                    AnswerSPContainer.Children.Remove(AnswerSP);
                    task.Answer = null;
                    break;
            }
        }
        private void TaskTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskTypeCB.SelectedItem == null) { return; }
            switch (((TaskType)TaskTypeCB.SelectedItem).ID)
            {
                case 1:
                    if (!AnswerSPContainer.Children.Contains(AnswerSP))
                        AnswerSPContainer.Children.Add(AnswerSP);
                    break;
                case 2:
                    AnswerSPContainer.Children.Remove(AnswerSP);
                    break;
            }
        }
        private void AddImage(object sender, RoutedEventArgs e) //Кнопка добавления картинки
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Image files: *.jpg, *.png|*.jpg; *.png"
            };
            openFile.ShowDialog();
            string fileName = openFile.FileName;
            if (fileName != "")
            {
                byte[] img = File.ReadAllBytes(fileName);
                Task task = (Task)TasksDG.SelectedItem;
                task.TaskImages.Add(new TaskImage { TaskId = task.ID, Image = img });
            }
            TaskUpdate();
        }
        private void DeleteImage(object sender, RoutedEventArgs e) //Метод удаления картинки
        {
            if (MessageBox.Show("Вы действительно хотите удалить изображение?", 
                "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                Button button = (Button)sender;
                TaskImage curImage = (TaskImage)button.DataContext;
                entities.TaskImages.Remove(curImage);
            }
            TaskUpdate();
        }
        private void VariantsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskSP.DataContext = null;
            TasksDG.ItemsSource = null;
            ImagesSP.Children.Clear();
            FileHyperlink.Inlines.Clear();
            if (VariantsDG.SelectedItem == null) { return; }
            Variant selectedVariant;
            try
            {
                selectedVariant = (Variant)VariantsDG.SelectedItem;
            }
            catch (Exception)
            {
                return;
            }
            List<Task> tasks = new List<Task>();
            foreach (TasksVariant tasksVariant in entities.TasksVariants.Where(x => x.VariantId == selectedVariant.ID)) //Добавление каждого задания в варианте в список заданий
            {
                tasks.Add(tasksVariant.Task);
            }
            TasksDG.ItemsSource = tasks; //Присвоение списка заданий текущего варианта источнику данных таблицы заданий
        }

        private void VariantsDG_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Variant newVariant = new Variant { VariantTitle = "Вариант " + (entities.Variants.Count() + 1) };
            entities.Variants.Add(newVariant);
            entities.SaveChanges();
            e.NewItem = newVariant;
        }

        private void TasksDG_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            Variant currentVariant = (Variant)VariantsDG.SelectedItem;
            Task newTask = new Task();
            int curTask = entities.TasksVariants.Where(x => x.VariantId == currentVariant.ID).Count();
            if (curTask >= 12 && curTask <= 14)
            {
                newTask.TaskTypeId = 2;
            }
            else
            {
                newTask.TaskTypeId = 1;
            }
            if (curTask < 15)
            {
                newTask.TaskNumberId = entities.TasksVariants.Where(x => x.VariantId == currentVariant.ID).Count() + 1;
            }
            else
            {
                newTask.TaskNumberId = 1;
            }
            entities.Tasks.Add(newTask);
            entities.TasksVariants.Add(new TasksVariant { TaskId = newTask.ID, VariantId = currentVariant.ID });
            entities.SaveChanges();
            e.NewItem = newTask;
        }

        private void AddFileBtn_Click(object sender, RoutedEventArgs e) //Кнопка добавления файла
        {
            Task task = (Task)TasksDG.SelectedItem;

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            if (openFile == null) { return; }
            string fileName = openFile.FileName;
            FileInfo fileInfo = new FileInfo(fileName);
            string newPath = Directory.GetCurrentDirectory() + "/Дополнительные материалы/" + fileInfo.Name;
            if (!File.Exists(newPath)) { fileInfo.CopyTo(newPath); }
            if (task.TaskFiles.Count() > 0)
            {
                task.TaskFiles.First().FileName = Path.GetFileName(fileName);
            }
            else
            {
                task.TaskFiles.Add(new TaskFile { TaskId = task.ID, FileName = Path.GetFileName(fileName) });
            }
            entities.SaveChanges();

            TaskUpdate();
        }

        private void FileHyperlink_Click(object sender, RoutedEventArgs e) 
        {
            Run run = FileHyperlink.Inlines.First() as Run;
            try
            {
                Process.Start("Дополнительные материалы\\" + run.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Нет файла в доп. материалах", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeleteFileBtn_Click(object sender, RoutedEventArgs e) //Кнопка удаления файла
        {
            Task task = (Task)TasksDG.SelectedItem;

            if (task.TaskFiles.Count() > 0)
            {
                if (MessageBox.Show("Вы хотите удалить файл?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    entities.TaskFiles.Remove(task.TaskFiles.First());
                }
            }
            else
            {MessageBox.Show("Нет файла");}
            TaskUpdate();
        }

        private void TasksDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DataGrid grid = (DataGrid)sender;
                if (MessageBox.Show("Вы действительно хотите удалить эту строку?","Удаление", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    entities.TasksVariants.Remove(entities.TasksVariants.First(x => x.TaskId == ((Task)grid.SelectedItem).ID));
                    entities.Tasks.Remove((Task)grid.SelectedItem);
                    entities.SaveChanges();
                }
            }
        }
    }
}
