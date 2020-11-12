using ModelInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TypesLibrary;

namespace ModelLibrary
{
    public class Model : IModel
    {
        readonly double R = 6371E3;
        readonly double g = 9.807;
        double Cx(double M)
        {
            double[] A;
            if (M <= 0.4)
                A = new double[] { 0.2901, 0.0013, -0.0357, 0, 0 };
            else if (M <= 0.9)
                A = new double[] { 0.7921, -1.9414, 1.6607, 0, 0 };
            else if (M <= 1.5)
                A = new double[] { 29.601, -105.27, 138.34, -78.503, 16.326 };
            else
                A = new double[] { 0.955, -0.25, 0.0361, -0.00225, 5.11E-5 };
            return A.Zip(new double[] { 0, 1, 2, 3, 4 }, (a, n) => a * Math.Pow(M, n)).Sum();
        }
        Func<double, double> phi_1(double phi_k, double t_k)
        {
            return t => t < 25 ?
                Math.PI / 2 :
                phi_k + (Math.PI / 2 - phi_k) / Math.Pow(t_k - 25, 2) * Math.Pow(t_k - t, 2);
        }
        Func<double, double> phi_2(double phi_0, double phi_k, double t_k)
        {
            return t => phi_0 + (phi_k - phi_0) / t_k * t;
        }
        Func<double[], double, double[]> GetF(double n_0, double I_p, double l, double P_m, Func<double, double> phi)
        {
            return (X, t) =>
            {
                double v_char = X[0];
                double v_grav = X[1];
                double v_aer = X[2];
                double v_press = X[3];
                double mu = X[4];
                double x = X[5];
                double y = X[6];
                double v = v_char - v_grav - v_aer - v_press;
                double r = Math.Sqrt(Math.Pow(R + y, 2) + x * x);
                double h = r - R;
                var AtmospehereParameters = Atmosphere.ParametersAtHeight(h);
                var phi_t = phi(t);
                return new double[]
                {
                    n_0 * g / (l * mu),
                    g * Math.Pow(R / r, 2) * Math.Sin(phi_t + Math.Atan(x / (R + y))),
                    Cx(v / AtmospehereParameters.SoundVelosity) * AtmospehereParameters.Density * v * v / (2 * mu * P_m),
                    n_0 * g / mu * (1 / l - 1) * AtmospehereParameters.Pressure / 1.0125E5,
                    -n_0 * g / (I_p * l),
                    v * Math.Cos(phi_t),
                    v * Math.Sin(phi_t)
                };
            };
        }
        public OutputParameters Get_mu_k2(
            StagesVariableValues vid,
            StageConstantParameters cid_1,
            StageConstantParameters cid_2,
            double phi_k1,
            double phi_k2,
            double phi_0,
            double V,
            double M_pg)
        {
            double t_k1 = (1 - vid.firstStageVariableValues.mu_k) / (g * vid.firstStageVariableValues.n_0) * cid_1.I_p * cid_1.l;
            var F1 = GetF(vid.firstStageVariableValues.n_0, cid_1.I_p, cid_1.l, cid_1.P_m, phi_1(phi_k1, t_k1));
            double t_1 = 0;
            double dt = 0.1;
            var X_1 = new double[] { 0, 0, 0, 0, 1, 0, 0 };
            while (t_1 < t_k1)
            {
                X_1 = X_1.Zip(RungeKuttaIteration.Get(X_1, F1, t_1, dt), (x, dx) => x + dx).ToArray();
                t_1 += dt;
            }
            Func<double, double[]> T = mu_k_2 =>
            {
                double t_k2 = (1 - mu_k_2) / (g * vid.secondStageVariableValues.n_0) * cid_2.I_p * cid_2.l;
                double t_2 = 0;
                var F2 = GetF(vid.secondStageVariableValues.n_0, cid_2.I_p, cid_2.l, cid_2.P_m, phi_2(phi_0, phi_k2, t_k2));
                var x_2 = X_1.Select(x => x).ToArray();
                x_2[4] = 1;
                while (t_2 < t_k2)
                {
                    x_2 = x_2.Zip(RungeKuttaIteration.Get(x_2, F2, t_2, dt), (x, dx) => x + dx).ToArray();
                    t_2 += dt;
                }
                return x_2;
            };
            double mu_k2 = 0.2;
            var X_2 = T(mu_k2);
            double v_char = X_2[0];
            double v_pot = X_2[1] + X_2[2] + X_2[3];
            int N = 1;
            while (Math.Abs(v_char - v_pot + 7.2921E-5 * 6371E3 - V) > 5)
            {
                double v_potr = V + v_pot - 7.2921E-5 * 6371E3;
                mu_k2 = Math.Exp(-(v_potr + cid_1.I_p * Math.Log(vid.firstStageVariableValues.mu_k)) / cid_2.I_p);
                X_2 = T(mu_k2);
                v_char = X_2[0];
                v_pot = X_2[1] + X_2[2] + X_2[3];
                N += 1;
                if (N == 10) return null;
            }
            double r_t1 = (1 + cid_1.kd) / (1 / cid_1.rg + cid_1.kd / cid_1.ro);
            double r_t2 = (1 + cid_2.kd) / (1 / cid_2.rg + cid_2.kd / cid_2.ro);
            double[] mu_0 = new double[]
            {
                1,
                vid.firstStageVariableValues.mu_k - (vid.firstStageVariableValues.a_to / r_t1 * (1 - vid.firstStageVariableValues.mu_k) + 
                    vid.firstStageVariableValues.gamma_du * vid.firstStageVariableValues.n_0 * g) * (1 + vid.firstStageVariableValues.mu_su_pr),
                mu_k2 - (vid.secondStageVariableValues.a_to / r_t2 * (1 - mu_k2) + 
                    vid.secondStageVariableValues.gamma_du * vid.secondStageVariableValues.n_0 * g) * (1 + vid.secondStageVariableValues.mu_su_pr)
            };
            double mu_pg = mu_0[0] * mu_0[1] * mu_0[2];
            if (mu_pg <= 0) return null;
            double m_0 = M_pg / mu_pg;
            var sod_1 = new FirstStageOutputData();
            
            sod_1.m_0 = m_0 * mu_0[0];
            sod_1.m_t = sod_1.m_0 * (1 - vid.firstStageVariableValues.mu_k);
            sod_1.m_g = sod_1.m_t / (1 + cid_1.kd);
            sod_1.m_o = sod_1.m_t / (1 + cid_1.kd) * cid_1.kd;
            sod_1.m_to = vid.firstStageVariableValues.a_to / r_t1 * sod_1.m_t;
            sod_1.P_0 = vid.firstStageVariableValues.n_0 * sod_1.m_0 * g;
            sod_1.m_du = vid.firstStageVariableValues.gamma_du * sod_1.P_0;
            sod_1.m_su_pr = vid.firstStageVariableValues.mu_su_pr * (sod_1.m_to + sod_1.m_du);
            sod_1.m_k = sod_1.m_0 * vid.firstStageVariableValues.mu_k;
            
            var sod_2 = new SecondStageOutputData();
            sod_2.m_0 = m_0 * mu_0[0] * mu_0[1];
            sod_2.m_t = sod_2.m_0 * (1 - mu_k2);
            sod_2.m_g = sod_2.m_t / (1 + cid_2.kd);
            sod_2.m_o = sod_2.m_t / (1 + cid_2.kd) * cid_2.kd;
            sod_2.m_to = vid.secondStageVariableValues.a_to / r_t2 * sod_2.m_t;
            sod_2.P_0 = vid.secondStageVariableValues.n_0 * sod_2.m_0 * g;
            sod_2.m_du = vid.secondStageVariableValues.gamma_du * sod_2.P_0;
            sod_2.m_su_pr = vid.secondStageVariableValues.mu_su_pr * (sod_2.m_to + sod_2.m_du);
            sod_2.m_k = sod_2.m_0 * mu_k2;
            sod_2.mu_k = mu_k2;

            sod_1.m_sux = sod_1.m_k - sod_2.m_0;
            sod_2.m_sux = sod_2.m_k - M_pg;
            sod_1.mu_sux = sod_1.m_sux / sod_1.m_0;
            sod_2.mu_sux = sod_2.m_sux / sod_2.m_0;

            var tod = new TrajectoryOutputParameters()
            {
                H = Math.Sqrt(Math.Pow(R + X_2[6], 2) + Math.Pow(X_2[5], 2)) - R,
                nu = phi_k2 + Math.Atan(X_2[5] / (R + X_2[6])),
                V = v_char - v_pot + 7.2921E-5 * 6371E3,
                phi_0 = phi_0,
                phi_k1 = phi_k1,
                phi_k2 = phi_k2,
                t_1 = t_k1,
                t_2 = (1 - mu_k2) / (g * vid.secondStageVariableValues.n_0) * cid_2.I_p * cid_2.l
            };

            return new OutputParameters()
            {
                mu_pg = mu_pg,
                FirstStageOutputData = sod_1,
                SecondStageOutputData = sod_2,
                TrajectoryOutputParameters = tod
            };
        }
        public List<TrajectoryParameters> GetTrajectory(
            StagesVariableValues StagesVariableValues,
            OutputParameters OutputParameters, 
            CalculationInputData id)
        {
            var Trajectory = new List<TrajectoryParameters>();
            try
            {
                double t_k1 = OutputParameters.TrajectoryOutputParameters.t_1;
                Func<double, double> firstStage_phi = phi_1(OutputParameters.TrajectoryOutputParameters.phi_k1, t_k1);
                var F1 = GetF(
                    StagesVariableValues.firstStageVariableValues.n_0,
                    id.FirstStageConstantParameters.I_p,
                    id.FirstStageConstantParameters.l,
                    id.FirstStageConstantParameters.P_m,
                    firstStage_phi);
                double t_1 = 0;
                double dt = 0.1;
                var X_1 = new double[] { 0, 0, 0, 0, 1, 0, 0 };
                Action<double, double[], double, double> AddTrajectoryParameter = (Time, X, m_0, phi) =>
                {
                    Trajectory.Add(new TrajectoryParameters()
                    {
                        Time = Time,
                        v_char = X[0],
                        v_grav = X[1],
                        v_aer = X[2],
                        v_press = X[3],
                        m = X[4] * m_0,
                        x = X[5],
                        y = X[6],
                        phi = phi,
                        h = Math.Sqrt(Math.Pow(X[5], 2) + Math.Pow(X[6] + R, 2)) - R,
                        xi = Math.Atan(X[5] / (R + X[6])),
                        nu = phi + Math.Atan(X[5] / (R + X[6]))
                    });
                };
                do
                {
                    AddTrajectoryParameter(t_1, X_1, OutputParameters.FirstStageOutputData.m_0, firstStage_phi(t_1));
                    X_1 = X_1.Zip(RungeKuttaIteration.Get(X_1, F1, t_1, dt), (x, dx) => x + dx).ToArray();
                    t_1 += dt;
                } while (t_1 < t_k1);
                double t_k2 = OutputParameters.TrajectoryOutputParameters.t_2;
                Func<double, double> secondStage_phi = phi_2(OutputParameters.TrajectoryOutputParameters.phi_0, OutputParameters.TrajectoryOutputParameters.phi_k2, t_k2);
                var F2 = GetF(
                    StagesVariableValues.secondStageVariableValues.n_0,
                    id.SecondStageConstantParameters.I_p,
                    id.SecondStageConstantParameters.l,
                    id.SecondStageConstantParameters.P_m,
                    secondStage_phi);
                double t_2 = 0;
                var X_2 = X_1;
                X_2[4] = 1;
                while (t_2 < t_k2)
                {
                    X_2 = X_2.Zip(RungeKuttaIteration.Get(X_2, F2, t_2, dt), (x, dx) => x + dx).ToArray();
                    t_2 = t_2 + dt;
                    AddTrajectoryParameter(t_2 + t_k1, X_2, OutputParameters.SecondStageOutputData.m_0, secondStage_phi(t_2));
                }
            }
            catch { }
            return Trajectory;
        }
        public Dictionary<StagesVariableValues, OutputParameters> Get_mu_k2_Orientiered(
            CalculationInputData id, 
            CancellationTokenSource cts)
        {
            Func<VariableParameter, int, List<double>> VariableParameterToList = (VariableParameter, Count) =>
            {
                double Start = VariableParameter.Start;
                double End = VariableParameter.End;
                var List = new List<double>();
                double Delta = (End - Start) / Count;
                if (Delta == 0)
                    List.Add(Start);
                else
                    while (Start <= End)
                    {
                        List.Add(Start);
                        Start += Delta;
                    }
                return List;
            };
            var AllVaribleParametersList = new List<AllVariableParameters>();
            foreach(var _n_01 in VariableParameterToList(id.FirstStageVariableParameters.n_0, 5))
            foreach(var _n_02 in VariableParameterToList(id.SecondStageVariableParameters.n_0, 5))
            foreach(var _a_to1 in VariableParameterToList(id.FirstStageVariableParameters.a_to, 5))
            foreach(var _a_to2 in VariableParameterToList(id.SecondStageVariableParameters.a_to, 5))
            foreach(var _mu_su_pr1 in VariableParameterToList(id.FirstStageVariableParameters.mu_su_pr, 5))
            foreach(var _mu_su_pr2 in VariableParameterToList(id.SecondStageVariableParameters.mu_su_pr, 5))
            foreach(var _gamma_du1 in VariableParameterToList(id.FirstStageVariableParameters.gamma_du, 5))
            foreach(var _gamma_du2 in VariableParameterToList(id.SecondStageVariableParameters.gamma_du, 5))
            foreach(var _mu_k1 in VariableParameterToList(id.FirstStageVariableParameters.mu_k, 5))
            foreach (var _phi_k1 in VariableParameterToList(id.phi_k1, 5))
            foreach (var _phi_0 in VariableParameterToList(id.phi_0, 5))
            foreach (var _phi_k2 in VariableParameterToList(id.phi_k2, 5))
            {
                AllVaribleParametersList.Add(new AllVariableParameters()
                {
                    StagesVariableValues = new StagesVariableValues() 
                    {
                        firstStageVariableValues  = new FirstStageVariableValues()  { n_0 = _n_01, a_to = _a_to1, mu_su_pr = _mu_su_pr1, gamma_du = _gamma_du1, mu_k = _mu_k1 },
                        secondStageVariableValues = new SecondStageVariableValues() { n_0 = _n_02, a_to = _a_to2, mu_su_pr = _mu_su_pr2, gamma_du = _gamma_du2 }
                    },
                    Angles = new Angles() { phi_k1 = _phi_k1, phi_0 = _phi_0, phi_k2 = _phi_k2 }
                });
            }
            var Variants = new List<InputOutputUnion>();
            int N = AllVaribleParametersList.Count;
            int n = 0;
            Dictionary<StagesVariableValues, OutputParameters> Result = null;
            var MaxResult = new KeyValuePair<StagesVariableValues,OutputParameters>(null, null);
            var po = new ParallelOptions() { CancellationToken = cts.Token, MaxDegreeOfParallelism = System.Environment.ProcessorCount };
            try
            {
                Parallel.For(0, N, po, i =>
                {
                    var varibleParameters = AllVaribleParametersList[i];
                    Variants.Add(new InputOutputUnion()
                    { 
                        VaribleParameterValues = varibleParameters.StagesVariableValues, 
                        OutputParameters = Get_mu_k2(
                            varibleParameters.StagesVariableValues,
                            id.FirstStageConstantParameters,
                            id.SecondStageConstantParameters,
                            varibleParameters.Angles.phi_k1,
                            varibleParameters.Angles.phi_k2,
                            varibleParameters.Angles.phi_0,
                            id.V,
                            id.M_pg),
                        Index = i
                    });
                    n++;
                    ProgressChanged(n * 100.0 / N);
                });
            }
            catch (OperationCanceledException e)
            { }
            finally
            {
                var dHmax = Variants.Where(x => x.OutputParameters != null).Max(x => Math.Abs(x.OutputParameters.TrajectoryOutputParameters.H - id.H));
                var dnumax = Variants.Where(x => x.OutputParameters != null).Max(x => Math.Abs(x.OutputParameters.TrajectoryOutputParameters.nu - id.nu));
                Result = Variants
                    .Where(x => x.OutputParameters != null)
                    .OrderBy(x => Math.Sqrt(Math.Pow((x.OutputParameters.TrajectoryOutputParameters.H - id.H) / dHmax, 2) + Math.Pow((x.OutputParameters.TrajectoryOutputParameters.nu - id.nu) / dnumax, 2)))
                    .Take(15)
                    .OrderBy(x => x.Index)
                    .ToDictionary(x => x.VaribleParameterValues, x => x.OutputParameters);
                ProgressChanged(100);
                cts.Dispose();
            }
            return Result;     
        }
        public void Save(string FileName, List<TrajectoryParameters> Trajectory)
        {
            CalculationIO.Save(FileName, Trajectory, ProgressChanged);
        }
        public void Save(string FileName, CalculationInputData id, Dictionary<StagesVariableValues, OutputParameters> result)
        {
            CalculationIO.Save(FileName, id, result);
        }
        public void Open(string FileName, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io)
        {
            CalculationIO.Open(FileName, out id, out io);
        }
        public void Open(XmlDocument XML, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io)
        {
            CalculationIO.Open(XML, out id, out io);
        }

        public event Action<double> ProgressChanged;
    }
}