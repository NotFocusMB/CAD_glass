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

        private Dictionary<TextBox, ParameterType> _textBoxMapping;

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
            InitializeTextBoxMapping();
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
        /// Инициализирует словарь для связи типов параметров с элементами UI.
        /// </summary>
        private void InitializeTextBoxMapping()
        {
            _textBoxMapping = new Dictionary<TextBox, ParameterType>
            {
                { StalkHeightTextBox, ParameterType.StalkHeight },
                { SideHeightTextBox, ParameterType.SideHeight },
                { BowlRadiusTextBox, ParameterType.BowlRadius },
                { StalkRadiusTextBox, ParameterType.StalkRadius },
                { SideAngleTextBox, ParameterType.SideAngle },
                { StandRadiusTextBox, ParameterType.StandRadius }
            };

            foreach (var kvp in _textBoxMapping)
            {
                kvp.Key.TextChanged += UnifiedTextChangedHandler;
            }
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
                bool createHandle = HandleCheckBox.Checked;
                _builder.BuildGlass(_parameters, createHandle);
                CalculateAndShowCharacteristics();
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
        /// Вычисление и отображение характеристик бокала.
        /// </summary>
        private void CalculateAndShowCharacteristics()
        {
            var numParams = _parameters.NumericalParameters;
            double stalkHeight = numParams[ParameterType.StalkHeight].Value;
            double bowlRadius = numParams[ParameterType.BowlRadius].Value;
            double stalkRadius = numParams[ParameterType.StalkRadius].Value;
            double standRadius = numParams[ParameterType.StandRadius].Value;
            double sideAngle = numParams[ParameterType.SideAngle].Value;
            double sideHeight = numParams[ParameterType.SideHeight].Value;

            double angleRad = sideAngle * Math.PI / 180.0;
            double wallVerticalProjection = sideHeight * Math.Cos(angleRad);
            double totalHeight = stalkRadius + stalkHeight + bowlRadius
                + wallVerticalProjection;

            double standArea = Math.PI * Math.Pow(standRadius, 2);

            double wallThickness = stalkRadius / 2.0;
            double innerBowlRadius = bowlRadius - wallThickness;

            double volumeHemisphere = (2.0 / 3.0) * Math.PI
                * Math.Pow(innerBowlRadius, 3);

            double h_frustum = wallVerticalProjection;
            double R_large = innerBowlRadius;
            double r_small = innerBowlRadius - (sideHeight * Math.Sin(angleRad));
            double radiiSum = Math.Pow(R_large, 2) + R_large * r_small
                + Math.Pow(r_small, 2);
            double volumeFrustum = (1.0 / 3.0) * Math.PI * h_frustum * radiiSum;

            double bowlVolumeMm3 = volumeHemisphere + volumeFrustum;
            double bowlVolumeMl = bowlVolumeMm3 / 1000.0;

            TotalHeightLabel.Text = $"{totalHeight:F1} мм";
            AreaLabel.Text = $"{standArea:F1} мм²";
            VolumeLabelMM.Text = $"{bowlVolumeMm3:F0} мм³";
            VolumeLabelML.Text = $"{bowlVolumeMl:F1} мл";

            this.Width = 436;
            CalculationsGroupBox.Visible = true;
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

        //TODO: remove region
        #region TextChanged Event Handlers

        /// <summary>
        /// Универсальный обработчик изменения текста в TextBox.
        /// </summary>
        private void UnifiedTextChangedHandler(object sender, EventArgs e)
        {
            if (sender is TextBox textBox 
                && _textBoxMapping.TryGetValue(textBox, out ParameterType paramType))
            {
                ValidateAndUpdate(paramType);
            }
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
