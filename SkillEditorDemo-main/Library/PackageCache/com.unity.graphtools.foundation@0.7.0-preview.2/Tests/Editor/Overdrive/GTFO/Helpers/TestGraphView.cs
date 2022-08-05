namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.GTFO
{
    public class TestGraphView : GraphView
    {
        public TestGraphView(GraphViewEditorWindow window, Store store) : base(window, store)
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale, 1.0f);
            focusable = true;
        }
    }
}