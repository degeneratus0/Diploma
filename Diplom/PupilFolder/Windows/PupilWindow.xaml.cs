using System.Windows;

namespace Diplom
{
    public partial class PupilWindow : Window
    {
        public PupilWindow(User pupil)
        {
            InitializeComponent();
            PupilFrame.Navigate(new VariantsPage(pupil));
        }
    }
}
