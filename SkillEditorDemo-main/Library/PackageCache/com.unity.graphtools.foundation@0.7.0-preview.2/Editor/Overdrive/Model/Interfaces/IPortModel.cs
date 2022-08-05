using System;
using System.Collections.Generic;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public enum PortCapacity
    {
        Single,
        Multi
    }

    [Flags]
    public enum PortModelOptions
    {
        None = 0,
        NoEmbeddedConstant = 1,
        Hidden = 2,
        Default = None,
    }

    // ReSharper disable once InconsistentNaming
    public interface IPortModel : IGraphElementModel
    {
        IPortNode NodeModel { get; set; }
        Direction Direction { get; set; }
        PortType PortType { get; set; }
        Orientation Orientation { get; set; }
        PortCapacity Capacity { get; }
        Type PortDataType { get; }
        PortModelOptions Options { get; set; }

        TypeHandle DataTypeHandle { get; set; }
        string ToolTip { get; set;  }

        bool CreateEmbeddedValueIfNeeded { get; }

        IEnumerable<IPortModel> GetConnectedPorts();
        IEnumerable<IEdgeModel> GetConnectedEdges();
        bool IsConnectedTo(IPortModel toPort);

        PortCapacity GetDefaultCapacity();
        IConstant EmbeddedValue { get; }
        bool DisableEmbeddedValueEditor { get; }

        string UniqueName { get; }
    }
}
