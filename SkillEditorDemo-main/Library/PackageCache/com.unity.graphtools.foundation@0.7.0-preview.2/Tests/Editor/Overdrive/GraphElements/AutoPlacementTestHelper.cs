using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.GraphElements
{
    class AutoPlacementTestHelper : GraphViewTester
    {
        protected IInOutPortsNode FirstNodeModel { get; set; }
        protected IInOutPortsNode SecondNodeModel { get; set; }
        protected IInOutPortsNode ThirdNodeModel { get; set; }
        protected IInOutPortsNode FourthNodeModel { get; set; }
        protected IPlacematModel PlacematModel { get; private set; }
        protected IStickyNoteModel StickyNoteModel { get; private set; }

        protected Node m_FirstNode;
        protected Node m_SecondNode;
        protected Node m_ThirdNode;
        protected Node m_FourthNode;
        protected Placemat m_Placemat;
        protected StickyNote m_StickyNote;

        protected static readonly Vector2 k_SelectionOffset = new Vector2(50, 50);

        protected IEnumerator SetupElements(bool smallerSize, Vector2 firstNodePos, Vector2 secondNodePos, Vector2 placematPos, Vector2 stickyNotePos)
        {
            var actions = CreateElements(firstNodePos, secondNodePos, placematPos, stickyNotePos, smallerSize);
            while (actions.MoveNext())
            {
                yield return null;
            }

            SelectElements();
            yield return null;
        }

        protected IEnumerator CreateConnectedNodes(Vector2 firstNodePos, Vector2 secondNodePos, Vector2 thirdNodePos, Vector2 fourthNodePos, bool isVerticalPort)
        {
            var orientation = isVerticalPort ? Orientation.Vertical : Orientation.Horizontal;

            FirstNodeModel = CreateNode("Node1", firstNodePos, 0, 0, 0, 1, orientation);
            SecondNodeModel = CreateNode("Node2", secondNodePos, 0, 0, 0, 1, orientation);
            ThirdNodeModel = CreateNode("Node3", thirdNodePos, 0, 0, 1, 1, orientation);
            FourthNodeModel = CreateNode("Node4", fourthNodePos, 0, 0, 1, 0, orientation);

            Store.State.RequestUIRebuild();
            yield return null;

            IPortModel outputPortFirstNode = FirstNodeModel.OutputsByDisplayOrder[0];
            IPortModel outputPortSecondNode = SecondNodeModel.OutputsByDisplayOrder[0];
            Assert.IsNotNull(outputPortFirstNode);
            Assert.IsNotNull(outputPortSecondNode);

            IPortModel intputPortThirdNode = ThirdNodeModel.InputsByDisplayOrder[0];
            IPortModel outputPortThirdNode = ThirdNodeModel.OutputsByDisplayOrder[0];
            Assert.IsNotNull(intputPortThirdNode);
            Assert.IsNotNull(outputPortThirdNode);

            IPortModel inputPortFourthNode = FourthNodeModel.InputsByDisplayOrder[0];
            Assert.IsNotNull(inputPortFourthNode);

            // Connect the ports together
            var actions = ConnectPorts(outputPortFirstNode, intputPortThirdNode);
            while (actions.MoveNext())
            {
                yield return null;
            }

            actions = ConnectPorts(outputPortSecondNode, intputPortThirdNode);
            while (actions.MoveNext())
            {
                yield return null;
            }

            actions = ConnectPorts(outputPortThirdNode, inputPortFourthNode);
            while (actions.MoveNext())
            {
                yield return null;
            }

            // Get the UI nodes
            m_FirstNode = FirstNodeModel.GetUI<Node>(graphView);
            m_SecondNode = SecondNodeModel.GetUI<Node>(graphView);
            m_ThirdNode = ThirdNodeModel.GetUI<Node>(graphView);
            m_FourthNode = FourthNodeModel.GetUI<Node>(graphView);
            Assert.IsNotNull(m_FirstNode);
            Assert.IsNotNull(m_SecondNode);
            Assert.IsNotNull(m_ThirdNode);
            Assert.IsNotNull(m_FourthNode);
        }

        IEnumerator CreateElements(Vector2 firstNodePos, Vector2 secondNodePos, Vector2 placematPos, Vector2 stickyNotePos, bool smallerSize)
        {
            FirstNodeModel = CreateNode("Node1", firstNodePos);
            SecondNodeModel = CreateNode("Node2", secondNodePos);
            PlacematModel = CreatePlacemat(new Rect(placematPos, new Vector2(200, smallerSize ? 100 : 200)), "Placemat");
            StickyNoteModel = CreateSticky("Sticky", "", new Rect(stickyNotePos, smallerSize ? new Vector2(100, 100) : new Vector2(200, 200)));

            Store.State.RequestUIRebuild();
            yield return null;

            // Get the UI elements
            m_FirstNode = FirstNodeModel.GetUI<Node>(graphView);
            m_SecondNode = SecondNodeModel.GetUI<Node>(graphView);
            m_Placemat = PlacematModel.GetUI<Placemat>(graphView);
            m_StickyNote = StickyNoteModel.GetUI<StickyNote>(graphView);
            Assert.IsNotNull(m_FirstNode);
            Assert.IsNotNull(m_SecondNode);
            Assert.IsNotNull(m_Placemat);
            Assert.IsNotNull(m_StickyNote);
        }

        protected void SelectConnectedNodes()
        {
            graphView.Selection.Add(m_FirstNode);
            graphView.Selection.Add(m_SecondNode);
            graphView.Selection.Add(m_ThirdNode);
            graphView.Selection.Add(m_FourthNode);
        }

        void SelectElements()
        {
            graphView.Selection.Add(m_FirstNode);
            graphView.Selection.Add(m_SecondNode);
            graphView.Selection.Add(m_Placemat);
            graphView.Selection.Add(m_StickyNote);
        }

        protected IEnumerator SelectElement(Vector2 selectedElementPos)
        {
            helpers.MouseDownEvent(selectedElementPos, MouseButton.LeftMouse, TestEventHelpers.multiSelectModifier);
            yield return null;
            helpers.MouseUpEvent(selectedElementPos, MouseButton.LeftMouse, TestEventHelpers.multiSelectModifier);
            yield return null;
        }
    }
}