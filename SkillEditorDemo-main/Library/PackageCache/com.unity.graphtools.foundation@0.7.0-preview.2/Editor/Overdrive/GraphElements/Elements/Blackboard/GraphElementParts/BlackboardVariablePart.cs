using System;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class BlackboardVariablePart : BaseGraphElementPart
    {
        public static readonly string ussClassName = "ge-blackboard-variable-part";

        public static readonly string blackboardVariablePartCreationContext = nameof(BlackboardVariablePart);

        public static BlackboardVariablePart Create(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
        {
            if (model is IVariableDeclarationModel)
            {
                return new BlackboardVariablePart(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        GraphElement m_Field;

        public override VisualElement Root => m_Field;

        protected BlackboardVariablePart(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        protected override void BuildPartUI(VisualElement parent)
        {
            if (m_Model is IVariableDeclarationModel variableDeclarationModel)
            {
                m_Field = GraphElementFactory.CreateUI<GraphElement>(m_OwnerElement.GraphView, m_OwnerElement.Store,
                    variableDeclarationModel, blackboardVariablePartCreationContext);

                if (m_Field == null)
                    return;

                m_Field.AddToClassList(ussClassName);
                m_Field.AddToClassList(m_ParentClassName.WithUssElement(PartName));
                m_Field.viewDataKey = m_Model.Guid + "__" + Blackboard.persistenceKey;

                m_Field.AddToGraphView(m_OwnerElement.GraphView);

                if (m_Field is BlackboardField blackboardField)
                {
                    blackboardField.NameLabel.RegisterCallback<ChangeEvent<string>>(OnFieldRenamed);
                }

                if (parent is BlackboardRow row)
                    row.FieldSlot.Add(m_Field);
                else
                    parent.Add(m_Field);
            }
        }

        protected override void UpdatePartFromModel() {}

        void OnFieldRenamed(ChangeEvent<string> e)
        {
            m_OwnerElement.Store.Dispatch(new RenameElementAction(m_Model as IRenamable, e.newValue));
        }

        protected override void PartOwnerAddedToGraphView()
        {
            m_Field.AddToGraphView(m_OwnerElement.GraphView);
            base.PartOwnerAddedToGraphView();
        }

        protected override void PartOwnerRemovedFromGraphView()
        {
            m_Field.RemoveFromGraphView();
            base.PartOwnerRemovedFromGraphView();
        }
    }
}
