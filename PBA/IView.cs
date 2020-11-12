using System;
using System.Collections.Generic;
using System.Threading;
using TypesLibrary;

namespace ViewInterface
{
    public interface IView
    {
        void SetProgress(double Progress);
        event EventHandler<CalculateEventArgs> Calculate;
        event EventHandler<CalculateTrajectoryEventArgs> CalculateTrajectory;
        event EventHandler<OpenEventArgs> Open;
        event EventHandler<OpenExampleEventArgs> OpenExample;
        event EventHandler<SaveEventArgs> Save;
        event EventHandler<SaveBallisticsEventArgs> SaveBallistics;
    }
    public class CalculateEventArgs : EventArgs
    {
        public CalculationInputData id { get; private set; }
        public CancellationTokenSource cts { get; private set; }
        public Dictionary<StagesVariableValues, OutputParameters> Result { get; set; }
        public CalculateEventArgs(CalculationInputData id , CancellationTokenSource cts) : base()
        {
            this.id = id;
            this.cts = cts;
        }
    }
    public class CalculateTrajectoryEventArgs : EventArgs
    {
        public StagesVariableValues StagesVariableValues { get; private set; }
        public OutputParameters OutputParameters { get; private set; }
        public CalculationInputData id { get; private set; }
        public List<TrajectoryParameters> Trajectory { get; set; }
        public CalculateTrajectoryEventArgs(
            StagesVariableValues StagesVariableValues, 
            OutputParameters OutputParameters, 
            CalculationInputData id) : base()
        {
            this.StagesVariableValues = StagesVariableValues;
            this.OutputParameters = OutputParameters;
            this.id = id;
        }
    }
    public class OpenEventArgs : EventArgs
    {
        public string FileName { get; private set;}
        public CalculationInputData id { get; set; }
        public Dictionary<StagesVariableValues, OutputParameters> Result { get; set; }
        public OpenEventArgs(string FileName) : base()
        {
            this.FileName = FileName;
        }
    }
    public class OpenExampleEventArgs : EventArgs
    {
        public CalculationInputData id { get; set; }
        public Dictionary<StagesVariableValues, OutputParameters> Result { get; set; }
    }
    public class SaveEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public CalculationInputData id { get; private set; }
        public Dictionary<StagesVariableValues, OutputParameters> Result { get; private set; }
        public SaveEventArgs(
            string FileName, 
            CalculationInputData id, 
            Dictionary<StagesVariableValues, OutputParameters> Result) : base()
        {
            this.FileName = FileName;
            this.id = id;
            this.Result = Result;
        }
    }
    public class SaveBallisticsEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public List<TrajectoryParameters> Trajectory { get; private set; }
        public SaveBallisticsEventArgs(string FileName, List<TrajectoryParameters> Trajectory) : base()
        {
            this.FileName = FileName;
            this.Trajectory = Trajectory;
        }
    }
}
