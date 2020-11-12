using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TypesLibrary;
using Excel = Microsoft.Office.Interop.Excel;

namespace ModelLibrary
{
    static class CalculationIO
    {
        public static void Save(string FileName, List<TrajectoryParameters> Trajectory, Action<double> ProgressChanged)
        {
            var app = new Excel.Application();
            app.Visible = false;
            app.DisplayAlerts = false;
            var Book = app.Workbooks.Add(1);
            var Sheet = (Excel.Worksheet)Book.Sheets[1];
            Sheet.Cells[1, 1] = "Время, с";
            Sheet.Cells[1, 2] = "Характеристическая скорость, м/с";
            Sheet.Cells[1, 3] = "Относительная скорсть, м/с";
            Sheet.Cells[1, 4] = "Абсолютная скорсть, м/с";
            Sheet.Cells[1, 5] = "Гравитационные потери, м/с";
            Sheet.Cells[1, 6] = "Аэродинамические потери, м/с";
            Sheet.Cells[1, 7] = "Потери на противодавление, с";
            Sheet.Cells[1, 8] = "Высота, м";
            Sheet.Cells[1, 9] = "Координата x, м";
            Sheet.Cells[1, 10] = "Координата y, м";
            Sheet.Cells[1, 11] = "Масса ракеты, кг";
            Sheet.Cells[1, 12] = "Угол тангажа, град.";
            Sheet.Cells[1, 13] = "Угол пути, град.";
            Sheet.Cells[1, 14] = "Угол наклона вектора скорости к местному горизонту, град.";
            long i = 2;
            long k = 0;
            double CurrentProgress = 0;
            double CurrentTime = -1;
            foreach (var tpar in Trajectory)
            {
                if (tpar.Time - CurrentTime > 0.5)
                {
                    Sheet.Cells[i, 1] = tpar.Time;
                    Sheet.Cells[i, 2] = tpar.v_char;
                    Sheet.Cells[i, 3] = tpar.v_char - tpar.v_grav - tpar.v_aer - tpar.v_press;
                    Sheet.Cells[i, 4] = tpar.v_char - tpar.v_grav - tpar.v_aer - tpar.v_press + 7.2921E-5 * 6371E3;
                    Sheet.Cells[i, 5] = tpar.v_grav;
                    Sheet.Cells[i, 6] = tpar.v_aer;
                    Sheet.Cells[i, 7] = tpar.v_press;
                    Sheet.Cells[i, 8] = tpar.h;
                    Sheet.Cells[i, 9] = tpar.x;
                    Sheet.Cells[i, 10] = tpar.y;
                    Sheet.Cells[i, 11] = tpar.m;
                    Sheet.Cells[i, 12] = tpar.phi * 180 / Math.PI;
                    Sheet.Cells[i, 13] = tpar.xi * 180 / Math.PI;
                    Sheet.Cells[i, 14] = tpar.nu * 180 / Math.PI;
                    CurrentTime = tpar.Time;
                    i++;
                }
                k++;
                double Progress = k * 100.0 / Trajectory.Count;
                if ((Progress - CurrentProgress) > 1)
                {
                    CurrentProgress = Progress;
                    ProgressChanged(Progress);
                }
            }
            ProgressChanged(100);
            Book.SaveAs(FileName);
            Book.Close();
            app.Quit();
        }
        public static void Save(string FileName, CalculationInputData id, Dictionary<StagesVariableValues, OutputParameters> result)
        {
            var XML = new XmlDocument();
            Action<XmlNode, string, double> AddNodeWithValue = (Node, name, value) =>
            {
                var node = XML.CreateElement(name);
                node.InnerText = value.ToString();
                Node.AppendChild(node);
            };
            Action<XmlNode, string, Dictionary<string, double>> AddNodeWithAttributes = (Node, name, attributes) =>
            {
                var node = XML.CreateElement(name);
                foreach (var attr in attributes)
                {
                    var Attribute = XML.CreateAttribute(attr.Key);
                    Attribute.InnerText = attr.Value.ToString();
                    node.Attributes.Append(Attribute);
                }
                Node.AppendChild(node);
            };
            Action<XmlNode, string, StageConstantParameters> AddStageConstantParametersNode = (Node, name, parameters) =>
            {
                AddNodeWithAttributes(Node, name, new Dictionary<string, double>()
                {
                    { "VoidSpecificImpulse", parameters.I_p },
                    { "SpecificImpulsesRatio", parameters.l },
                    { "MidshipLoad", parameters.P_m },
                    { "FuelDensity", parameters.rg },
                    { "OxidizeDensity", parameters.ro },
                    { "FuelRatio", parameters.kd }
                });
            };
            Action<XmlNode, string, VariableParameter> AddVariableParameterNode = (Node, name, parameter) =>
            {
                AddNodeWithAttributes(Node, name, new Dictionary<string, double>()
                {
                    { "StartValue", parameter.Start },
                    { "EndValue",   parameter.End }
                });
            };
            Action<XmlNode, string, StageVariableParameters> AddStageVariableParametersNode = (Node, name, parameters) =>
            {
                var node = XML.CreateElement(name);
                AddVariableParameterNode(node, "InitialThrustToWeightRatio", parameters.n_0);
                AddVariableParameterNode(node, "FuelBayRatio", parameters.a_to);
                AddVariableParameterNode(node, "OtherSystemsRatio", parameters.mu_su_pr);
                AddVariableParameterNode(node, "EngineBayRatio", parameters.gamma_du);
                if (parameters is FirstStageVariableParameters)
                    AddVariableParameterNode(node, "EndMassRatio", (parameters as FirstStageVariableParameters).mu_k);
                Node.AppendChild(node);
            };
            Func<XmlNode, string, StagesVariableValues, OutputParameters, XmlNode> AddCalculationResultNode = (Node, name, VariableValues, OutputParameters) =>
            {
                var CalculationResultNode = XML.CreateElement(name);
                Action<string, StageVariableValues> AddStageVariableNode = (StageVariableNodeName, StageVariableValues) =>
                {
                    var dict = new Dictionary<string, double>()
                    {
                        { "InitialThrustToWeightRatio", StageVariableValues.n_0 },
                        { "FuelBayRatio", StageVariableValues.a_to },
                        { "OtherSystemsRatio", StageVariableValues.mu_su_pr },
                        { "EngineBayRatio", StageVariableValues.gamma_du }
                    };
                    if (StageVariableValues is FirstStageVariableValues)
                        dict.Add("EndMassRatio", (StageVariableValues as FirstStageVariableValues).mu_k);
                    AddNodeWithAttributes(CalculationResultNode, StageVariableNodeName, dict);
                };
                Action<string, StageOutputData> AddStageOutputDataNode = (StageOutputDataNodeName, StageOutputData) =>
                {
                    var StageOutputDataNode = XML.CreateElement(StageOutputDataNodeName);
                    AddNodeWithValue(StageOutputDataNode, "Thrust", StageOutputData.P_0);
                    AddNodeWithValue(StageOutputDataNode, "InitialMass", StageOutputData.m_0);
                    AddNodeWithValue(StageOutputDataNode, "EndMass", StageOutputData.m_k);
                    AddNodeWithValue(StageOutputDataNode, "EngineBayMass", StageOutputData.m_du);
                    AddNodeWithValue(StageOutputDataNode, "CombustibleMass", StageOutputData.m_g);
                    AddNodeWithValue(StageOutputDataNode, "OxidizeMass", StageOutputData.m_o);
                    AddNodeWithValue(StageOutputDataNode, "FuelMass", StageOutputData.m_t);
                    AddNodeWithValue(StageOutputDataNode, "FuelBayMass", StageOutputData.m_to);
                    AddNodeWithValue(StageOutputDataNode, "OtherSystemsMass", StageOutputData.m_su_pr);
                    AddNodeWithValue(StageOutputDataNode, "DryMass", StageOutputData.m_sux);
                    AddNodeWithValue(StageOutputDataNode, "DryMassRatio", StageOutputData.mu_sux);
                    if (StageOutputData is SecondStageOutputData)
                        AddNodeWithValue(StageOutputDataNode, "EndMassRatio", (StageOutputData as SecondStageOutputData).mu_k);
                    CalculationResultNode.AppendChild(StageOutputDataNode);
                };
                AddStageVariableNode("FirstStageVariableValues", VariableValues.firstStageVariableValues);
                AddStageVariableNode("SecondStageVariableValues", VariableValues.secondStageVariableValues);
                AddStageOutputDataNode("FirstStageOutputData", OutputParameters.FirstStageOutputData);
                AddStageOutputDataNode("SecondStageOutputData", OutputParameters.SecondStageOutputData);
                var TrajectoryParametersNode = XML.CreateElement("TrajectoryParameters");
                AddNodeWithAttributes(TrajectoryParametersNode, "EndPoint", new Dictionary<string, double>()
                {
                    { "Height", OutputParameters.TrajectoryOutputParameters.H },
                    { "Velocity", OutputParameters.TrajectoryOutputParameters.V },
                    { "Angle", OutputParameters.TrajectoryOutputParameters.nu }
                });
                AddNodeWithAttributes(TrajectoryParametersNode, "FlightTime", new Dictionary<string, double>()
                {
                    { "FirstStage", OutputParameters.TrajectoryOutputParameters.t_1 },
                    { "SecondStage", OutputParameters.TrajectoryOutputParameters.t_2 }
                });
                AddNodeWithAttributes(TrajectoryParametersNode, "PitchProgram", new Dictionary<string, double>()
                {
                    { "FirstStageEndPitchAngle", OutputParameters.TrajectoryOutputParameters.phi_k1 },
                    { "SecondStageInitialPitchAngle", OutputParameters.TrajectoryOutputParameters.phi_0 },
                    { "SecondStageEndPitchAngle", OutputParameters.TrajectoryOutputParameters.phi_k2 }
                });
                CalculationResultNode.AppendChild(TrajectoryParametersNode);
                AddNodeWithValue(CalculationResultNode, "PayLoadMassRatio", OutputParameters.mu_pg);
                Node.AppendChild(CalculationResultNode);
                return CalculationResultNode;
            };
            Action<XmlNode, string, Dictionary<StagesVariableValues, OutputParameters>> AddPossibleSolutionsNode = (Node, Name, OutputParametersArray) =>
            {
                var node = XML.CreateElement(Name);
                int i = 1;
                foreach (var outputParameters in OutputParametersArray)
                {
                    AddCalculationResultNode(node, "Solution_" + i.ToString(), outputParameters.Key, outputParameters.Value);
                    i++;
                }
                Node.AppendChild(node);
            };
            var Root = XML.CreateElement("Calculation");
            var CalculationInputDataNode = XML.CreateElement("CalculationInputData");
            AddNodeWithValue(CalculationInputDataNode, "PayloadMass", id.M_pg);
            AddNodeWithAttributes(CalculationInputDataNode, "EndPoint", new Dictionary<string, double>()
            {
                { "Height", id.H },
                { "Velocity", id.V },
                { "Angle", id.nu }
            });
            AddVariableParameterNode(CalculationInputDataNode, "FirstStageEndPitchAngle", id.phi_k1);
            AddVariableParameterNode(CalculationInputDataNode, "SecondStageInitialPitchAngle", id.phi_0);
            AddVariableParameterNode(CalculationInputDataNode, "SecondStageEndPitchAngle", id.phi_k2);
            AddStageConstantParametersNode(CalculationInputDataNode, "FirstStageConstantParameters", id.FirstStageConstantParameters);
            AddStageConstantParametersNode(CalculationInputDataNode, "SecondStageConstantParameters", id.SecondStageConstantParameters);
            AddStageVariableParametersNode(CalculationInputDataNode, "FirstStageVariableParameters", id.FirstStageVariableParameters);
            AddStageVariableParametersNode(CalculationInputDataNode, "SecondStageVariableParameters", id.SecondStageVariableParameters);
            Root.AppendChild(CalculationInputDataNode);
            if ((result != null) && (result.Count != 0))
            {
                var CalculationResultNode = XML.CreateElement("CalculationResult");
                AddPossibleSolutionsNode(CalculationResultNode, "PossibleSolutions", result);
                Root.AppendChild(CalculationResultNode);
            }
            XML.AppendChild(Root);
            XML.Save(FileName);
        }
        public static void Open(XmlDocument XML, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io)
        {
            Func<XmlNode, VariableParameter> GetVariableParameter = Node => new VariableParameter(
                Convert.ToDouble(Node.Attributes["StartValue"].InnerText),
                Convert.ToDouble(Node.Attributes["EndValue"].InnerText));
            Func<XmlNode, StageConstantParameters> GetStageConstantParameters = Node =>
            {
                var Parameters = new StageConstantParameters();
                Parameters.I_p = Convert.ToDouble(Node.Attributes["VoidSpecificImpulse"].InnerText);
                Parameters.l = Convert.ToDouble(Node.Attributes["SpecificImpulsesRatio"].InnerText);
                Parameters.P_m = Convert.ToDouble(Node.Attributes["MidshipLoad"].InnerText);
                Parameters.rg = Convert.ToDouble(Node.Attributes["FuelDensity"].InnerText);
                Parameters.ro = Convert.ToDouble(Node.Attributes["OxidizeDensity"].InnerText);
                Parameters.kd = Convert.ToDouble(Node.Attributes["FuelRatio"].InnerText);
                return Parameters;
            };
            Action<XmlNode, StageVariableParameters> FillStageVariableParameters = (Node, Parameters) =>
            {
                Parameters.n_0 = GetVariableParameter(Node.FindChildByName("InitialThrustToWeightRatio"));
                Parameters.a_to = GetVariableParameter(Node.FindChildByName("FuelBayRatio"));
                Parameters.mu_su_pr = GetVariableParameter(Node.FindChildByName("OtherSystemsRatio"));
                Parameters.gamma_du = GetVariableParameter(Node.FindChildByName("EngineBayRatio"));
                if (Parameters is FirstStageVariableParameters)
                    (Parameters as FirstStageVariableParameters).mu_k = GetVariableParameter(Node.FindChildByName("EndMassRatio"));
            };
            var idNode = XML.ChildNodes[0].ChildNodes[0];
            id = new CalculationInputData
            {
                M_pg = Convert.ToDouble(idNode.FindChildByName("PayloadMass").InnerText),
                H = Convert.ToDouble(idNode.FindChildByName("EndPoint").Attributes["Height"].InnerText),
                V = Convert.ToDouble(idNode.FindChildByName("EndPoint").Attributes["Velocity"].InnerText),
                nu = Convert.ToDouble(idNode.FindChildByName("EndPoint").Attributes["Angle"].InnerText),
                phi_k1 = GetVariableParameter(idNode.FindChildByName("FirstStageEndPitchAngle")),
                phi_0 = GetVariableParameter(idNode.FindChildByName("SecondStageInitialPitchAngle")),
                phi_k2 = GetVariableParameter(idNode.FindChildByName("SecondStageEndPitchAngle")),
                FirstStageConstantParameters = GetStageConstantParameters(idNode.FindChildByName("FirstStageConstantParameters")),
                SecondStageConstantParameters = GetStageConstantParameters(idNode.FindChildByName("SecondStageConstantParameters")),
                FirstStageVariableParameters = new FirstStageVariableParameters(),
                SecondStageVariableParameters = new SecondStageVariableParameters()
            };
            FillStageVariableParameters(idNode.FindChildByName("FirstStageVariableParameters"), id.FirstStageVariableParameters);
            FillStageVariableParameters(idNode.FindChildByName("SecondStageVariableParameters"), id.SecondStageVariableParameters);
            if (XML.ChildNodes[0].ChildNodes.Count == 1)
            {
                io = null;
                return;
            }
            var ioNode = XML.ChildNodes[0].ChildNodes[1];
            Action<XmlNode, StageVariableValues> FillStageVariableValues = (Node, Parameters) =>
            {
                Parameters.n_0 = Convert.ToDouble(Node.Attributes["InitialThrustToWeightRatio"].InnerText);
                Parameters.a_to = Convert.ToDouble(Node.Attributes["FuelBayRatio"].InnerText);
                Parameters.mu_su_pr = Convert.ToDouble(Node.Attributes["OtherSystemsRatio"].InnerText);
                Parameters.gamma_du = Convert.ToDouble(Node.Attributes["EngineBayRatio"].InnerText);
                if (Parameters is FirstStageVariableValues)
                    (Parameters as FirstStageVariableValues).mu_k = Convert.ToDouble(Node.Attributes["EndMassRatio"].InnerText);
            };
            Action<XmlNode, StageOutputData> FillStageOutputData = (Node, Parameters) =>
            {
                Parameters.P_0 = Convert.ToDouble(Node.FindChildByName("Thrust").InnerText);
                Parameters.m_0 = Convert.ToDouble(Node.FindChildByName("InitialMass").InnerText);
                Parameters.m_k = Convert.ToDouble(Node.FindChildByName("EndMass").InnerText);
                Parameters.m_du = Convert.ToDouble(Node.FindChildByName("EngineBayMass").InnerText);
                Parameters.m_g = Convert.ToDouble(Node.FindChildByName("CombustibleMass").InnerText);
                Parameters.m_o = Convert.ToDouble(Node.FindChildByName("OxidizeMass").InnerText);
                Parameters.m_t = Convert.ToDouble(Node.FindChildByName("FuelMass").InnerText);
                Parameters.m_to = Convert.ToDouble(Node.FindChildByName("FuelBayMass").InnerText);
                Parameters.m_su_pr = Convert.ToDouble(Node.FindChildByName("OtherSystemsMass").InnerText);
                Parameters.m_sux = Convert.ToDouble(Node.FindChildByName("DryMass").InnerText);
                Parameters.mu_sux = Convert.ToDouble(Node.FindChildByName("DryMassRatio").InnerText);
                if (Parameters is SecondStageOutputData)
                    (Parameters as SecondStageOutputData).mu_k = Convert.ToDouble(Node.FindChildByName("EndMassRatio").InnerText);
            };
            Func<XmlNode, TrajectoryOutputParameters> GetTrajectoryParameters = Node => new TrajectoryOutputParameters
            {
                H = Convert.ToDouble(Node.FindChildByName("EndPoint").Attributes["Height"].InnerText),
                V = Convert.ToDouble(Node.FindChildByName("EndPoint").Attributes["Velocity"].InnerText),
                nu = Convert.ToDouble(Node.FindChildByName("EndPoint").Attributes["Angle"].InnerText),
                t_1 = Convert.ToDouble(Node.FindChildByName("FlightTime").Attributes["FirstStage"].InnerText),
                t_2 = Convert.ToDouble(Node.FindChildByName("FlightTime").Attributes["SecondStage"].InnerText),
                phi_k1 = Convert.ToDouble(Node.FindChildByName("PitchProgram").Attributes["FirstStageEndPitchAngle"].InnerText),
                phi_0 = Convert.ToDouble(Node.FindChildByName("PitchProgram").Attributes["SecondStageInitialPitchAngle"].InnerText),
                phi_k2 = Convert.ToDouble(Node.FindChildByName("PitchProgram").Attributes["SecondStageEndPitchAngle"].InnerText)
            };
            Func<XmlNode, KeyValuePair<StagesVariableValues, OutputParameters>> GetOutputParameters = Node =>
            {
                var stagesVariableValues = new StagesVariableValues
                {
                    firstStageVariableValues = new FirstStageVariableValues()
                };
                FillStageVariableValues(Node.FindChildByName("FirstStageVariableValues"), stagesVariableValues.firstStageVariableValues);
                stagesVariableValues.secondStageVariableValues = new SecondStageVariableValues();
                FillStageVariableValues(Node.FindChildByName("SecondStageVariableValues"), stagesVariableValues.secondStageVariableValues);
                var outputParameters = new OutputParameters
                {
                    FirstStageOutputData = new FirstStageOutputData()
                };
                FillStageOutputData(Node.FindChildByName("FirstStageOutputData"), outputParameters.FirstStageOutputData);
                outputParameters.SecondStageOutputData = new SecondStageOutputData();
                FillStageOutputData(Node.FindChildByName("SecondStageOutputData"), outputParameters.SecondStageOutputData);
                outputParameters.TrajectoryOutputParameters = GetTrajectoryParameters(Node.FindChildByName("TrajectoryParameters"));
                outputParameters.mu_pg = Convert.ToDouble(Node.FindChildByName("PayLoadMassRatio").InnerText);
                return new KeyValuePair<StagesVariableValues, OutputParameters>(stagesVariableValues, outputParameters);
            };
            io = new Dictionary<StagesVariableValues, OutputParameters>();
            foreach (var node in ioNode.FindChildByName("PossibleSolutions").ChildNodes.Cast<XmlNode>())
            {
                var OutputParameters = GetOutputParameters(node);
                io.Add(OutputParameters.Key, OutputParameters.Value);
            }
        }
        public static void Open(string FileName, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io)
        {
            var XML = new XmlDocument();
            XML.Load(FileName);
            Open(XML, out id, out io);
        }
    }
}