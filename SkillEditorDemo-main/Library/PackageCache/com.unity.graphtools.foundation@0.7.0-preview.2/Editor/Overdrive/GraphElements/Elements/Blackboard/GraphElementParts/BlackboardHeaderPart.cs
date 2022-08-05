using System;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class BlackboardHeaderPart : BaseGraphElementPart
    {
        public static readonly string ussClassName = "ge-blackboard-header-part";
        public static readonly string titleUssClassName = ussClassName.WithUssElement("title");
        public static readonly string subTitleUssClassName = ussClassName.WithUssElement("subtitle");

        protected static readonly string defaultTitle = "Blackboard";
        protected static readonly string defaultSubTitle = "";

        public static BlackboardHeaderPart Create(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
        {
            if (model is IBlackboardGraphModel)
            {
                return new BlackboardHeaderPart(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        VisualElement m_Root;

        VisualElement m_MainContainer;
        Label m_TitleLabel;
        Label m_SubTitleLabel;
        ScrollView m_ScrollView;
        VisualElement m_ContentContainer;

        protected BlackboardHeaderPart(string name, IGraphElementModel model, IGraphElement ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {}

        public override VisualElement Root => m_Root;

        protected override void BuildPartUI(VisualElement parent)
        {
            m_Root = new VisualElement { name = PartName };
            m_Root.AddToClassList(ussClassName);
            m_Root.AddToClassList(m_ParentClassName.WithUssElement(PartName));

            m_TitleLabel = new Label { name = "title-label" };
            m_TitleLabel.AddToClassList(titleUssClassName);
            m_SubTitleLabel = new Label { name = "sub-title-label" };
            m_SubTitleLabel.AddToClassList(subTitleUssClassName);

            m_Root.Add(m_TitleLabel);
            m_Root.Add(m_SubTitleLabel);

            parent.Add(m_Root);
        }

        protected override void UpdatePartFromModel()
        {
            if (m_Model is IBlackboardGraphModel graphProxyElement && graphProxyElement.Valid)
            {
                m_TitleLabel.text = graphProxyElement.GetBlackboardTitle();
                m_SubTitleLabel.text = graphProxyElement.GetBlackboardSubTitle();
            }
            else
            {
                m_TitleLabel.text = defaultTitle;
                m_SubTitleLabel.text = defaultSubTitle;
            }
        }
    }
}
