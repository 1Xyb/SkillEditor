namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.TestModels
{
    public class PlacematModel : BasicModel.PlacematModel
    {
        IGraphModel m_GraphModel;
        public override IGraphModel GraphModel => m_GraphModel;

        public PlacematModel(IGraphModel graphModel)
        {
            m_GraphModel = graphModel;
        }
    }
}
