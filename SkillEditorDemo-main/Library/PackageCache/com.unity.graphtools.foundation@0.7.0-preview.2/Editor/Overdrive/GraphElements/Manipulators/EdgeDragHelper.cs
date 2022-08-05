using System;
using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive.InternalModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class EdgeDragHelper
    {
        internal const int panAreaWidth = 100;
        internal const int panSpeed = 4;
        internal const int panInterval = 10;
        internal const float maxSpeedFactor = 2.5f;
        internal const float maxPanSpeed = maxSpeedFactor * panSpeed;

        List<IPortModel> m_CompatiblePorts;
        GhostEdgeModel m_GhostEdgeModel;
        Edge m_GhostEdge;
        public GraphView GraphView { get; }
        readonly Store m_Store;
        readonly EdgeConnectorListener m_Listener;
        readonly Func<IGraphModel, GhostEdgeModel> m_GhostEdgeViewModelCreator;

        IVisualElementScheduledItem m_PanSchedule;
        Vector3 m_PanDiff = Vector3.zero;
        bool m_WasPanned;

        bool resetPositionOnPan { get; set; }

        public EdgeDragHelper(Store store, GraphView graphView, EdgeConnectorListener listener, Func<IGraphModel, GhostEdgeModel> ghostEdgeViewModelCreator)
        {
            m_Store = store;
            GraphView = graphView;
            m_Listener = listener;
            m_GhostEdgeViewModelCreator = ghostEdgeViewModelCreator;
            resetPositionOnPan = true;
            Reset();
        }

        public Edge CreateGhostEdge(IGraphModel graphModel)
        {
            GhostEdgeModel ghostEdge;

            if (m_GhostEdgeViewModelCreator != null)
            {
                ghostEdge = m_GhostEdgeViewModelCreator.Invoke(graphModel);
            }
            else
            {
                ghostEdge = new GhostEdgeModel(graphModel);
            }
            var ui = GraphElementFactory.CreateUI<Edge>(GraphView, m_Store, ghostEdge);
            return ui;
        }

        GhostEdgeModel m_EdgeCandidateModel;
        Edge m_EdgeCandidate;
        public GhostEdgeModel edgeCandidateModel => m_EdgeCandidateModel;

        public void CreateEdgeCandidate(IGraphModel graphModel)
        {
            m_EdgeCandidate = CreateGhostEdge(graphModel);
            m_EdgeCandidateModel = m_EdgeCandidate.EdgeModel as GhostEdgeModel;
        }

        void ClearEdgeCandidate()
        {
            m_EdgeCandidateModel = null;
            m_EdgeCandidate = null;
        }

        public IPortModel draggedPort { get; set; }
        public Edge originalEdge { get; set; }

        public void Reset(bool didConnect = false)
        {
            if (m_CompatiblePorts != null)
            {
                // Reset the highlights.
                GraphView.Ports.ForEach((p) =>
                {
                    p.SetEnabled(true);
                    p.Highlighted = false;
                });
                m_CompatiblePorts = null;
            }

            if (m_GhostEdge != null)
            {
                GraphView.RemoveElement(m_GhostEdge);
            }

            if (m_EdgeCandidate != null)
            {
                GraphView.RemoveElement(m_EdgeCandidate);
            }

            if (m_WasPanned)
            {
                if (!resetPositionOnPan || didConnect)
                {
                    Vector3 p = GraphView.contentViewContainer.transform.position;
                    Vector3 s = GraphView.contentViewContainer.transform.scale;
                    GraphView.UpdateViewTransform(p, s);
                }
            }

            m_PanSchedule?.Pause();

            if (draggedPort != null && !didConnect)
            {
                var portUI = draggedPort.GetUI<Port>(GraphView);
                if (portUI != null)
                    portUI.WillConnect = false;

                draggedPort = null;
            }

            m_GhostEdge = null;
            ClearEdgeCandidate();
        }

        public bool HandleMouseDown(MouseDownEvent evt)
        {
            Vector2 mousePosition = evt.mousePosition;

            if (draggedPort == null || edgeCandidateModel == null)
            {
                return false;
            }

            if (m_EdgeCandidate == null)
                return false;

            if (m_EdgeCandidate.parent == null)
            {
                GraphView.AddElement(m_EdgeCandidate);
            }

            bool startFromOutput = draggedPort.Direction == Direction.Output;

            edgeCandidateModel.EndPoint = mousePosition;
            m_EdgeCandidate.SetEnabled(false);

            if (startFromOutput)
            {
                edgeCandidateModel.FromPort = draggedPort;
                edgeCandidateModel.ToPort = null;
            }
            else
            {
                edgeCandidateModel.FromPort = null;
                edgeCandidateModel.ToPort = draggedPort;
            }

            var portUI = draggedPort.GetUI<Port>(GraphView);
            if (portUI != null)
                portUI.WillConnect = true;

            m_CompatiblePorts = m_Store.State.GraphModel.GetCompatiblePorts(draggedPort);

            // Only light compatible anchors when dragging an edge.
            GraphView.Ports.ForEach((p) =>
            {
                p.SetEnabled(false);
                p.Highlighted = false;
            });

            foreach (var compatiblePort in m_CompatiblePorts)
            {
                var compatiblePortUI = compatiblePort.GetUI<Port>(GraphView);
                if (compatiblePortUI != null)
                {
                    compatiblePortUI.SetEnabled(true);
                    compatiblePortUI.Highlighted = true;
                }
            }

            m_EdgeCandidate.UpdateFromModel();

            if (m_PanSchedule == null)
            {
                m_PanSchedule = GraphView.schedule.Execute(Pan).Every(panInterval).StartingIn(panInterval);
                m_PanSchedule.Pause();
            }

            m_WasPanned = false;

            m_EdgeCandidate.Layer = Int32.MaxValue;

            return true;
        }

        Vector2 GetEffectivePanSpeed(Vector2 mousePos)
        {
            Vector2 effectiveSpeed = Vector2.zero;

            if (mousePos.x <= panAreaWidth)
                effectiveSpeed.x = -(((panAreaWidth - mousePos.x) / panAreaWidth) + 0.5f) * panSpeed;
            else if (mousePos.x >= GraphView.contentContainer.layout.width - panAreaWidth)
                effectiveSpeed.x = (((mousePos.x - (GraphView.contentContainer.layout.width - panAreaWidth)) / panAreaWidth) + 0.5f) * panSpeed;

            if (mousePos.y <= panAreaWidth)
                effectiveSpeed.y = -(((panAreaWidth - mousePos.y) / panAreaWidth) + 0.5f) * panSpeed;
            else if (mousePos.y >= GraphView.contentContainer.layout.height - panAreaWidth)
                effectiveSpeed.y = (((mousePos.y - (GraphView.contentContainer.layout.height - panAreaWidth)) / panAreaWidth) + 0.5f) * panSpeed;

            effectiveSpeed = Vector2.ClampMagnitude(effectiveSpeed, maxPanSpeed);

            return effectiveSpeed;
        }

        public void HandleMouseMove(MouseMoveEvent evt)
        {
            var ve = (VisualElement)evt.target;
            Vector2 gvMousePos = ve.ChangeCoordinatesTo(GraphView.contentContainer, evt.localMousePosition);
            m_PanDiff = GetEffectivePanSpeed(gvMousePos);

            if (m_PanDiff != Vector3.zero)
            {
                m_PanSchedule.Resume();
            }
            else
            {
                m_PanSchedule.Pause();
            }

            Vector2 mousePosition = evt.mousePosition;

            edgeCandidateModel.EndPoint = mousePosition;
            m_EdgeCandidate.UpdateFromModel();

            // Draw ghost edge if possible port exists.
            Port endPort = GetEndPort(mousePosition);

            if (endPort != null)
            {
                if (m_GhostEdge == null)
                {
                    m_GhostEdge = CreateGhostEdge(endPort.PortModel.GraphModel);
                    m_GhostEdgeModel = m_GhostEdge.EdgeModel as GhostEdgeModel;

                    m_GhostEdge.pickingMode = PickingMode.Ignore;
                    GraphView.AddElement(m_GhostEdge);
                }

                Debug.Assert(m_GhostEdgeModel != null);

                if (edgeCandidateModel.FromPort == null)
                {
                    m_GhostEdgeModel.ToPort = edgeCandidateModel.ToPort;
                    var portUI = m_GhostEdgeModel?.FromPort?.GetUI<Port>(GraphView);
                    if (portUI != null)
                        portUI.WillConnect = false;
                    m_GhostEdgeModel.FromPort = endPort.PortModel;
                    endPort.WillConnect = true;
                }
                else
                {
                    var portUI = m_GhostEdgeModel?.ToPort?.GetUI<Port>(GraphView);
                    if (portUI != null)
                        portUI.WillConnect = false;
                    m_GhostEdgeModel.ToPort = endPort.PortModel;
                    endPort.WillConnect = true;
                    m_GhostEdgeModel.FromPort = edgeCandidateModel.FromPort;
                }

                m_GhostEdge.UpdateFromModel();
            }
            else if (m_GhostEdge != null && m_GhostEdgeModel != null)
            {
                if (edgeCandidateModel.ToPort == null)
                {
                    var portUI = m_GhostEdgeModel?.ToPort?.GetUI<Port>(GraphView);
                    if (portUI != null)
                        portUI.WillConnect = false;
                }
                else
                {
                    var portUI = m_GhostEdgeModel?.FromPort?.GetUI<Port>(GraphView);
                    if (portUI != null)
                        portUI.WillConnect = false;
                }
                GraphView.RemoveElement(m_GhostEdge);
                m_GhostEdgeModel.ToPort = null;
                m_GhostEdgeModel.FromPort = null;
                m_GhostEdgeModel = null;
                m_GhostEdge = null;
            }
        }

        void Pan(TimerState ts)
        {
            GraphView.viewTransform.position -= m_PanDiff;
            edgeCandidateModel.GetUI<Edge>(GraphView)?.UpdateFromModel();
            m_WasPanned = true;
        }

        public void HandleMouseUp(MouseUpEvent evt)
        {
            bool didConnect = false;

            Vector2 mousePosition = evt.mousePosition;

            // Reset the highlights.
            GraphView.Ports.ForEach((p) =>
            {
                p.SetEnabled(true);
                p.Highlighted = false;
            });

            Port portUI;
            // Clean up ghost edges.
            if (m_GhostEdgeModel != null)
            {
                portUI = m_GhostEdgeModel.ToPort?.GetUI<Port>(GraphView);
                if (portUI != null)
                    portUI.WillConnect = false;

                portUI = m_GhostEdgeModel.FromPort?.GetUI<Port>(GraphView);
                if (portUI != null)
                    portUI.WillConnect = false;

                GraphView.RemoveElement(m_GhostEdge);
                m_GhostEdgeModel.ToPort = null;
                m_GhostEdgeModel.FromPort = null;
                m_GhostEdgeModel = null;
                m_GhostEdge = null;
            }

            Port endPort = GetEndPort(mousePosition);

            if (endPort == null && m_Listener != null)
            {
                m_Listener.OnDropOutsidePort(m_Store, m_EdgeCandidate, mousePosition, originalEdge);
            }

            m_EdgeCandidate.SetEnabled(true);

            portUI = edgeCandidateModel?.ToPort?.GetUI<Port>(GraphView);
            if (portUI != null)
                portUI.WillConnect = false;

            portUI = edgeCandidateModel?.FromPort?.GetUI<Port>(GraphView);
            if (portUI != null)
                portUI.WillConnect = false;

            // If it is an existing valid edge then delete and notify the model (using DeleteElements()).
            if (edgeCandidateModel?.ToPort != null && edgeCandidateModel?.FromPort != null)
            {
                // Save the current input and output before deleting the edge as they will be reset
                var oldInput = edgeCandidateModel.ToPort;
                var oldOutput = edgeCandidateModel.FromPort;

                GraphView.RemoveElement(m_EdgeCandidate);

                // Restore the previous input and output
                edgeCandidateModel.ToPort = oldInput;
                edgeCandidateModel.FromPort = oldOutput;
            }
            // otherwise, if it is an temporary edge then just remove it as it is not already known my the model
            else
            {
                GraphView.RemoveElement(m_EdgeCandidate);
            }

            if (endPort != null)
            {
                if (edgeCandidateModel != null)
                {
                    if (endPort.PortModel.Direction == Direction.Output)
                        edgeCandidateModel.FromPort = endPort.PortModel;
                    else
                        edgeCandidateModel.ToPort = endPort.PortModel;
                }

                m_Listener.OnDrop(m_Store, m_EdgeCandidate, originalEdge);
                didConnect = true;
            }
            else if (edgeCandidateModel != null)
            {
                edgeCandidateModel.FromPort = null;
                edgeCandidateModel.ToPort = null;
            }

            m_EdgeCandidate?.ResetLayer();

            ClearEdgeCandidate();
            m_CompatiblePorts = null;
            Reset(didConnect);

            originalEdge = null;
        }

        Port GetEndPort(Vector2 mousePosition)
        {
            Port endPort = null;

            foreach (var compatiblePort in m_CompatiblePorts)
            {
                var compatiblePortUI = compatiblePort.GetUI<Port>(GraphView);
                if (compatiblePortUI == null || compatiblePortUI.resolvedStyle.visibility != Visibility.Visible)
                    continue;

                Rect bounds = compatiblePortUI.worldBound;
                float hitboxExtraPadding = bounds.height;

                if (compatiblePort.Orientation == Orientation.Horizontal)
                {
                    // Add extra padding for mouse check to the left of input port or right of output port.
                    if (compatiblePort.Direction == Direction.Input)
                    {
                        // Move bounds to the left by hitboxExtraPadding and increase width
                        // by hitboxExtraPadding.
                        bounds.x -= hitboxExtraPadding;
                        bounds.width += hitboxExtraPadding;
                    }
                    else if (compatiblePort.Direction == Direction.Output)
                    {
                        // Just add hitboxExtraPadding to the width.
                        bounds.width += hitboxExtraPadding;
                    }
                }

                // Check if mouse is over port.
                if (bounds.Contains(mousePosition))
                {
                    endPort = compatiblePortUI;
                    break;
                }
            }

            return endPort;
        }
    }
}