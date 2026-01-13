using Core;
using GlassPlugin;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace glass_plugin
{
    public partial class GUI : Form
    {
        private GlassBuilder _builder;
        private Parameters _parameters;

        public GUI()
        {
            InitializeComponent();
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            _parameters = new Parameters();
            _builder = new GlassBuilder();
            ResetFormToDefaults();
        }

        /// <summary>
        /// Этот метод обновляет ТОЛЬКО текстовые метки (Labels) с ограничениями.
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

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (CheckAll())
            {
                try
                {
                    _builder.BuildGlass(_parameters);
                    MessageBox.Show("Построение бокала начато!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка построения",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, исправьте все поля, выделенные красным!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Проверяет, что ни одно поле не подсвечено красным.
        /// </summary>
        private bool CheckAll()
        {
            return StalkHeightTextBox.BackColor == Color.White &&
                   SideHeightTextBox.BackColor == Color.White &&
                   BowlRadiusTextBox.BackColor == Color.White &&
                   StalkRadiusTextBox.BackColor == Color.White &&
                   SideAngleTextBox.BackColor == Color.White &&
                   StandRadiusTextBox.BackColor == Color.White;
        }

        // --- ФИНАЛЬНЫЕ, РАБОЧИЕ ОБРАБОТЧИКИ TextChanged ---

        private void StalkHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StalkHeight];
            if (double.TryParse(StalkHeightTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StalkHeightTextBox.BackColor = Color.White;
                ErrorToolTip1.Hide(StalkHeightTextBox);
                _parameters.SetDependenses(value, ParameterType.StalkHeight);
                UpdateAllLimitLabels();
            }
            else
            {
                StalkHeightTextBox.BackColor = Color.LightCoral;
                ErrorToolTip1.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StalkHeightTextBox, 3000);
            }
        }

        private void SideHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.SideHeight];
            if (double.TryParse(SideHeightTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                SideHeightTextBox.BackColor = Color.White;
                ErrorToolTip2.Hide(SideHeightTextBox);
                _parameters.SetDependenses(value, ParameterType.SideHeight);
                UpdateAllLimitLabels();
            }
            else
            {
                SideHeightTextBox.BackColor = Color.LightCoral;
                ErrorToolTip2.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", SideHeightTextBox, 3000);
            }
        }

        private void BowlRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.BowlRadius];
            if (double.TryParse(BowlRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                BowlRadiusTextBox.BackColor = Color.White;
                ErrorToolTip3.Hide(BowlRadiusTextBox);
                _parameters.SetDependenses(value, ParameterType.BowlRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                BowlRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip3.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", BowlRadiusTextBox, 3000);
            }
        }

        private void StalkRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StalkRadius];
            if (double.TryParse(StalkRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StalkRadiusTextBox.BackColor = Color.White;
                ErrorToolTip4.Hide(StalkRadiusTextBox);
                _parameters.SetDependenses(value, ParameterType.StalkRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                StalkRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip4.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StalkRadiusTextBox, 3000);
            }
        }

        private void SideAngleTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.SideAngle];
            if (double.TryParse(SideAngleTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                SideAngleTextBox.BackColor = Color.White;
                ErrorToolTip5.Hide(SideAngleTextBox);
                _parameters.SetDependenses(value, ParameterType.SideAngle);
                UpdateAllLimitLabels();
            }
            else
            {
                SideAngleTextBox.BackColor = Color.LightCoral;
                ErrorToolTip5.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", SideAngleTextBox, 3000);
            }
        }

        private void StandRadiusTextBox_TextChanged(object sender, EventArgs e)
        {
            var param = _parameters.NumericalParameters[ParameterType.StandRadius];
            if (double.TryParse(StandRadiusTextBox.Text, out double value) && value >= param.MinValue && value <= param.MaxValue)
            {
                StandRadiusTextBox.BackColor = Color.White;
                ErrorToolTip6.Hide(StandRadiusTextBox);
                _parameters.SetDependenses(value, ParameterType.StandRadius);
                UpdateAllLimitLabels();
            }
            else
            {
                StandRadiusTextBox.BackColor = Color.LightCoral;
                ErrorToolTip6.Show($"Значение должно быть от {param.MinValue:F1} до {param.MaxValue:F1}", StandRadiusTextBox, 3000);
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetFormToDefaults();
        }

        /// <summary>
        /// Сбрасывает модель и UI к начальным значениям.
        /// </summary>
        private void ResetFormToDefaults()
        {
            _parameters.ResetToDefaults();

            // Устанавливаем текст из сброшенной модели
            StalkHeightTextBox.Text = _parameters.NumericalParameters[ParameterType.StalkHeight].Value.ToString("F1");
            SideHeightTextBox.Text = _parameters.NumericalParameters[ParameterType.SideHeight].Value.ToString("F1");
            BowlRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.BowlRadius].Value.ToString("F1");
            StalkRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.StalkRadius].Value.ToString("F1");
            SideAngleTextBox.Text = _parameters.NumericalParameters[ParameterType.SideAngle].Value.ToString("F1");
            StandRadiusTextBox.Text = _parameters.NumericalParameters[ParameterType.StandRadius].Value.ToString("F1");

            // Обновляем метки
            UpdateAllLimitLabels();

            // Убираем все красные подсветки и прячем все подсказки
            StalkHeightTextBox.BackColor = Color.White; ErrorToolTip1.Hide(StalkHeightTextBox);
            SideHeightTextBox.BackColor = Color.White; ErrorToolTip2.Hide(SideHeightTextBox);
            BowlRadiusTextBox.BackColor = Color.White; ErrorToolTip3.Hide(BowlRadiusTextBox);
            StalkRadiusTextBox.BackColor = Color.White; ErrorToolTip4.Hide(StalkRadiusTextBox);
            SideAngleTextBox.BackColor = Color.White; ErrorToolTip5.Hide(SideAngleTextBox);
            StandRadiusTextBox.BackColor = Color.White; ErrorToolTip6.Hide(StandRadiusTextBox);
        }

        // Пустой обработчик, чтобы дизайнер не ругался, если он есть
        private void labelRadius_Click(object sender, EventArgs e) { }
    }
}
