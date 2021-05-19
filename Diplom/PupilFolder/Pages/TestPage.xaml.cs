using Diplom.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Diplom.PupilFolder.Pages
{
    public partial class TestPage : Page
    {
        Dictionary<int, string> answers { get; set; }
        List<Task> tasks = new List<Task>();

        int currentTask = 1;
        User currentUser;
        Variant currentVariant;
        bool mode;

        public TestPage(Variant selected, User currentUser, bool mode)
        {
            InitializeComponent();

            currentVariant = selected;
            this.currentUser = currentUser;
            this.mode = mode;
            answers = new Dictionary<int, string>();

            foreach (TasksVariant t in selected.TasksVariants)
            {
                Button button = new Button { Content = t.Task.TaskNumber.Number, Height = 31, Width = 30 };
                button.Click += new RoutedEventHandler(CurTaskBtn);
                TaskSwitchSP.Children.Add(button);
                tasks.Add(t.Task);
            }
            if (mode)
            {
                TaskGrid.Children.Remove(TaskSolutionSP);
                AnswerDP.Children.Remove(ViewSolutionBtn);
            }
            TaskSwitch(currentTask);
        }
        private void CurTaskBtn(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            currentTask = (int)button.Content;
            TaskSwitch(currentTask);
        }
        private void TaskSwitch(int curTask)
        {
            TaskSolutionSP.Children.Clear();
            TaskContentSP.Children.Clear();
            Task task = tasks.FirstOrDefault(x => x.TaskNumber.Number == curTask);
            TaskSolutionSP.Children.Add(new TextBlock { Text = task.Solution, TextWrapping = TextWrapping.Wrap });
            TaskContentSP.Children.Add(new TextBlock { Text = "Задание " + curTask });
            TaskContentSP.Children.Add(new TextBlock { Text = task.Description, TextWrapping = TextWrapping.Wrap });
            if (task.TaskImages != null)
            {
                StackPanel ImagesSP = new StackPanel { Orientation = Orientation.Horizontal };
                TaskContentSP.Children.Add(ImagesSP);
                foreach (TaskImage image in task.TaskImages)
                {
                    ImagesSP.Children.Add(new Image { Height = ImageWorks.Display(image.Image).Height, Source = ImageWorks.Display(image.Image) });
                }
            }
            if (task.TaskFiles != null)
            {
                foreach (TaskFile file in task.TaskFiles)
                {
                    Hyperlink hp = new Hyperlink();
                    hp.Inlines.Add(file.FileName);
                    hp.Click += new RoutedEventHandler(FileHyperlink_Click); 
                    TextBlock tb = new TextBlock();
                    tb.Inlines.Add(hp);
                    TaskContentSP.Children.Add(tb);
                }
            }
            switch (task.TaskTypeId)
            {
                case 1:
                    AnswerText.Text = "Введите ответ:";
                    if (!AnswerSP.Children.Contains(AnswerTB))
                        AnswerSP.Children.Add(AnswerTB);
                    WriteAnswerBtn.Content = "Ответить";
                    break;
                case 2:
                    AnswerText.Text = "Развернутые задания будет предложено оценить на странице результатов";
                    AnswerSP.Children.Remove(AnswerTB);
                    WriteAnswerBtn.Content = "Далее";
                    break;
            }
        }
        private void FileHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Run run = ((Hyperlink)sender).Inlines.First() as Run;
            try
            {
                Process.Start("Дополнительные материалы\\" + run.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Нет файла в доп. материалах", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);   
            }
        }
        private void ViewSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TaskSolutionSP.Visibility == Visibility.Visible)
            {
                SolutionColumn.Width = new GridLength(0);
                TaskSolutionSP.Visibility = Visibility.Collapsed;
                ViewSolutionBtn.Content = "Показать решение";
            }
            else
            {
                SolutionColumn.Width = new GridLength(0.5, GridUnitType.Star);
                TaskSolutionSP.Visibility = Visibility.Visible;
                ViewSolutionBtn.Content = "Скрыть решение";
            }
        }
        public void WriteAnswerBtn_Click(object sender, RoutedEventArgs e) //Кнопка "Ответить"
        {
            answers[currentTask] = AnswerTB.Text;
            AnswerTB.Text = null;

            if (currentTask < tasks.Count())
                currentTask++;
            else if (currentTask == tasks.Count())
                FinishTestBtn_Click(sender, e); //Вызов метода пункта "Завершить решение" если задание последнее

            TaskSwitch(currentTask);
        }

        private void FinishTestBtn_Click(object sender, RoutedEventArgs e) //Пункт "Завершить решение"
        {
            int result = 0;
            foreach (Task t in tasks)
            {
                try
                {
                    if (answers[t.TaskNumber.Number] == t.Answer)
                    {
                        result++;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            MessageBoxResult messageBoxResult = MessageBoxResult.None;
            switch (mode)
            {
                case true:
                    messageBoxResult = MessageBox.Show("Вы хотите завершить решение?\nНа следующей странице учителю необходимо оценить развернутые задания.",
                "Результат", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    break;
                case false:
                    messageBoxResult = MessageBox.Show("Вы хотите завершить решение?\nНа следующей странице будет предложено оценить развернутые задания.",
                "Результат", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    break;
            }

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (mode)
                {
                    AuthWindow auth = new AuthWindow(false); //Авторизация
                    auth.ShowDialog();
                    if (auth.AuthorizationResult() != null)
                    {
                        User authorized = auth.AuthorizationResult();
                        switch (authorized.Role.Title)
                        {
                            case "Ученик":
                                MessageBox.Show("Авторизацию должен пройти учитель", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                            case "Учитель":
                                Window.GetWindow(this).Title = "Результаты";
                                NavigationService.Navigate(new ResultsPage(currentVariant, currentUser, mode, result)); //Смена страницы на страницу результатов
                                break;
                        }
                    }
                }
                else
                {
                    Window.GetWindow(this).Title = "Результаты";
                    NavigationService.Navigate(new ResultsPage(currentVariant, currentUser, mode, result)); //Смена страницы на страницу результатов
                }
            }
        }
    }
}
