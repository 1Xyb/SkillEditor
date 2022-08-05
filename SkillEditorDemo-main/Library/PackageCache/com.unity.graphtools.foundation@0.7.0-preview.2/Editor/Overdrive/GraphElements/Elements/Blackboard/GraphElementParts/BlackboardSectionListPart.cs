using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class BlackboardSectionListPart : BaseGraphElementPart
    {
        public static readonly string ussClassName = "ge-blackboard-section-list-part";

        public static BlackboardSectionListPart Create(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
        {
            if (model is IBlackboardGraphModel)
            {
                return new BlackboardSectionListPart(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        public Blackboard Blackboard => m_OwnerElement as Blackboard;

        VisualElement m_Root;
        Dictionary<string, BlackboardSection> m_Sections;
        List<GraphElement> m_Rows = new List<GraphElement>();

        protected BlackboardSectionListPart(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        public override VisualElement Root => m_Root;

        protected override void BuildPartUI(VisualElement parent)
        {
            m_Root = new VisualElement { name = PartName };
            m_Root.AddToClassList(ussClassName);
            m_Root.AddToClassList(m_ParentClassName.WithUssElement(PartName));

            if (m_Model is IBlackboardGraphModel graphProxyElement)
            {
                m_Sections = new Dictionary<string, BlackboardSection>();
                foreach (var sectionName in graphProxyElement.SectionNames)
                {
                    var section = new BlackboardSection(Blackboard, sectionName);
                    m_Root.Add(section);
                    m_Sections.Add(sectionName, section);
                }
            }

            parent.Add(m_Root);
        }

        protected override void UpdatePartFromModel()
        {
            if (m_Model is IBlackboardGraphModel graphProxyElement)
            {
                foreach (var row in m_Rows)
                {
                    row.RemoveFromGraphView();
                }
                m_Rows.Clear();

                Blackboard.Highlightables.Clear();

                foreach (var sectionName in graphProxyElement.SectionNames)
                {
                    // PF FIXME: implement partial rebuild, like for PortContainer
                    m_Sections[sectionName].Clear();

                    var variableDeclarationModels = graphProxyElement.GetSectionRows(sectionName);
                    foreach (var vdm in variableDeclarationModels)
                    {
                        var ui = GraphElementFactory.CreateUI<GraphElement>(m_OwnerElement.GraphView, m_OwnerElement.Store, vdm);

                        if (ui == null)
                            continue;

                        m_Sections[sectionName].Add(ui);
                        ui.AddToGraphView(m_OwnerElement.GraphView);
                        m_Rows.Add(ui);

                        Blackboard.Highlightables.AddRange(vdm.GetAllUIs(m_OwnerElement.GraphView).OfType<IHighlightable>());
                    }
                }
            }
        }

        protected override void PartOwnerAddedToGraphView()
        {
            foreach (var row in m_Rows)
            {
                row.AddToGraphView(m_OwnerElement.GraphView);
            }

            base.PartOwnerAddedToGraphView();
        }

        protected override void PartOwnerRemovedFromGraphView()
        {
            foreach (var row in m_Rows)
            {
                row.RemoveFromGraphView();
            }

            base.PartOwnerRemovedFromGraphView();
        }
    }
}
