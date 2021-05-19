using System.Linq;
using System.Windows;

namespace Diplom
{
    public partial class AuthWindow : Window
    {
        readonly DiplomEntities entities = new DiplomEntities(); //Экземпляр модели базы данных
        bool mode = true;
        public AuthWindow() //Конструктор окна по умолчанию
        {
            InitializeComponent();
        }
        public AuthWindow(bool mode) //Конструктор окна
        {
            InitializeComponent();
            this.mode = mode;
        }
        private void Authorize_Click(object sender, RoutedEventArgs e) //Код кнопки "Вход"
        {
            if (mode)
            {
                User authorized = AuthorizationResult();
                switch (authorized.Role.Title) //Открытие окна в зависимости от роли найденного пользователя
                {
                    case "Ученик":
                        new PupilWindow(authorized).Show();
                        break;
                    case "Учитель":
                        new TeacherWindow().Show();
                        break;
                }
            }
            Close();
        }
        public User AuthorizationResult()
        {
            User authorized = entities.Users.FirstOrDefault(x => x.Login == TBLogin.Text && x.Password == TBPass.Password); //Поиск пользователя по данным из полей логина и пароля с помощью LINQ-запроса к экземпляру модели базы данных
            if (authorized == null) //Если пользователь не найден
            {
                MessageBox.Show("Неверный пароль или логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
            return authorized;
        }
        private void Exit_Click(object sender, RoutedEventArgs e) //Код кнопки "Выход"
        {
            Close();
        }
    }
}
