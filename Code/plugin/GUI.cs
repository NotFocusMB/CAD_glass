using Core;
using GlassPlugin;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace glass_plugin
{
    /// <summary>
    /// Главная форма пользовательского интерфейса для плагина построения бокала.
    /// </summary>
    public partial class GUI : Form
    {
        // Экземпляр класса, отвечающего за построение 3D-модели.
        private GlassBuilder _builder;
        // Экземпляр класса, управляющего параметрами и их зависимостями.
        private Parameters _parameters;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public GUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// Инициализирует объекты параметров и построителя, а также сбрасывает UI к начальным значениям.
        /// </summary>
        private void GUI_Load(object sender, EventArgs e)
        {
            _parameters = new Parameters();
            _builder = new GlassBuilder();
            ResetFormToDefaults();
        }

        /// <summary>
        /// Обновляет все текстовые метки (Label) на форме, отображающие
        /// актуальные динамические ограничения для каждого параметра.
        /// </summary>
        private void UpdateAllLimitLabels()
        {
            StalkHeightLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.StalkHeight);
            SideHeightLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.SideHeight);
            BowlRadiusLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.BowlRadius);
            StalkRadiusLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.StalkRadius);
            SideAngleLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.SideAngle);
            StandRadiusLimitsLabel.Text = _parameters.GetLimitsString(ParameterType.StandRadius);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Создать".
        /// Проверяет корректность всех введенных данных и запускает процесс построения модели.
        /// </summary>
        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (CheckAll())
            {
                _builder.BuildGlass(_parameters);
                MessageBox.Show("Построение бокала начато!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, исправьте все поля, выделенные красным!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Проверяет, что все поля ввода на форме содержат корректные значения.
        /// </summary>
        /// <returns>True, если все поля валидны; иначе False.</returns>
        private bool CheckAll()
        {
            // Простая и надежная проверка: если ни одно поле не подсвечено красным, все в порядке.
            return StalkHeightTextBox.BackColor == Color.White &&
                   SideHeightTextBox.BackColor == Color.White &&
                   BowlRadiusTextBox.BackColor == Color.White &&
                   StalkRadiusTextBox.BackColor == Color.White &&
                   SideAngleTextBox.BackColor == Color.White &&
                   StandRadiusTextBox.BackColor == Color.White;
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Высота ножки".
        /// </summary>
        private void StalkHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StalkHeight];
            if (double.TryParse(StalkHeightTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StalkHeightTextBox.BackColor = Color.White;
                ErrorToolTip1.Hide(StalkHeightTextBox);
                _parameters.SetDependencies(value, ParameterType.StalkHeight);
                UpdateAllLimitLabels(); // Обновляем все метки с ограничениями
            }
            else
            {
                // Если значение невалидно, подсвечиваем поле и показываем подсказку.
                StalkHeightTextBox.BackColor = Color.LightCoral;
                ErrorToolTip1.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StalkHeightTextBox, 5000);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Высота стенки".
        /// </summary>
        private void SideHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.SideHeight];
            if (double.TryParse(SideHeightTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                SideHeightTextBox.BackColor = Color.White;
                ErrorToolTip2.Hide(SideHeightTextBox);
                _parameters.SetDependencies(value, ParameterType.SideHeight);
                UpdateAllLimitLabels();
            }
            else
            {
                SideHeightTextBox.BackColor = Color.LightCoral;
                ErrorToolTip2.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", SideHeightTextBox, 5000);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Радиус чаши".
        /// </summary>
        private void BowlRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.BowlRadius];
            if (double.TryParse(BowlRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                BowlRadiusTextBox.BackColor = Color.White;
                ErrorToolTip3.Hide(BowlRadiusTextBox);
                _parameters.SetDependencies(value, ParameterType.BowlRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                BowlRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip3.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", BowlRadiusTextBox, 5000);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Радиус ножки".
        /// </summary>
        private void StalkRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StalkRadius];
            if (double.TryParse(StalkRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StalkRadiusTextBox.BackColor = Color.White;
                ErrorToolTip4.Hide(StalkRadiusTextBox);
                _parameters.SetDependencies(value, ParameterType.StalkRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                StalkRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip4.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StalkRadiusTextBox, 5000);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Наклон стенки".
        /// </summary>
        private void SideAngleTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.SideAngle];
            if (double.TryParse(SideAngleTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                SideAngleTextBox.BackColor = Color.White;
                ErrorToolTip5.Hide(SideAngleTextBox);
                _parameters.SetDependencies(value, ParameterType.SideAngle);
                UpdateAllLimitLabels();
            }
            else
            {
                SideAngleTextBox.BackColor = Color.LightCoral;
                ErrorToolTip5.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", SideAngleTextBox, 5000);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста в поле "Радиус основания".
        /// </summary>
        private void StandRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StandRadius];
            if (double.TryParse(StandRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StandRadiusTextBox.BackColor = Color.White;
                ErrorToolTip6.Hide(StandRadiusTextBox);
                _parameters.SetDependencies(value, ParameterType.StandRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                StandRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip6.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StandRadiusTextBox, 5000);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сброс".
        /// </summary>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetFormToDefaults();
        }

        /// <summary>
        /// Сбрасывает модель и UI к начальным значениям по умолчанию.
        /// </summary>
        private void ResetFormToDefaults()
        {
            // Сбрасываем модель данных к "заводским" настройкам.
            _parameters.ResetToDefaults();

            // Устанавливаем текст в полях ввода из сброшенной модели.
            StalkHeightTextBox.Text = _parameters.NumericalParameters[ParameterType.StalkHeight].Value.ToString("F1");
            SideHeightTextBox.Text = _parameters.NumericalParameters[ParameterType.SideHeight].Value.ToString("F1");
            BowlRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.BowlRadius].Value.ToString("F1");
            StalkRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.StalkRadius].Value.ToString("F1");
            SideAngleTextBox.Text = _parameters.NumericalParameters[ParameterType.SideAngle].Value.ToString("F1");
            StandRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.StandRadius].Value.ToString("F1");

            // Обновляем все метки с ограничениями.
            UpdateAllLimitLabels();

            // Убираем все красные подсветки и прячем все всплывающие подсказки.
            StalkHeightTextBox.BackColor = Color.White; ErrorToolTip1.Hide(StalkHeightTextBox);
            SideHeightTextBox.BackColor = Color.White; ErrorToolTip2.Hide(SideHeightTextBox);
            BowlRadiusTextBox.BackColor = Color.White; ErrorToolTip3.Hide(BowlRadiusTextBox);
            StalkRadiusTextBox.BackColor = Color.White; ErrorToolTip4.Hide(StalkRadiusTextBox);
            SideAngleTextBox.BackColor = Color.White; ErrorToolTip5.Hide(SideAngleTextBox);
            StandRadiusTextBox.BackColor = Color.White; ErrorToolTip6.Hide(StandRadiusTextBox);
        }
    }
}
