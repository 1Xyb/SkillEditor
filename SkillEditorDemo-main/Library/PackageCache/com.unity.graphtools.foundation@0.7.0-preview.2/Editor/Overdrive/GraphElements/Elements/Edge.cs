using System.Linq;
using UnityEditor.GraphToolsFoundation.Overdrive.InternalModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class Edge : GraphElement
    {
        public new static readonly string ussClassName = "ge-edge";
        public static readonly string editModeModifierUssClassName = ussClassName.WithUssModifier("edit-mode");
        public static readonly string ghostModifierUssClassName = ussClassName.WithUssModifier("ghost");

        public static readonly string edgeControlPartName = "edge-control";
        public static readonly string edgeBubblePartName = "edge-bubble";

        EdgeManipulator m_EdgeManipulator;

        EdgeControl m_EdgeControl;

        protected EdgeManipulator EdgeManipulator
        {
            get => m_EdgeManipulator;
            set => this.ReplaceManipulator(ref m_EdgeManipulator, value);
        }

        public IEdgeModel EdgeModel => Model as IEdgeModel;

        public bool IsGhostEdge => EdgeModel is IGhostEdge;

        public Vector2 From
        {
            get
            {
                var p = Vector2.zero;

                var port = EdgeModel.FromPort;
                if (port == null)
                {
                    if (EdgeModel is IGhostEdge ghostEdgeModel)
                    {
                        p = ghostEdgeModel.EndPoint;
                    }
                }
                else
                {
                    var ui = port.GetUI<Port>(GraphView);
                    if (ui == null)
                        return Vector2.zero;

                    p = ui.GetGlobalCenter();
                }

                return this.WorldToLocal(p);
            }
        }

        public Vector2 To
        {
            get
            {
                var p = Vector2.zero;

                var port = EdgeModel.ToPort;
                if (port == null)
                {
                    if (EdgeModel is GhostEdgeModel ghostEdgeModel)
                    {
                        p = ghostEdgeModel.EndPoint;
                    }
                }
                else
                {
                    var ui = port.GetUI<Port>(GraphView);
                    if (ui == null)
                        return Vector2.zero;

                    p = ui.GetGlobalCenter();
                }

                return this.WorldToLocal(p);
            }
        }

        public EdgeControl EdgeControl
        {
            get
            {
                if (m_EdgeControl == null)
                {
                    var edgeControlPart = PartList.GetPart(edgeControlPartName);
                    m_EdgeControl = edgeControlPart?.Root as EdgeControl;
                }

                return m_EdgeControl;
            }
        }

        public IPortModel Output => EdgeModel.FromPort;

        public IPortModel Input => EdgeModel.ToPort;

        public override bool ShowInMiniMap => false;

        public Edge()
        {
            Layer = -1;

            EdgeManipulator = new EdgeManipulator();
        }

        protected override void BuildPartList()
        {
            PartList.AppendPart(EdgeControlPart.Create(edgeControlPartName, Model, this, ussClassName));
            PartList.AppendPart(EdgeBubblePart.Create(edgeBubblePartName, Model, this, ussClassName));
        }

        protected override void PostBuildUI()
        {
            base.PostBuildUI();

            AddToClassList(ussClassName);
            EnableInClassList(ghostModifierUssClassName, IsGhostEdge);
            this.AddStylesheet("Edge.uss");
        }

        protected override void UpdateElementFromModel()
        {
            base.UpdateElementFromModel();

            if (EdgeModel is IEditableEdge editableEdge)
                EnableInClassList(editModeModifierUssClassName, editableEdge.EditMode);
        }

        public override void AddBackwardDependencies()
        {
            base.AddBackwardDependencies();

            AddBackwardDependencies(EdgeModel.FromPort);
            AddBackwardDependencies(EdgeModel.ToPort);

            void AddBackwardDependencies(IPortModel portModel)
            {
                if (portModel == null)
                    return;

                var ui = portModel.GetUI(GraphView);
                if (ui != null)
                {
                    // Edge color changes with port color.
                    Dependencies.AddBackwardDependency(ui, DependencyType.Style);
                }

                ui = portModel.NodeModel.GetUI(GraphView);
                if (ui != null)
                {
                    // Edge position changes with node position.
                    Dependencies.AddBackwardDependency(ui, DependencyType.Geometry);
                }
            }
        }

        public override bool Overlaps(Rect rectangle)
        {
            return EdgeControl.Overlaps(this.ChangeCoordinatesTo(EdgeControl, rectangle));
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            return EdgeControl.ContainsPoint(this.ChangeCoordinatesTo(EdgeControl, localPoint));
        }

        public override void OnSelected()
        {
            // PF FIXME: model modification should occur as part of a SelectElementAction.
            // Then the UpdateFromModel at the end would not be necessary.

            base.OnSelected();

            var edgeControlPart = PartList.GetPart(edgeControlPartName);
            edgeControlPart?.UpdateFromModel();

            if (EdgeModel.FromPort == null)
                return;

            EdgeModel.FromPort.NodeModel.RevealReorderableEdgesOrder(true, EdgeModel);
            var nodeModel = EdgeModel.FromPort.NodeModel;
            foreach (var edge in nodeModel.ConnectedPortsWithReorderableEdges().SelectMany(p => p.GetConnectedEdges()))
                edge.GetUI<Edge>(GraphView)?.UpdateFromModel();
        }

        public override void OnUnselected()
        {
            // PF FIXME: model modification should occur as part of a SelectElementAction
            // Then the UpdateFromModel at the end would not be necessary.

            base.OnUnselected();

            var edgeControlPart = PartList.GetPart(edgeControlPartName);
            edgeControlPart?.UpdateFromModel();

            if (EdgeModel.FromPort == null)
                return;

            EdgeModel.FromPort.NodeModel.RevealReorderableEdgesOrder(false);
            var nodeModel = EdgeModel.FromPort.NodeModel;
            foreach (var edge in nodeModel.ConnectedPortsWithReorderableEdges().SelectMany(p => p.GetConnectedEdges()))
                edge.GetUI<Edge>(GraphView)?.UpdateFromModel();
        }

        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (!(evt.currentTarget is Edge edge))
                return;

            var editableEdge = edge.EdgeModel as IEditableEdge;

            if (editableEdge?.EditMode ?? false)
            {
                if (evt.menu.MenuItems().Count > 0)
                    evt.menu.AppendSeparator();

                var p = edge.EdgeControl.WorldToLocal(evt.triggerEvent.originalMousePosition);
                edge.EdgeControl.FindNearestCurveSegment(p, out _, out var controlPointIndex, out _);
                p = edge.WorldToLocal(evt.triggerEvent.originalMousePosition);

                if (!(evt.target is EdgeControlPoint))
                {
                    evt.menu.AppendAction("Add Control Point", menuAction =>
                    {
                        Store.Dispatch(new AddControlPointOnEdgeAction(editableEdge, controlPointIndex, p));
                    });
                }

                evt.menu.AppendAction("Stop Editing Edge", menuAction =>
                {
                    Store.Dispatch(new SetEdgeEditModeAction(editableEdge, false));
                });
            }
            else
            {
                bool initialSeparatorAdded = false;
                int initialMenuItemCount = evt.menu.MenuItems().Count;

                if (editableEdge != null)
                {
                    if (initialMenuItemCount > 0)
                    {
                        initialSeparatorAdded = true;
                        evt.menu.AppendSeparator();
                    }

                    evt.menu.AppendAction("Edit Edge", menuAction =>
                    {
                        Store.Dispatch(new SetEdgeEditModeAction(editableEdge, true));
                    });
                }

                if ((edge.EdgeModel.FromPort as IReorderableEdgesPort)?.HasReorderableEdges ?? false)
                {
                    if (!initialSeparatorAdded && initialMenuItemCount > 0)
                        evt.menu.AppendSeparator();

                    var siblingEdges = edge.EdgeModel.FromPort.GetConnectedEdges().ToList();
                    var siblingEdgesCount = siblingEdges.Count;

                    var index = siblingEdges.IndexOf(edge.EdgeModel);
                    evt.menu.AppendAction("Reorder Edge/Move First",
                        a => ReorderEdges(ReorderEdgeAction.ReorderType.MoveFirst),
                        siblingEdgesCount > 1 && index > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendAction("Reorder Edge/Move Up",
                        a => ReorderEdges(ReorderEdgeAction.ReorderType.MoveUp),
                        siblingEdgesCount > 1 && index > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendAction("Reorder Edge/Move Down",
                        a => ReorderEdges(ReorderEdgeAction.ReorderType.MoveDown),
                        siblingEdgesCount > 1 && index < siblingEdgesCount - 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                    evt.menu.AppendAction("Reorder Edge/Move Last",
                        a => ReorderEdges(ReorderEdgeAction.ReorderType.MoveLast),
                        siblingEdgesCount > 1 && index < siblingEdgesCount - 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                    void ReorderEdges(ReorderEdgeAction.ReorderType reorderType)
                    {
                        Store.Dispatch(new ReorderEdgeAction(edge.EdgeModel, reorderType));
                    }
                }
            }
        }
    }
}