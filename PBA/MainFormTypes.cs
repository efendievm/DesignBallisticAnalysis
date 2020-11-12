using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TypesLibrary;

namespace PBA
{
    enum CurveType { v_char, v_grav, v_aer, v_press, v_rel, v_abs, x, y, phi, xi, nu }
    enum FirstStageVariableParameterType { n_0, a_to, mu_su_pr, gamma_du, mu_k }
    enum SecondStageVariableParameterType { n_0, a_to, mu_su_pr, gamma_du }
    enum TrajectoryVariableParameterType { phi_k1, phi_k2, phi_0 }
    enum FirstStageOutputParameterType { P_0, m_0, m_k, m_t, m_o, m_g, m_to, m_su_pr, m_du, m_sux, mu_sux }
    enum SecondStageOutputParameterType { P_0, m_0, m_k, m_t, m_o, m_g, m_to, m_su_pr, m_du, m_sux, mu_sux, mu_k }
    enum TrajectoryOutputParameterType { phi_k1, phi_k2, phi_0, V, H, nu, t_1, t_2 }
    class CurveItem
    {
        public string Name { get; private set; }
        public ZedGraph.ZedGraphControl Graph { get; private set; }
        public ZedGraph.LineItem Curve { get; private set; }
        public CurveItem(string Name, ZedGraph.ZedGraphControl Graph, Color Color)
        {
            this.Name = Name;
            this.Graph = Graph;
            Curve = Graph.GraphPane.AddCurve(Name, null, null, Color, ZedGraph.SymbolType.None);
            Curve.IsVisible = false;
        }
    }
    class VariableParameterControl
    {
        TextBox StartTextBox;
        TextBox EndTextBox;
        CheckBox AutoCheckBox;
        CheckBox ManualCheckBox;
        Label StartLabel;
        Label EndLabel;
        double Scale;
        void CheckBox_Clicked(object sender, EventArgs e)
        {
            bool AutoMode = sender == AutoCheckBox;
            StartLabel.Visible = AutoMode;
            EndLabel.Visible = AutoMode;
            EndTextBox.Visible = AutoMode;
            ManualCheckBox.Checked = !AutoMode;
            AutoCheckBox.Checked = AutoMode;
        }
        public VariableParameter GetParameter()
        {
            return new VariableParameter(
                Convert.ToDouble(StartTextBox.Text) * Scale,
                (AutoCheckBox.Checked ? Convert.ToDouble(EndTextBox.Text) : Convert.ToDouble(StartTextBox.Text)) * Scale);
        }
        public bool AutoMode
        {
            get
            {
                return AutoCheckBox.Checked;
            }
            set
            {
                AutoCheckBox.Checked = value;
                CheckBox_Clicked(value ? AutoCheckBox : ManualCheckBox, EventArgs.Empty);
            }
        }
        public void SetManuel(double Value)
        {
            StartTextBox.Text = (Value / Scale).ToString();
            AutoMode = false;
        }
        public void SetInterval(double StartValue, double EndValue)
        {
            if (StartValue == EndValue)
                SetManuel(StartValue);
            else
            {
                StartTextBox.Text = (StartValue / Scale).ToString();
                EndTextBox.Text = (EndValue / Scale).ToString();
                AutoMode = true;
            }
        }
        public void Set(VariableParameter Value)
        {
            SetInterval(Value.Start, Value.End);
        }
        public VariableParameterControl(
            TextBox StartTextBox,
            TextBox EndTextBox,
            CheckBox AutoCheckBox,
            CheckBox ManualCheckBox,
            Label StartLabel,
            Label EndLabel,
            double Scale = 1)
        {
            this.StartTextBox = StartTextBox;
            this.EndTextBox = EndTextBox;
            this.AutoCheckBox = AutoCheckBox;
            this.ManualCheckBox = ManualCheckBox;
            this.StartLabel = StartLabel;
            this.EndLabel = EndLabel;
            this.Scale = Scale;
            AutoCheckBox.Click += CheckBox_Clicked;
            ManualCheckBox.Click += CheckBox_Clicked;
        }
    }
    class ConstantParametersControl
    {
        TextBox PayloadMassTextBox;
        TextBox HeightTextBox;
        TextBox VelocityTextBox;
        TextBox AngleTextBox;
        public double PayloadMass
        {
            get
            {
                return Convert.ToDouble(PayloadMassTextBox.Text);
            }
            set
            {
                PayloadMassTextBox.Text = value.ToString();
            }
        }
        public double Height
        {
            get
            {
                return Convert.ToDouble(HeightTextBox.Text) * 1E3;
            }
            set
            {
                HeightTextBox.Text = (value * 1E-3).ToString();
            }
        }
        public double Velocity
        {
            get
            {
                return Convert.ToDouble(VelocityTextBox.Text);
            }
            set
            {
                VelocityTextBox.Text = value.ToString();
            }
        }
        public double Angle
        {
            get
            {
                return Convert.ToDouble(AngleTextBox.Text) * Math.PI / 180;
            }
            set
            {
                AngleTextBox.Text = (value * 180 / Math.PI).ToString();
            }
        }
        public ConstantParametersControl(
            TextBox PayloadMassTextBox,
            TextBox HeightTextBox,
            TextBox VelocityTextBox,
            TextBox AngleTextBox)
        {
            this.PayloadMassTextBox = PayloadMassTextBox;
            this.HeightTextBox = HeightTextBox;
            this.VelocityTextBox = VelocityTextBox;
            this.AngleTextBox = AngleTextBox;
        }
    }
    class StageConstantParametersControl
    {
        TextBox VoidSpecificImpulseTextBox;
        TextBox SpecificImpulsesRatioTextBox;
        TextBox MidshipLoadTextBox;
        TextBox FuelDensityTextBox;
        TextBox OxidizeDensityTextBox;
        TextBox FuelRatioTextBox;
        public double VoidSpecificImpulse
        {
            get
            {
                return Convert.ToDouble(VoidSpecificImpulseTextBox.Text);
            }
            set
            {
                VoidSpecificImpulseTextBox.Text = value.ToString();
            }
        }
        public double SpecificImpulsesRatio
        {
            get
            {
                return Convert.ToDouble(SpecificImpulsesRatioTextBox.Text);
            }
            set
            {
                SpecificImpulsesRatioTextBox.Text = value.ToString();
            }
        }
        public double MidshipLoad
        {
            get
            {
                return Convert.ToDouble(MidshipLoadTextBox.Text);
            }
            set
            {
                MidshipLoadTextBox.Text = value.ToString();
            }
        }
        public double FuelDensity
        {
            get
            {
                return Convert.ToDouble(FuelDensityTextBox.Text);
            }
            set
            {
                FuelDensityTextBox.Text = value.ToString();
            }
        }
        public double OxidizeDensity
        {
            get
            {
                return Convert.ToDouble(OxidizeDensityTextBox.Text);
            }
            set
            {
                OxidizeDensityTextBox.Text = value.ToString();
            }
        }
        public double FuelRatio
        {
            get
            {
                return Convert.ToDouble(FuelRatioTextBox.Text);
            }
            set
            {
                FuelRatioTextBox.Text = value.ToString();
            }
        }
        public StageConstantParameters GetStageConstantParameters()
        {
            return new StageConstantParameters()
            {
                I_p = VoidSpecificImpulse,
                P_m = MidshipLoad,
                l = SpecificImpulsesRatio,
                rg = FuelDensity,
                ro = OxidizeDensity,
                kd = FuelRatio
            };
        }
        public StageConstantParametersControl(
            TextBox VoidSpecificImpulseTextBox,
            TextBox SpecificImpulsesRatioTextBox,
            TextBox MidshipLoadTextBox,
            TextBox FuelDensityTextBox,
            TextBox OxidizeDensityTextBox,
            TextBox FuelRatioTextBox)
        {
            this.VoidSpecificImpulseTextBox = VoidSpecificImpulseTextBox;
            this.SpecificImpulsesRatioTextBox = SpecificImpulsesRatioTextBox;
            this.MidshipLoadTextBox = MidshipLoadTextBox;
            this.FuelDensityTextBox = FuelDensityTextBox;
            this.OxidizeDensityTextBox = OxidizeDensityTextBox;
            this.FuelRatioTextBox = FuelRatioTextBox;
        }
    }
    class VariableParametersControl : IEnumerable
    {
        Dictionary<FirstStageVariableParameterType, VariableParameterControl> FirstStageVariableParameterControl;
        Dictionary<SecondStageVariableParameterType, VariableParameterControl> SecondStageVariableParameterControl;
        Dictionary<TrajectoryVariableParameterType, VariableParameterControl> TrajecoryVariableParameterControl;
        public void Add(FirstStageVariableParameterType firstStageVariableParameterType, VariableParameterControl variableParameterControl)
        {
            FirstStageVariableParameterControl.Add(firstStageVariableParameterType, variableParameterControl);
        }
        public void Add(SecondStageVariableParameterType secondStageVariableParameterType, VariableParameterControl variableParameterControl)
        {
            SecondStageVariableParameterControl.Add(secondStageVariableParameterType, variableParameterControl);
        }
        public void Add(TrajectoryVariableParameterType trajecoryVariableParameterType, VariableParameterControl variableParameterControl)
        {
            TrajecoryVariableParameterControl.Add(trajecoryVariableParameterType, variableParameterControl);
        }
        public VariableParameterControl this[FirstStageVariableParameterType firstStageVariableParameterType]
        {
            get
            {
                return FirstStageVariableParameterControl[firstStageVariableParameterType];
            }
            set
            {
                FirstStageVariableParameterControl[firstStageVariableParameterType] = value;
            }
        }
        public VariableParameterControl this[SecondStageVariableParameterType secondStageVariableParameterType]
        {
            get
            {
                return SecondStageVariableParameterControl[secondStageVariableParameterType];
            }
            set
            {
                SecondStageVariableParameterControl[secondStageVariableParameterType] = value;
            }
        }
        public VariableParameterControl this[TrajectoryVariableParameterType trajecoryStageVariableParameterType]
        {
            get
            {
                return TrajecoryVariableParameterControl[trajecoryStageVariableParameterType];
            }
            set
            {
                TrajecoryVariableParameterControl[trajecoryStageVariableParameterType] = value;
            }
        }
        public FirstStageVariableParameters GetFirstStageVariableParameters()
        {
            return new FirstStageVariableParameters()
            {
                n_0 = this[FirstStageVariableParameterType.n_0].GetParameter(),
                a_to = this[FirstStageVariableParameterType.a_to].GetParameter(),
                mu_su_pr = this[FirstStageVariableParameterType.mu_su_pr].GetParameter(),
                gamma_du = this[FirstStageVariableParameterType.gamma_du].GetParameter(),
                mu_k = this[FirstStageVariableParameterType.mu_k].GetParameter()
            };
        }
        public SecondStageVariableParameters GetSecondStageVariableParameters()
        {
            return new SecondStageVariableParameters()
            {
                n_0 = this[SecondStageVariableParameterType.n_0].GetParameter(),
                a_to = this[SecondStageVariableParameterType.a_to].GetParameter(),
                mu_su_pr = this[SecondStageVariableParameterType.mu_su_pr].GetParameter(),
                gamma_du = this[SecondStageVariableParameterType.gamma_du].GetParameter()
            };
        }
        public IEnumerator GetEnumerator()
        {
            var VariableParameterControls = FirstStageVariableParameterControl.Select(x => x.Value)
                .Concat(SecondStageVariableParameterControl.Select(x => x.Value))
                .Concat(TrajecoryVariableParameterControl.Select(x => x.Value));
            foreach (var control in VariableParameterControls)
                yield return control;
        }
        public VariableParametersControl()
        {
            FirstStageVariableParameterControl = new Dictionary<FirstStageVariableParameterType, VariableParameterControl>();
            SecondStageVariableParameterControl = new Dictionary<SecondStageVariableParameterType, VariableParameterControl>();
            TrajecoryVariableParameterControl = new Dictionary<TrajectoryVariableParameterType, VariableParameterControl>();
        }
    }
    class CalculationResultControl
    {
        Dictionary<FirstStageVariableParameterType, TextBox> FirstStageVariableParameterTextBoxes;
        Dictionary<SecondStageVariableParameterType, TextBox> SecondStageVariableParameterTextBoxes;
        Dictionary<TrajectoryOutputParameterType, TextBox> TrajectoryOutputParameters;
        Dictionary<FirstStageOutputParameterType, TextBox> FirstStageOutputParameterTextBoxes;
        Dictionary<SecondStageOutputParameterType, TextBox> SecondStageOutputParameterTextBoxes;
        TextBox PayloadMassRatioTextBox;
        public void SetCalculationResult(StagesVariableValues variableParameterValues, OutputParameters outputParameters)
        {
            FirstStageVariableParameterTextBoxes[FirstStageVariableParameterType.n_0].Text = FormatConvert.ToString(variableParameterValues.firstStageVariableValues.n_0);
            FirstStageVariableParameterTextBoxes[FirstStageVariableParameterType.a_to].Text = FormatConvert.ToString(variableParameterValues.firstStageVariableValues.a_to * 1E-3);
            FirstStageVariableParameterTextBoxes[FirstStageVariableParameterType.mu_su_pr].Text = FormatConvert.ToString(variableParameterValues.firstStageVariableValues.mu_su_pr);
            FirstStageVariableParameterTextBoxes[FirstStageVariableParameterType.gamma_du].Text = FormatConvert.ToString(variableParameterValues.firstStageVariableValues.gamma_du);
            FirstStageVariableParameterTextBoxes[FirstStageVariableParameterType.mu_k].Text = FormatConvert.ToString(variableParameterValues.firstStageVariableValues.mu_k);

            SecondStageVariableParameterTextBoxes[SecondStageVariableParameterType.n_0].Text = FormatConvert.ToString(variableParameterValues.secondStageVariableValues.n_0);
            SecondStageVariableParameterTextBoxes[SecondStageVariableParameterType.a_to].Text = FormatConvert.ToString(variableParameterValues.secondStageVariableValues.a_to * 1E-3);
            SecondStageVariableParameterTextBoxes[SecondStageVariableParameterType.mu_su_pr].Text = FormatConvert.ToString(variableParameterValues.secondStageVariableValues.mu_su_pr);
            SecondStageVariableParameterTextBoxes[SecondStageVariableParameterType.gamma_du].Text = FormatConvert.ToString(variableParameterValues.secondStageVariableValues.gamma_du);

            TrajectoryOutputParameters[TrajectoryOutputParameterType.phi_k1].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.phi_k1 * 180 / Math.PI);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.phi_k2].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.phi_k2 * 180 / Math.PI);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.phi_0].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.phi_0 * 180 / Math.PI);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.V].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.V);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.H].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.H * 1E-3);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.nu].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.nu * 180 / Math.PI);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.t_1].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.t_1);
            TrajectoryOutputParameters[TrajectoryOutputParameterType.t_2].Text = FormatConvert.ToString(outputParameters.TrajectoryOutputParameters.t_2);

            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.P_0].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.P_0 * 1E-3);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_0].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_0);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_k].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_k);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_t].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_t);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_o].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_o);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_g].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_g);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_to].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_to);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_su_pr].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_su_pr);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_du].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_du);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.m_sux].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.m_sux);
            FirstStageOutputParameterTextBoxes[FirstStageOutputParameterType.mu_sux].Text = FormatConvert.ToString(outputParameters.FirstStageOutputData.mu_sux);

            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.P_0].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.P_0 * 1E-3);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_0].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_0);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_k].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_k);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_t].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_t);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_o].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_o);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_g].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_g);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_to].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_to);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_su_pr].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_su_pr);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_du].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_du);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.m_sux].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.m_sux);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.mu_sux].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.mu_sux);
            SecondStageOutputParameterTextBoxes[SecondStageOutputParameterType.mu_k].Text = FormatConvert.ToString(outputParameters.SecondStageOutputData.mu_k);

            PayloadMassRatioTextBox.Text = FormatConvert.ToString(outputParameters.mu_pg);
        }
        public CalculationResultControl(
            Dictionary<FirstStageVariableParameterType, TextBox> FirstStageVariableParameterTextBoxes,
            Dictionary<SecondStageVariableParameterType, TextBox> SecondStageVariableParameterTextBoxes,
            Dictionary<TrajectoryOutputParameterType, TextBox> TrajectoryOutputParameters,
            Dictionary<FirstStageOutputParameterType, TextBox> FirstStageOutputParameterTextBoxes,
            Dictionary<SecondStageOutputParameterType, TextBox> SecondStageOutputParameterTextBoxes,
            TextBox PayloadMassRatioTextBox)
        {
            this.FirstStageVariableParameterTextBoxes = FirstStageVariableParameterTextBoxes;
            this.SecondStageVariableParameterTextBoxes = SecondStageVariableParameterTextBoxes;
            this.TrajectoryOutputParameters = TrajectoryOutputParameters;
            this.FirstStageOutputParameterTextBoxes = FirstStageOutputParameterTextBoxes;
            this.SecondStageOutputParameterTextBoxes = SecondStageOutputParameterTextBoxes;
            this.PayloadMassRatioTextBox = PayloadMassRatioTextBox;
        }
    }
    static class FormatConvert
    {
        public static string CharsToString(List<char> s)
        {
            return new string(s.ToArray());
        }
        public static string ToString(double Value)
        {
            if (Value == 0) return "0";
            double Root = Math.Truncate(Value);
            var Tail = Math.Abs(Value - Root).ToString("F50").ToCharArray().Skip(2).ToList();
            var NullTail = CharsToString(Tail.TakeWhile(x => x == '0').ToList());
            if (NullTail.Length == Tail.Count)
                return Root.ToString();
            var DigitTailChars = Tail.Skip(NullTail.Count()).ToList();
            DigitTailChars = DigitTailChars.Take(Math.Min(4, DigitTailChars.Count)).ToList();
            string Res;
            if (DigitTailChars.Count < 4)
                Res = Root.ToString() + "," + NullTail + CharsToString(DigitTailChars);
            else
            {
                DigitTailChars.Insert(3, ',');
                double RoundDigitTail = Math.Round(Convert.ToDouble(CharsToString(DigitTailChars)));
                if (RoundDigitTail == 1000)
                {
                    if (NullTail.Length == 0)
                        Res = (Root + Math.Sign(Root)).ToString();
                    else
                    {
                        var nullTail = NullTail.ToCharArray();
                        nullTail[nullTail.Length - 1] = '1';
                        NullTail = CharsToString(nullTail.ToList());
                        Res = Root.ToString() + "," + NullTail;
                    }
                }
                else
                    Res = Root.ToString() + "," + NullTail + RoundDigitTail.ToString();
            }
            var res = Res.ToCharArray();
            Array.Reverse(res);
            res = res.SkipWhile(x => x == '0').ToArray();
            Array.Reverse(res);
            return new string(res);
        }
    }
}
