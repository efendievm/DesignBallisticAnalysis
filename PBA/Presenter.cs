using System;
using System.Collections.Generic;
using ViewInterface;
using ModelInterface;
using TypesLibrary;
using System.Xml;

namespace Presenter
{
    public class Presenter
    {
        IView View;
        IModel Model;
        public Presenter(IView View, IModel Model)
        {
            this.View = View;
            this.Model = Model;
            View.Calculate += View_Calculate;
            View.CalculateTrajectory += View_CalculateTrajectory;
            View.Open += View_Open;
            View.OpenExample += View_OpenExample;
            View.Save += View_Save;
            View.SaveBallistics += View_SaveBallistics;
            Model.ProgressChanged += Model_ProgressChanged;
        }

        void View_Calculate(object sender, CalculateEventArgs e)
        {
            e.Result = Model.Get_mu_k2_Orientiered(e.id, e.cts);
        }
        void View_CalculateTrajectory(object sender, CalculateTrajectoryEventArgs e)
        {
            e.Trajectory = Model.GetTrajectory(e.StagesVariableValues, e.OutputParameters, e.id);
        }
        void View_Open(object sender, OpenEventArgs e)
        {
            Model.Open(e.FileName, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io);
            e.id = id;
            e.Result = io;
        }
        private void View_OpenExample(object sender, OpenExampleEventArgs e)
        {
            var XML = new XmlDocument();
            XML.LoadXml(ExampleResource.Example);
            Model.Open(XML, out CalculationInputData id, out Dictionary<StagesVariableValues, OutputParameters> io);
            e.id = id;
            e.Result = io;
        }
        void View_Save(object sender, SaveEventArgs e)
        {
            Model.Save(e.FileName, e.id, e.Result);
        }
        void View_SaveBallistics(object sender, SaveBallisticsEventArgs e)
        {
            Model.Save(e.FileName, e.Trajectory);
        }
        void Model_ProgressChanged(double Progress)
        {
            View.SetProgress(Progress);
        }
    }
}
