using System;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class BlackboardVariablePropertiesPart : BaseGraphElementPart
    {
        public static readonly string ussClassName = "ge-blackboard-variable-properties-part";

        public static readonly string blackboardVariablePropertiesPartCreationContext = nameof(BlackboardVariablePropertiesPart);

        public static BlackboardVariablePropertiesPart Create(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
        {
            if (model is IVariableDeclarationModel)
            {
                return new BlackboardVariablePropertiesPart(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        GraphElement m_VariablePropertiesView;

        public override VisualElement Root => m_VariablePropertiesView;

        protected BlackboardVariablePropertiesPart(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        protected override void BuildPartUI(VisualElement parent)
        {
            m_VariablePropertiesView = GraphElementFactory.CreateUI<GraphElement>(m_OwnerElement.GraphView,
                m_OwnerElement.Store, m_Model, blackboardVariablePropertiesPartCreationContext);
            m_VariablePropertiesView.AddToClassList(ussClassName);
            m_VariablePropertiesView.AddToClassList(m_ParentClassName.WithUssElement(PartName));

            m_VariablePropertiesView.AddToGraphView(m_OwnerElement.GraphView);

            if (parent is BlackboardRow row)
                row.PropertiesSlot.Add(m_VariablePropertiesView);
            else
                parent.Add(m_VariablePropertiesView);
        }

        protected override void UpdatePartFromModel()
        {
            m_VariablePropertiesView.UpdateFromModel();
        }

        protected override void PartOwnerAddedToGraphView()
        {
            m_VariablePropertiesView.AddToGraphView(m_OwnerElement.GraphView);
            base.PartOwnerAddedToGraphView();
        }

        protected override void PartOwnerRemovedFromGraphView()
        {
            m_VariablePropertiesView.RemoveFromGraphView();
            base.PartOwnerRemovedFromGraphView();
        }
    }
}
