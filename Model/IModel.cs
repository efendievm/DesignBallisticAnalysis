using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using TypesLibrary;

namespace ModelInterface
{
    public interface IModel
    {
        OutputParameters Get_mu_k2(
            StagesVariableValues vid,
            StageConstantParameters cid_1,
            StageConstantParameters cid_2,
            double phi_k1,
            double phi_k2,
            double phi_0,
            double V,
            double M_pg);
        List<TrajectoryParameters> GetTrajectory(
            StagesVariableValues StagesVariableValues,
            OutputParameters OutputParameters, 
            CalculationInputData id);
        Dictionary<StagesVariableValues, OutputParameters> Get_mu_k2_Orientiered(
            CalculationInputData id, 
            CancellationTokenSource cts);
        void Save(string FileName, List<TrajectoryParameters> Trajectory);
        void Save(
            string FileName,
            CalculationInputData id,
            Dictionary<StagesVariableValues,
            OutputParameters> result);
        void Open(string FileName, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io);
        void Open(XmlDocument XML, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io);
        event Action<double> ProgressChanged;
    }
}
