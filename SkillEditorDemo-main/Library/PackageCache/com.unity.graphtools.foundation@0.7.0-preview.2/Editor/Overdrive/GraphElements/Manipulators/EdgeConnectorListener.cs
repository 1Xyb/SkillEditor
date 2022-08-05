using System;
using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive.InternalModels;
using UnityEngine;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class EdgeConnectorListener
    {
        Action<Store, Edge, Vector2> m_OnDropOutsideDelegate;
        Action<Store, Edge> m_OnDropDelegate;

        public void SetDropOutsideDelegate(Action<Store, Edge, Vector2> action)
        {
            m_OnDropOutsideDelegate = action;
        }

        public void SetDropDelegate(Action<Store, Edge> action)
        {
            m_OnDropDelegate = action;
        }

        public static List<IEdgeModel> GetDropEdgeModelsToDelete(IEdgeModel edge)
        {
            List<IEdgeModel> edgeModelsToDelete = new List<IEdgeModel>();

            if (edge.ToPort != null && edge.ToPort.Capacity == PortCapacity.Single)
            {
                foreach (var edgeToDelete in edge.ToPort.GetConnectedEdges())
                {
                    if (!ReferenceEquals(edgeToDelete, edge) && !(edgeToDelete is GhostEdgeModel))
                        edgeModelsToDelete.Add(edgeToDelete);
                }
            }

            if (edge.FromPort != null && edge.FromPort.Capacity == PortCapacity.Single)
            {
                foreach (var edgeToDelete in edge.FromPort.GetConnectedEdges())
                {
                    if (!ReferenceEquals(edgeToDelete, edge) && !(edgeToDelete is GhostEdgeModel))
                        edgeModelsToDelete.Add(edgeToDelete);
                }
            }

            return edgeModelsToDelete;
        }

        public void OnDropOutsidePort(Store store, Edge edge, Vector2 position, Edge originalEdge)
        {
            if (m_OnDropOutsideDelegate != null)
            {
                m_OnDropOutsideDelegate(store, edge, position);
            }
            else
            {
                List<IEdgeModel> edgesToDelete = GetDropEdgeModelsToDelete(edge.EdgeModel);

                // when grabbing an existing edge's end, the edgeModel should be deleted
                if (!(edge.EdgeModel is GhostEdgeModel))
                    edgesToDelete.Add(edge.EdgeModel);

                if (originalEdge != null)
                    edgesToDelete.Add(originalEdge.EdgeModel);

                store.Dispatch(new DeleteEdgeAction(edgesToDelete));
            }
        }

        public void OnDrop(Store store, Edge edge, Edge originalEdge)
        {
            if (m_OnDropDelegate != null)
            {
                m_OnDropDelegate(store, edge);
            }
            else
            {
                List<IEdgeModel> edgeModelsToDelete = GetDropEdgeModelsToDelete(edge.EdgeModel);

                if (edge.EdgeModel.ToPort.IsConnectedTo(edge.EdgeModel.FromPort))
                    return;

                // when grabbing an existing edge's end, the edgeModel should be deleted
                if (!(edge.EdgeModel is GhostEdgeModel))
                    edgeModelsToDelete.Add(edge.EdgeModel);

                if (originalEdge != null)
                    edgeModelsToDelete.Add(originalEdge.EdgeModel);

                store.Dispatch(new CreateEdgeAction(
                    edge.EdgeModel.ToPort,
                    edge.EdgeModel.FromPort,
                    edgeModelsToDelete
                ));
            }
        }
    }
}