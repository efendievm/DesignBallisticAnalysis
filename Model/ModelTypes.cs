using System.Linq;
using TypesLibrary;
using System.Xml;

namespace ModelLibrary
{
    struct Angles
    {
        public double phi_k1;
        public double phi_0;
        public double phi_k2;
    }
    class InputOutputUnion
    {
        public int Index;
        public StagesVariableValues VaribleParameterValues;
        public OutputParameters OutputParameters;
    };
    class AllVariableParameters
    {
        public StagesVariableValues StagesVariableValues;
        public Angles Angles;
    }
    static class XmlNodeExtension
    {
        public static XmlNode FindChildByName(this XmlNode Node, string Name)
        {
            return Node.ChildNodes.Cast<XmlNode>().First(x => x.Name == Name);
        }
    }
}
