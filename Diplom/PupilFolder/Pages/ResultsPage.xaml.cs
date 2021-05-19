using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom.PupilFolder.Pages
{
    public partial class ResultsPage : Page
    {
        Variant currentVariant;
        User currentUser;
        bool mode;
        int preresult;
        int result;
        int maxResult = 0;

        public ResultsPage(Variant currentVariant, User currentUser, bool mode, int preresult)
        {
            InitializeComponent();

            this.currentVariant = currentVariant;
            this.currentUser = currentUser;
            this.mode = mode;
            this.preresult = preresult;

            foreach (TasksVariant taskVariant in currentVariant.TasksVariants)
            {
                if (taskVariant.Task.TaskTypeId == 1)
                {
                    maxResult++;
                }
                else if (taskVariant.Task.TaskTypeId == 2)
                {
                    if (taskVariant.Task.TaskNumber.Number == 14)
                    {
                        Task14ContentSP.Children.Add(new TextBlock { Text = taskVariant.Task.Description, TextWrapping = TextWrapping.Wrap });
                        maxResult += 3;
                    }
                    else
                    {
                        if (taskVariant.Task.TaskNumber.Number == 13)
                        {
                            Task13ContentSP.Children.Add(new TextBlock { Text = taskVariant.Task.Description, TextWrapping = TextWrapping.Wrap });
                        }
                        if (taskVariant.Task.TaskNumber.Number == 15)
                        {
                            Task15ContentSP.Children.Add(new TextBlock { Text = taskVariant.Task.Description, TextWrapping = TextWrapping.Wrap });
                        }
                        maxResult += 2;
                    }
                }
            }

            OverallPoints.Text = preresult.ToString() + " из " + maxResult;
        }

        private void EndEvalBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mode) //Если был выбран режим контрольной, записывает результат в базу данных
            {
                DiplomEntities entities = new DiplomEntities();
                Pupil currentPupil = currentUser.Pupils.First();
                PupilsVariant pupilsVariant = entities.PupilsVariants.First(x => x.PupilId == currentPupil.ID && x.VariantId == currentVariant.ID);
                if (pupilsVariant == null)
                {
                    PupilsVariant newPupilsVariant = new PupilsVariant
                    {
                        PupilId = currentPupil.ID,
                        VariantId = currentVariant.ID,
                        Result = result + "/" + maxResult,
                        Completed = true
                    };
                    entities.PupilsVariants.Add(newPupilsVariant);
                }
                else
                {
                    pupilsVariant.Result = result + "/" + maxResult;
                }

                entities.SaveChanges();
            }

            if (MessageBox.Show($"Вы хотите завершить оценивание?\nИтоговое количество баллов {result} из {maxResult}.", "Завершение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Window.GetWindow(this).Title = "Выбор варианта";
                NavigationService.Navigate(new VariantsPage(currentUser));
            }
        }

        int RB13 = 0;
        int RB14 = 0;
        int RB15 = 0;

        private void PointsUpdate()
        {
            result = preresult + RB13 + RB14 + RB15;
            OverallPoints.Text = result.ToString() + " из " + maxResult;
        }
        private void RadioButtons13_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RB = (RadioButton)sender;
            RB13 = int.Parse(RB.Content.ToString());
            PointsUpdate();
        }
        private void RadioButtons14_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RB = (RadioButton)sender;
            RB14 = int.Parse(RB.Content.ToString());
            PointsUpdate();
        }
        private void RadioButtons15_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton RB = (RadioButton)sender;
            RB15 = int.Parse(RB.Content.ToString());
            PointsUpdate();
        }
    }
}
