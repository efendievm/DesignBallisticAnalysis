namespace TypesLibrary
{
    public class VariableParameter
    {
        public double Start { get; set; }
        public double End { get; set; }
        public VariableParameter(double Start, double End)
        {
            this.Start = Start;
            this.End = End;
        }
    }

    public class StageVariableParameters
    {
        public VariableParameter n_0 { get; set; }
        public VariableParameter a_to { get; set; }
        public VariableParameter mu_su_pr { get; set; }
        public VariableParameter gamma_du { get; set; }
    }

    public class FirstStageVariableParameters : StageVariableParameters
    {
        public VariableParameter mu_k { get; set; }
    }

    public class SecondStageVariableParameters : StageVariableParameters { }

    public class StageVariableValues
    {
        public double n_0 { get; set; }
        public double a_to { get; set; }
        public double mu_su_pr { get; set; }
        public double gamma_du { get; set; }
    }

    public class FirstStageVariableValues : StageVariableValues
    {
        public double mu_k { get; set; }
    }

    public class SecondStageVariableValues : StageVariableValues { }

    public class StageConstantParameters
    {
        public double I_p { get; set; }
        public double P_m { get; set; }
        public double l { get; set; }
        public double rg { get; set; }
        public double ro { get; set; }
        public double kd { get; set; }
    }
    
    public class CalculationInputData
    {
        public StageConstantParameters FirstStageConstantParameters { get; set; }
        public StageConstantParameters SecondStageConstantParameters { get; set; }
        public FirstStageVariableParameters FirstStageVariableParameters { get; set; }
        public SecondStageVariableParameters SecondStageVariableParameters { get; set; }
        public VariableParameter phi_k1 { get; set; }
        public VariableParameter phi_k2 { get; set; }
        public VariableParameter phi_0 { get; set; }
        public double M_pg { get; set; }
        public double V  { get; set; }
        public double H  { get; set; }
        public double nu { get; set; }
    }
    
    public class StagesVariableValues
    {
        public FirstStageVariableValues firstStageVariableValues { get; set; }
        public SecondStageVariableValues secondStageVariableValues { get; set; }
    }
    
    public class StageOutputData
    {
        public double P_0 { get; set; }
        public double m_0 { get; set; }
        public double m_k { get; set; }
        public double m_t { get; set; }
        public double m_o { get; set; }
        public double m_g { get; set; }
        public double m_to { get; set; }
        public double m_su_pr { get; set; }
        public double m_du { get; set; }
        public double m_sux { get; set; }
        public double mu_sux { get; set; }
    }
    
    public class FirstStageOutputData  : StageOutputData {}
    
    public class SecondStageOutputData : StageOutputData
    {
        public double mu_k { get; set; }
    }
    
    public class TrajectoryOutputParameters
    {
        public double phi_k1 { get; set; }
        public double phi_k2 { get; set; }
        public double phi_0 { get; set; }
        public double V { get; set; }
        public double H { get; set; }
        public double nu { get; set; }
        public double t_1 { get; set; }
        public double t_2 { get; set; }
    }
    
    public class OutputParameters
    {
        public double mu_pg { get; set; }
        public FirstStageOutputData  FirstStageOutputData { get; set; }
        public SecondStageOutputData SecondStageOutputData { get; set; }
        public TrajectoryOutputParameters TrajectoryOutputParameters { get; set; }
    }

    public class TrajectoryParameters
    {
        public double Time { get; set; }
        public double v_char { get; set; }
        public double v_grav { get; set; }
        public double v_aer { get; set; }
        public double v_press { get; set; }
        public double m { get; set; }
        public double phi { get; set; }
        public double nu { get; set; }
        public double xi { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double h { get; set; }
    }
}