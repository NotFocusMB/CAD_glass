using Core;
using GlassPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace glass_plugin
{
    /// <summary>
    /// Главная форма пользовательского интерфейса для плагина построения бокала.
    /// </summary>
    public partial class GUI : Form
    {
        /// <summary>
        /// Экземпляр класса, отвечающего за построение 3D-модели.
        /// </summary>
        private GlassBuilder _builder;

        /// <summary>
        /// Экземпляр класса, управляющего параметрами и их зависимостями.
        /// </summary>
        private Parameters _parameters;

        /// <summary>
        /// Карта для связи типа параметра с его элементами управления на форме.
        /// </summary>
        private Dictionary<ParameterType,
            (TextBox TextBox, ToolTip ToolTip, Label Label)> _controlMap;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public GUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void GUI_Load(object sender, EventArgs e)
        {
            _parameters = new Parameters();
            _builder = new GlassBuilder();
            InitializeControlMap();
            ResetFormToDefaults();
        }

        /// <summary>
        /// Инициализирует словарь для связи параметров с элементами UI.
        /// </summary>
        private void InitializeControlMap()
        {
            _controlMap = new Dictionary<ParameterType, (TextBox, ToolTip, Label)>
            {
                [ParameterType.StalkHeight] = (StalkHeightTextBox, ErrorToolTip1,
                    StalkHeightLimitsLabel),
                [ParameterType.SideHeight] = (SideHeightTextBox, ErrorToolTip2,
                    SideHeightLimitsLabel),
                [ParameterType.BowlRadius] = (BowlRadiusTextBox, ErrorToolTip3,
                    BowlRadiusLimitsLabel),
                [ParameterType.StalkRadius] = (StalkRadiusTextBox, ErrorToolTip4,
                    StalkRadiusLimitsLabel),
                [ParameterType.SideAngle] = (SideAngleTextBox, ErrorToolTip5,
                    SideAngleLimitsLabel),
                [ParameterType.StandRadius] = (StandRadiusTextBox, ErrorToolTip6,
                    StandRadiusLimitsLabel)
            };
        }

        /// <summary>
        /// Обновляет все метки с ограничениями на форме.
        /// </summary>
        private void UpdateAllLimitLabels()
        {
            foreach (var entry in _controlMap)
            {
                entry.Value.Label.Text = _parameters.GetLimitsString(entry.Key);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Создать".
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
                MessageBox.Show("Пожалуйста, исправьте все поля, выделенные красным!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Проверяет, что все поля ввода на форме содержат корректные значения.
        /// </summary>
        /// <returns>True, если все поля валидны; иначе False.</returns>
        private bool CheckAll()
        {
            return _controlMap.Values.All(c => c.TextBox.BackColor == Color.White);
        }

        /// <summary>
        /// Универсальный обработчик для валидации и обновления параметра.
        /// </summary>
        /// <param name="paramType">Тип параметра для обработки.</param>
        private void ValidateAndUpdate(ParameterType paramType)
        {
            var (textBox, toolTip, _) = _controlMap[paramType];
            var param = _parameters.NumericalParameters[paramType];

            bool isValid = double.TryParse(textBox.Text, out double value) &&
                           value >= param.MinValue &&
                           value <= param.MaxValue;

            if (isValid)
            {
                textBox.BackColor = Color.White;
                toolTip.Hide(textBox);
                _parameters.SetDependencies(value, paramType);
                UpdateAllLimitLabels();
            }
            else
            {
                textBox.BackColor = Color.LightCoral;
                string message = $"Значение должно быть от {param.MinValue:F1}" +
                                 $" до {param.MaxValue:F1}";
                toolTip.Show(message, textBox, 5000);
            }
        }

        #region TextChanged Event Handlers

        //TODO: XML
        //TODO: duplication
        private void StalkHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.StalkHeight);
        }

        //TODO: duplication
        private void SideHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.SideHeight);
        }

        //TODO: duplication
        private void BowlRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.BowlRadius);
        }

        //TODO: duplication
        private void StalkRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.StalkRadius);
        }

        //TODO: duplication
        private void SideAngleTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.SideAngle);
        }

        //TODO: duplication
        private void StandRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateAndUpdate(ParameterType.StandRadius);
        }

        #endregion

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
            _parameters.ResetToDefaults();

            // Используем цикл для обновления всех полей и их вида
            foreach (var entry in _controlMap)
            {
                var (textBox, toolTip, _) = entry.Value;
                var param = _parameters.NumericalParameters[entry.Key];

                textBox.Text = param.Value.ToString("F1");
                textBox.BackColor = Color.White;
                toolTip.Hide(textBox);
            }

            UpdateAllLimitLabels();
        }
    }
}
