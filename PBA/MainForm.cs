using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewInterface;
using TypesLibrary;

namespace PBA
{
    public partial class MainForm : Form, IView
    {
        public MainForm()
        {
            InitializeComponent();
            for (int i = 0; i < Grid.Columns.Count; i++)
                Grid.Columns[i].HeaderCell.Style.Font = new Font("Arial", 9, i < 10 ? FontStyle.Bold : FontStyle.Regular);
            int Width9Cols = Grid.Columns.Cast<DataGridViewColumn>().Take(10).Sum(x => x.Width + x.DividerWidth * 2);
            Resize += (sender, e) =>
            {
                if (Grid.Width > Width9Cols)
                    Grid.Columns[9].Frozen = true;
                else
                {
                    for (int i = 0; i < 10; i++)
                        Grid.Columns[i].Frozen = false;
                }
            };

            constantParametersControl = new ConstantParametersControl(textBox80, textBox8, textBox9, textBox10);
            FirstStageConstantParametersControl = new StageConstantParametersControl(textBox2, textBox4, textBox6, textBox71, textBox72, textBox73);
            SecondStageConstantParametersControl = new StageConstantParametersControl(textBox27, textBox26, textBox25, textBox24, textBox23, textBox22);
            variableParametersControl = new VariableParametersControl()
            {
                { FirstStageVariableParameterType.n_0, new VariableParameterControl(textBox70, textBox74, checkBox12, checkBox28, label86, label97) },
                { FirstStageVariableParameterType.a_to, new VariableParameterControl(textBox67, textBox68, checkBox10, checkBox27, label77, label81, 1E3) },
                { FirstStageVariableParameterType.mu_su_pr, new VariableParameterControl(textBox65, textBox66, checkBox9, checkBox11, label74, label75) },
                { FirstStageVariableParameterType.gamma_du, new VariableParameterControl(textBox63, textBox64, checkBox7, checkBox8, label72, label73) },
                { FirstStageVariableParameterType.mu_k, new VariableParameterControl(textBox75, textBox76, checkBox30, checkBox29, label99, label100) },
                { SecondStageVariableParameterType.n_0, new VariableParameterControl(textBox40, textBox41, checkBox20, checkBox22, label53, label54) },
                { SecondStageVariableParameterType.a_to, new VariableParameterControl(textBox38, textBox39, checkBox18, checkBox21, label23, label49, 1E3) },
                { SecondStageVariableParameterType.mu_su_pr, new VariableParameterControl(textBox20, textBox21, checkBox15, checkBox19, label20, label22) },
                { SecondStageVariableParameterType.gamma_du, new VariableParameterControl(textBox18, textBox19, checkBox5, checkBox6, label18, label19) },
                { TrajectoryVariableParameterType.phi_k1, new VariableParameterControl(textBox16, textBox17, CheckBox14, CheckBox17, label47, label48, Math.PI / 180) },
                { TrajectoryVariableParameterType.phi_0, new VariableParameterControl(textBox13, textBox15, CheckBox2, CheckBox16, label12, label16, Math.PI / 180) },
                { TrajectoryVariableParameterType.phi_k2, new VariableParameterControl(textBox5, textBox11, CheckBox1, CheckBox13, label4, label10, Math.PI / 180) }
            };
            foreach (var variableParameterControl in variableParametersControl)
                ((VariableParameterControl)variableParameterControl).AutoMode = false;
            var FirstStageVariableTextBoxes = new Dictionary<FirstStageVariableParameterType, TextBox>()
            {
                { FirstStageVariableParameterType.n_0, textBox12 },
                { FirstStageVariableParameterType.a_to, textBox29 },
                { FirstStageVariableParameterType.mu_su_pr, textBox31 },
                { FirstStageVariableParameterType.gamma_du, textBox30 },
                { FirstStageVariableParameterType.mu_k, textBox28 }
            };
            var SecondStageVariableTextBoxes = new Dictionary<SecondStageVariableParameterType, TextBox>()
            {
                { SecondStageVariableParameterType.n_0, textBox58 },
                { SecondStageVariableParameterType.a_to, textBox43 },
                { SecondStageVariableParameterType.mu_su_pr, textBox14 },
                { SecondStageVariableParameterType.gamma_du, textBox42 }
            };
            var TrajectoryOutputParameters = new Dictionary<TrajectoryOutputParameterType, TextBox>()
            {
                { TrajectoryOutputParameterType.phi_k1, textBox82 },
                { TrajectoryOutputParameterType.phi_k2, textBox84 },
                { TrajectoryOutputParameterType.phi_0, textBox83 },
                { TrajectoryOutputParameterType.V, textBox88 },
                { TrajectoryOutputParameterType.H, textBox89 },
                { TrajectoryOutputParameterType.nu, textBox87 },
                { TrajectoryOutputParameterType.t_1, textBox85 },
                { TrajectoryOutputParameterType.t_2, textBox86 }
            };
            var FirstStageOutputParameterTextBoxes = new Dictionary<FirstStageOutputParameterType, TextBox>()
            {
                { FirstStageOutputParameterType.P_0, textBox50 },
                { FirstStageOutputParameterType.m_0, textBox35 },
                { FirstStageOutputParameterType.m_k, textBox69 },
                { FirstStageOutputParameterType.m_t, textBox33 },
                { FirstStageOutputParameterType.m_o, textBox32 },
                { FirstStageOutputParameterType.m_g, textBox53 },
                { FirstStageOutputParameterType.m_to, textBox52 },
                { FirstStageOutputParameterType.m_su_pr, textBox51 },
                { FirstStageOutputParameterType.m_du, textBox34 },
                { FirstStageOutputParameterType.m_sux, textBox54 },
                { FirstStageOutputParameterType.mu_sux, textBox3 }
            };
            var SecondStageOutputParameterTextBoxes = new Dictionary<SecondStageOutputParameterType, TextBox>()
            {
                { SecondStageOutputParameterType.P_0, textBox57 },
                { SecondStageOutputParameterType.m_0, textBox56 },
                { SecondStageOutputParameterType.m_k, textBox37 },
                { SecondStageOutputParameterType.m_t, textBox55 },
                { SecondStageOutputParameterType.m_o, textBox49 },
                { SecondStageOutputParameterType.m_g, textBox48 },
                { SecondStageOutputParameterType.m_to, textBox47 },
                { SecondStageOutputParameterType.m_su_pr, textBox46 },
                { SecondStageOutputParameterType.m_du, textBox45 },
                { SecondStageOutputParameterType.m_sux, textBox36 },
                { SecondStageOutputParameterType.mu_sux, textBox7 },
                { SecondStageOutputParameterType.mu_k, textBox44 }
            };
            calculationResultControl = new CalculationResultControl(
                FirstStageVariableTextBoxes,
                SecondStageVariableTextBoxes,
                TrajectoryOutputParameters,
                FirstStageOutputParameterTextBoxes,
                SecondStageOutputParameterTextBoxes,
                textBox81);
            
            Action<ZedGraph.GraphPane, string, string> AdjustGraph = (Pane, XTitle, YTitle) =>
            {

                Pane.Legend.Position = ZedGraph.LegendPos.InsideTopLeft;
                Pane.XAxis.Title.Text = XTitle;
                Pane.YAxis.Title.Text = YTitle;
                Pane.Title.IsVisible = false;
                Action<ZedGraph.Axis> adjustAxis = Axis =>
                {
                    Axis.MajorGrid.DashOff = 1;
                    Axis.MinorGrid.DashOff = 5;
                    Axis.MajorGrid.IsVisible = true;
                    Axis.MinorGrid.IsVisible = true;
                };
                adjustAxis(Pane.XAxis);
                adjustAxis(Pane.YAxis);
                Pane.IsFontsScaled = false;
            };
            VelocityGraph.GraphPane.Legend.Position = ZedGraph.LegendPos.InsideTopLeft;
            AdjustGraph(VelocityGraph.GraphPane, "Время, с", "Скорость, км/с");
            AdjustGraph(CoordinatesGraph.GraphPane, "Время, с", "Координата, км");
            AdjustGraph(TrajectoryGraph.GraphPane, "Координата X, км", "Координата Y, км");
            AdjustGraph(HeightGraph.GraphPane, "Время, с", "Высота, км");
            AdjustGraph(MassGraph.GraphPane, "Время, с", "Масса, т");
            AdjustGraph(AngleGraph.GraphPane, "Время, с", "Угол, град");
            CurveItems = new Dictionary<CurveType, CurveItem>()
            {
                { CurveType.v_char,  new CurveItem("Характеристическая", VelocityGraph, Color.Green) },
                { CurveType.v_rel,   new CurveItem("Относительная", VelocityGraph, Color.Navy) },
                { CurveType.v_abs,   new CurveItem("Абсолютная", VelocityGraph, Color.Black) },
                { CurveType.v_grav,  new CurveItem("Гравитационные потери", VelocityGraph, Color.Red) },
                { CurveType.v_aer,   new CurveItem("Аэродинамические потери", VelocityGraph, Color.Blue) },
                { CurveType.v_press, new CurveItem("Потери на противодавление", VelocityGraph, Color.Orange) },
                { CurveType.x,       new CurveItem("Координата x", CoordinatesGraph, Color.Red) },
                { CurveType.y,       new CurveItem("Координата y", CoordinatesGraph, Color.Navy) },
                { CurveType.phi,     new CurveItem("Угол тангажа", AngleGraph, Color.Red) },
                { CurveType.xi,      new CurveItem("Угол пути", AngleGraph, Color.Green) },
                { CurveType.nu,      new CurveItem("Угол наклона вектора скорости к местному горизонту", AngleGraph, Color.Navy) },
            };
            TrajectoryGraph.GraphPane.AddCurve("Траектория", null, null, Color.Navy, ZedGraph.SymbolType.None);
            MassGraph.GraphPane.AddCurve("Масса ракеты", null, null, Color.Navy, ZedGraph.SymbolType.None);
            HeightGraph.GraphPane.AddCurve("Высота", null, null, Color.Navy, ZedGraph.SymbolType.None);
            Action<ContextMenuStrip> AdjustDefaultContextMenu = menuStrip =>
            {
                menuStrip.Items.RemoveAt(2);
                menuStrip.Items.RemoveAt(3);
                menuStrip.Items.RemoveAt(3);
                menuStrip.Items.RemoveAt(3);
                menuStrip.Items[0].Text = "Копировать";
                menuStrip.Items[1].Text = "Сохранить изображение как..";
                menuStrip.Items[2].Text = "Печать...";
                menuStrip.Items[3].Text = "Исходный вид";
            };
            VelocityGraph.ContextMenuBuilder += (sender, menuStrip, mousePt, objState) =>
            {
                AdjustDefaultContextMenu(menuStrip);
                menuStrip.Items.Add(new ToolStripSeparator());
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_char));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_rel));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_abs));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_grav));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_aer));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.v_press));
            };
            CoordinatesGraph.ContextMenuBuilder += (sender, menuStrip, mousePt, objState) =>
            {
                AdjustDefaultContextMenu(menuStrip);
                menuStrip.Items.Add(new ToolStripSeparator());
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.x));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.y));
            };
            AngleGraph.ContextMenuBuilder += (sender, menuStrip, mousePt, objState) =>
            {
                AdjustDefaultContextMenu(menuStrip);
                menuStrip.Items.Add(new ToolStripSeparator());
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.phi));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.xi));
                menuStrip.Items.Add(new CustomToolStripMenuItem(CurveType.nu));
            };
            foreach (var graph in new List<ZedGraph.ZedGraphControl>() { TrajectoryGraph, MassGraph, HeightGraph })
                graph.ContextMenuBuilder += (sender, menuStrip, mousePt, objState) => AdjustDefaultContextMenu(menuStrip);
        }
        VariableParametersControl variableParametersControl;
        ConstantParametersControl constantParametersControl;
        StageConstantParametersControl FirstStageConstantParametersControl;
        StageConstantParametersControl SecondStageConstantParametersControl;
        CalculationResultControl calculationResultControl;
        CancellationTokenSource cts;
        DateTime StartTime;
        Dictionary<StagesVariableValues, OutputParameters> Result;
        Dictionary<StagesVariableValues, List<TrajectoryParameters>> Trajectories = new Dictionary<StagesVariableValues,List<TrajectoryParameters>>();
        double CurrentProgress;
        static Dictionary<CurveType, CurveItem> CurveItems;
        CalculationInputData GetCalculationInputData()
        {
            CalculationInputData id = new CalculationInputData();
            id.FirstStageConstantParameters = FirstStageConstantParametersControl.GetStageConstantParameters();
            id.SecondStageConstantParameters = SecondStageConstantParametersControl.GetStageConstantParameters();
            id.FirstStageVariableParameters = variableParametersControl.GetFirstStageVariableParameters();
            id.SecondStageVariableParameters = variableParametersControl.GetSecondStageVariableParameters();
            id.phi_k1 = variableParametersControl[TrajectoryVariableParameterType.phi_k1].GetParameter();
            id.phi_k2 = variableParametersControl[TrajectoryVariableParameterType.phi_k2].GetParameter();
            id.phi_0 = variableParametersControl[TrajectoryVariableParameterType.phi_0].GetParameter();
            id.M_pg = constantParametersControl.PayloadMass;
            id.V = constantParametersControl.Velocity;
            id.H = constantParametersControl.Height;
            id.nu = constantParametersControl.Angle;
            return id;
        }
        async void FillCalculationResult()
        {
            Grid.Rows.Clear();
            if ((Result != null) && (Result.Count != 0))
            {
                Action<DataGridViewRow, int, StageOutputData> AddStageOutputDataInGrid = (Row, StartColumnIndex, StageOutputData) =>
                {
                    Row.Cells[StartColumnIndex + 0].Value = FormatConvert.ToString(StageOutputData.P_0 * 1E-3);
                    Row.Cells[StartColumnIndex + 1].Value = FormatConvert.ToString(StageOutputData.m_0);
                    Row.Cells[StartColumnIndex + 2].Value = FormatConvert.ToString(StageOutputData.m_k);
                    Row.Cells[StartColumnIndex + 3].Value = FormatConvert.ToString(StageOutputData.m_t);
                    Row.Cells[StartColumnIndex + 4].Value = FormatConvert.ToString(StageOutputData.m_o);
                    Row.Cells[StartColumnIndex + 5].Value = FormatConvert.ToString(StageOutputData.m_g);
                    Row.Cells[StartColumnIndex + 6].Value = FormatConvert.ToString(StageOutputData.m_to);
                    Row.Cells[StartColumnIndex + 7].Value = FormatConvert.ToString(StageOutputData.m_su_pr);
                    Row.Cells[StartColumnIndex + 8].Value = FormatConvert.ToString(StageOutputData.m_du);
                    Row.Cells[StartColumnIndex + 9].Value = FormatConvert.ToString(StageOutputData.m_sux);
                };
                int i = 1;
                foreach (var result in Result)
                {
                    Grid.Rows.Add();
                    var row = Grid.Rows[Grid.Rows.Count - 1];
                    row.Cells[0].Value = i;
                    row.Cells[1].Value = FormatConvert.ToString(result.Key.firstStageVariableValues.n_0);
                    row.Cells[2].Value = FormatConvert.ToString(result.Key.secondStageVariableValues.n_0);
                    row.Cells[3].Value = FormatConvert.ToString(result.Key.firstStageVariableValues.gamma_du);
                    row.Cells[4].Value = FormatConvert.ToString(result.Key.secondStageVariableValues.gamma_du);
                    row.Cells[5].Value = FormatConvert.ToString(result.Key.firstStageVariableValues.a_to * 1E-3);
                    row.Cells[6].Value = FormatConvert.ToString(result.Key.secondStageVariableValues.a_to * 1E-3);
                    row.Cells[7].Value = FormatConvert.ToString(result.Key.firstStageVariableValues.mu_su_pr);
                    row.Cells[8].Value = FormatConvert.ToString(result.Key.secondStageVariableValues.mu_su_pr);
                    row.Cells[9].Value = FormatConvert.ToString(result.Key.firstStageVariableValues.mu_k);
                    row.Cells[10].Value = FormatConvert.ToString(result.Value.SecondStageOutputData.mu_k);
                    row.Cells[11].Value = FormatConvert.ToString(result.Value.FirstStageOutputData.mu_sux);
                    row.Cells[12].Value = FormatConvert.ToString(result.Value.SecondStageOutputData.mu_sux);

                    AddStageOutputDataInGrid(row, 13, result.Value.FirstStageOutputData);
                    AddStageOutputDataInGrid(row, 23, result.Value.SecondStageOutputData);

                    row.Cells[33].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.phi_k1 * 180 / Math.PI);
                    row.Cells[34].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.phi_0 * 180 / Math.PI);
                    row.Cells[35].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.phi_k2 * 180 / Math.PI);
                    row.Cells[36].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.t_1);
                    row.Cells[37].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.t_2);
                    row.Cells[38].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.H * 1E-3);
                    row.Cells[39].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.V);
                    row.Cells[40].Value = FormatConvert.ToString(result.Value.TrajectoryOutputParameters.nu * 180 / Math.PI);
                    row.Cells[41].Value = FormatConvert.ToString(result.Value.mu_pg);
                    VariantsComboBox.Items.Add("Вариант " + i.ToString());
                    i++;
                }
                await Task.Run(() => 
                { 
                    Trajectories.Clear();
                    var id = GetCalculationInputData();
                    for (int j = 0; j < Result.Count; j++)
                    {
                        var Key = Result.Keys.ToList()[j];
                        var Args = new CalculateTrajectoryEventArgs(Key, Result[Key], id);
                        CalculateTrajectory(this, Args);
                        Trajectories.Add(Key, Args.Trajectory);
                    }
                });
                VariantsComboBox.SelectedIndex = 0;
                VariantsComboBox_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }
        async void DoCalculate()
        {
            var id = GetCalculationInputData();
            Result = await Task.Run<Dictionary<StagesVariableValues, OutputParameters>>(() => 
            {
                var Args = new CalculateEventArgs(id, cts);
                Calculate(this, Args);
                return Args.Result;
            });
            if ((Result == null) || (Result.Count == 0))
                label65.Text = "Состояние: вывод на требуемую орбиту при заданных параметрах невозможен";
            else
                label65.Text = "Состояние: расчёт окончен";
            FillCalculationResult();
            расчётToolStripMenuItem.Text = "Начать расчёт";
        }
        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (расчётToolStripMenuItem.Text == "Начать расчёт")
            {
                cts = new CancellationTokenSource();
                StartTime = DateTime.Now;
                CurrentProgress = 0;
                DoCalculate();
                расчётToolStripMenuItem.Text = "Остановить расчёт";
            }
            else
            {
                cts.Cancel();
                расчётToolStripMenuItem.Text = "Начать расчёт";
            }
        }
        private void VariantsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Key = Result.Keys.ToList()[VariantsComboBox.SelectedIndex];
            calculationResultControl.SetCalculationResult(Key, Result[Key]);
            Action<ZedGraph.ZedGraphControl> Clear = Graph =>
            {
                foreach (var curve in Graph.GraphPane.CurveList)
                    curve.Clear();
            };
            Action<ZedGraph.ZedGraphControl> Refresh = Graph =>
            {
                Graph.Invalidate();
                Graph.GraphPane.AxisChange();
            };
            var Graphs = new List<ZedGraph.ZedGraphControl>() { VelocityGraph, CoordinatesGraph, TrajectoryGraph, AngleGraph, HeightGraph, MassGraph };
            foreach (var graph in Graphs)
                Clear(graph);
            foreach (var tpar in Trajectories[Key])
            {
                CurveItems[CurveType.v_char].Curve.AddPoint(tpar.Time, tpar.v_char * 1E-3);
                CurveItems[CurveType.v_grav].Curve.AddPoint(tpar.Time, tpar.v_grav * 1E-3);
                CurveItems[CurveType.v_aer].Curve.AddPoint(tpar.Time, tpar.v_aer * 1E-3);
                CurveItems[CurveType.v_press].Curve.AddPoint(tpar.Time, tpar.v_press * 1E-3);
                CurveItems[CurveType.v_rel].Curve.AddPoint(tpar.Time, (tpar.v_char - tpar.v_grav - tpar.v_aer - tpar.v_press) * 1E-3);
                CurveItems[CurveType.v_abs].Curve.AddPoint(tpar.Time, (tpar.v_char - tpar.v_grav - tpar.v_aer - tpar.v_press + 7.2921E-5 * 6371E3) * 1E-3);
                MassGraph.GraphPane.CurveList[0].AddPoint(tpar.Time, tpar.m * 1E-3);
                CurveItems[CurveType.x].Curve.AddPoint(tpar.Time, tpar.x * 1E-3);
                CurveItems[CurveType.y].Curve.AddPoint(tpar.Time, tpar.y * 1E-3);
                CurveItems[CurveType.phi].Curve.AddPoint(tpar.Time, tpar.phi * 180 / Math.PI);
                CurveItems[CurveType.xi].Curve.AddPoint(tpar.Time, tpar.xi * 180 / Math.PI);
                CurveItems[CurveType.nu].Curve.AddPoint(tpar.Time, tpar.nu * 180 / Math.PI);
                TrajectoryGraph.GraphPane.CurveList[0].AddPoint(tpar.x * 1E-3, tpar.y * 1E-3);
                HeightGraph.GraphPane.CurveList[0].AddPoint(tpar.Time, tpar.h * 1E-3);
            }
            foreach (var graph in Graphs)
                Refresh(graph);
        }
        private void SaveCalculationMenu_Click(object sender, EventArgs e)
        {
            var SaveDialog = new SaveFileDialog();
            if (SaveDialog.ShowDialog() == DialogResult.OK)
                Save(this, new SaveEventArgs(SaveDialog.FileName, GetCalculationInputData(), Result));
        }
        private async void SaveBallisticsMenu_Click(object sender, EventArgs e)
        {
            var SaveDialog = new SaveFileDialog();
            var Key = Trajectories.Keys.ToList()[VariantsComboBox.SelectedIndex];
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                StartTime = DateTime.Now;
                CurrentProgress = 0;
                await Task.Run(() => SaveBallistics(this, new SaveBallisticsEventArgs(
                    SaveDialog.FileName,
                    Trajectories[Key])));
            }
            label65.Text = "Готово";
        }
        private void SetCalculationInputData(CalculationInputData id)
        {
            
            constantParametersControl.PayloadMass = id.M_pg;
            constantParametersControl.Height = id.H;
            constantParametersControl.Velocity = id.V;
            constantParametersControl.Angle = id.nu;
            FirstStageConstantParametersControl.VoidSpecificImpulse = id.FirstStageConstantParameters.I_p;
            FirstStageConstantParametersControl.SpecificImpulsesRatio = id.FirstStageConstantParameters.l;
            FirstStageConstantParametersControl.MidshipLoad = id.FirstStageConstantParameters.P_m;
            FirstStageConstantParametersControl.FuelDensity = id.FirstStageConstantParameters.rg;
            FirstStageConstantParametersControl.OxidizeDensity = id.FirstStageConstantParameters.ro;
            FirstStageConstantParametersControl.FuelRatio = id.FirstStageConstantParameters.kd;
            SecondStageConstantParametersControl.VoidSpecificImpulse = id.SecondStageConstantParameters.I_p;
            SecondStageConstantParametersControl.SpecificImpulsesRatio = id.SecondStageConstantParameters.l;
            SecondStageConstantParametersControl.MidshipLoad = id.SecondStageConstantParameters.P_m;
            SecondStageConstantParametersControl.FuelDensity = id.SecondStageConstantParameters.rg;
            SecondStageConstantParametersControl.OxidizeDensity = id.SecondStageConstantParameters.ro;
            SecondStageConstantParametersControl.FuelRatio = id.SecondStageConstantParameters.kd;
            variableParametersControl[FirstStageVariableParameterType.n_0].Set(id.FirstStageVariableParameters.n_0);
            variableParametersControl[FirstStageVariableParameterType.a_to].Set(id.FirstStageVariableParameters.a_to);
            variableParametersControl[FirstStageVariableParameterType.mu_su_pr].Set(id.FirstStageVariableParameters.mu_su_pr);
            variableParametersControl[FirstStageVariableParameterType.gamma_du].Set(id.FirstStageVariableParameters.gamma_du);
            variableParametersControl[FirstStageVariableParameterType.mu_k].Set(id.FirstStageVariableParameters.mu_k);
            variableParametersControl[SecondStageVariableParameterType.n_0].Set(id.SecondStageVariableParameters.n_0);
            variableParametersControl[SecondStageVariableParameterType.a_to].Set(id.SecondStageVariableParameters.a_to);
            variableParametersControl[SecondStageVariableParameterType.mu_su_pr].Set(id.SecondStageVariableParameters.mu_su_pr);
            variableParametersControl[SecondStageVariableParameterType.gamma_du].Set(id.SecondStageVariableParameters.gamma_du);
            variableParametersControl[TrajectoryVariableParameterType.phi_k1].Set(id.phi_k1);
            variableParametersControl[TrajectoryVariableParameterType.phi_k2].Set(id.phi_k2);
            variableParametersControl[TrajectoryVariableParameterType.phi_0].Set(id.phi_0);
        }
        private void OpenMenu_Click(object sender, EventArgs e)
        {
            var OpenDialog = new OpenFileDialog();
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                var Args = new OpenEventArgs(OpenDialog.FileName);
                Open(this, Args);
                Result = Args.Result;
                FillCalculationResult();
                SetCalculationInputData(Args.id);
            }
        }
        private void ExampleMenu_Click(object sender, EventArgs e)
        {
            var Args = new OpenExampleEventArgs();
            OpenExample(this, Args);
            Result = Args.Result;
            FillCalculationResult();
            SetCalculationInputData(Args.id);
        }
        class CustomToolStripMenuItem : ToolStripMenuItem
        {
            CurveType Type;
            public CustomToolStripMenuItem(CurveType Type)
            {
                Text = CurveItems[Type].Name;
                CheckState = CurveItems[Type].Curve.IsVisible ? CheckState.Checked : CheckState.Unchecked;
                this.Type = Type;
                Click += MenuItem_Click;
            }
            void MenuItem_Click(object sender, EventArgs e)
            {
                var Type = ((CustomToolStripMenuItem)sender).Type;
                CurveItems[Type].Curve.IsVisible = !CurveItems[Type].Curve.IsVisible;
                CurveItems[Type].Graph.GraphPane.AxisChange();
                CurveItems[Type].Graph.Invalidate();
            }
        }
        public void SetProgress(double Progress)
        {
            this.Invoke(new Action(() =>
            {
                var SpendTime = DateTime.Now - StartTime;
                int LeftTime = (int)(SpendTime.TotalSeconds * (100 / Progress - 1));
                if (Progress - CurrentProgress > 0.5)
                {
                    label65.Text = "Состояние: Оставшееся время примерно " + TimeSpan.FromSeconds(LeftTime).ToString();
                    CurrentProgress = Progress;
                }
                progressBar2.Value = (int)Progress;
            }));
        }
        public event EventHandler<CalculateEventArgs> Calculate;
        public event EventHandler<CalculateTrajectoryEventArgs> CalculateTrajectory;
        public event EventHandler<OpenEventArgs> Open;
        public event EventHandler<OpenExampleEventArgs> OpenExample;
        public event EventHandler<SaveEventArgs> Save;
        public event EventHandler<SaveBallisticsEventArgs> SaveBallistics;
    }
}