using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public class BlackboardVariablePropertyView : GraphElement
    {
        public static new readonly string ussClassName = "ge-blackboard-variable-property-view";
        public static readonly string rowUssClassName = ussClassName.WithUssElement("row");
        public static readonly string rowLabelUssClassName = ussClassName.WithUssElement("row-label");
        public static readonly string rowControlUssClassName = ussClassName.WithUssElement("row-control");

        public static readonly string rowTypeSelectorUssClassName = ussClassName.WithUssElement("row-type-selector");
        public static readonly string rowExposedUssClassName = ussClassName.WithUssElement("row-exposed");
        public static readonly string rowTooltipUssClassName = ussClassName.WithUssElement("row-tooltip");
        public static readonly string rowInitValueUssClassName = ussClassName.WithUssElement("row-init-value");

        protected Button m_TypeSelectorButton;
        protected Toggle m_ExposedToggle;
        protected TextField m_TooltipTextField;

        public BlackboardVariablePropertyView()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected override void BuildElementUI()
        {
            base.BuildElementUI();
            BuildRows();
        }

        protected virtual void BuildRows()
        {
            AddTypeSelector();
            AddExposedToggle();
            AddInitializationField();
            AddTooltipField();
        }

        protected override void PostBuildUI()
        {
            base.PostBuildUI();

            AddToClassList(ussClassName);
        }

        protected override void UpdateElementFromModel()
        {
            base.UpdateElementFromModel();

            if (Model is IVariableDeclarationModel variableDeclarationModel)
            {
                if (m_TypeSelectorButton != null)
                    m_TypeSelectorButton.text = GetTypeDisplayText();

                m_ExposedToggle?.SetValueWithoutNotify(variableDeclarationModel.IsExposed);
                m_TooltipTextField?.SetValueWithoutNotify(variableDeclarationModel.Tooltip);
            }
        }

        string GetTypeDisplayText()
        {
            if (Model is IVariableDeclarationModel variableDeclarationModel)
            {
                var stencil = Store.State.GraphModel.Stencil;
                return variableDeclarationModel.DataType.GetMetadata(stencil).FriendlyName;
            }

            return "";
        }

        VisualElement MakeRow(string labelText, VisualElement control, string ussClassName)
        {
            var row = new VisualElement { name = "blackboard-variable-property-view-row" };
            row.AddToClassList(rowUssClassName);
            if (!string.IsNullOrEmpty(ussClassName))
                row.AddToClassList(ussClassName);

            // TODO: Replace this with a variable pill/token and set isExposed appropriately
            var label = new Label(labelText);
            label.AddToClassList(rowLabelUssClassName);
            row.Add(label);

            if (control != null)
            {
                control.AddToClassList(rowControlUssClassName);
                row.Add(control);
            }

            return row;
        }

        protected void AddRow(string labelText, VisualElement control, string ussClassName)
        {
            var row = MakeRow(labelText, control, ussClassName);
            Add(row);
        }

        protected void InsertRow(int index, string labelText, VisualElement control, string ussClassName)
        {
            var row = MakeRow(labelText, control, ussClassName);
            Insert(index, row);
        }

        protected void AddTypeSelector()
        {
            void OnClick()
            {
                var pos = new Vector2(m_TypeSelectorButton.layout.xMin, m_TypeSelectorButton.layout.yMax);
                pos = m_TypeSelectorButton.parent.LocalToWorld(pos);
                // PF: FIX weird searcher position computation
                pos.y += 21;

                SearcherService.ShowVariableTypes(
                    Store.State.GraphModel.Stencil,
                    pos,
                    (t, i) => OnTypeChanged(t)
                );
            }

            m_TypeSelectorButton = new Button(OnClick) { text = GetTypeDisplayText() };
            AddRow("Type", m_TypeSelectorButton, rowTypeSelectorUssClassName);
        }

        protected void AddExposedToggle()
        {
            if (Model is IVariableDeclarationModel variableDeclarationModel)
            {
                if (variableDeclarationModel.VariableType == VariableType.GraphVariable)
                {
                    m_ExposedToggle = new Toggle { value = variableDeclarationModel.IsExposed };
                    AddRow("Exposed", m_ExposedToggle, rowExposedUssClassName);
                }
            }
        }

        protected void AddTooltipField()
        {
            if (Model is IVariableDeclarationModel variableDeclarationModel)
            {
                m_TooltipTextField = new TextField
                {
                    isDelayed = true,
                    value = variableDeclarationModel.Tooltip
                };
                AddRow("Tooltip", m_TooltipTextField, rowTooltipUssClassName);
            }
        }

        protected void AddInitializationField()
        {
            VisualElement initializationElement = null;

            if (Model is IVariableDeclarationModel variableDeclarationModel)
            {
                if (variableDeclarationModel.InitializationModel == null)
                {
                    var stencil = Store.State.GraphModel.Stencil;
                    if (stencil.RequiresInitialization(variableDeclarationModel))
                    {
                        initializationElement = new Button(OnInitializationButton) { text = "Create Init value" };
                    }
                }
                else
                {
                    initializationElement = InlineValueEditor.CreateEditorForConstant(Model.AssetModel, null, variableDeclarationModel.InitializationModel, (_, v) =>
                    {
                        Store.Dispatch(new UpdateConstantNodeValueAction(variableDeclarationModel.InitializationModel, v, null));
                    }, Store, false);
                }
            }

            if (initializationElement != null)
                AddRow("Initialization", initializationElement, rowInitValueUssClassName);
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_ExposedToggle?.UnregisterValueChangedCallback(OnExposedChanged);
            m_TooltipTextField.UnregisterValueChangedCallback(OnTooltipChanged);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            schedule.Execute(() => m_ExposedToggle?.RegisterValueChangedCallback(OnExposedChanged));
            m_TooltipTextField.RegisterValueChangedCallback(OnTooltipChanged);
        }

        void OnInitializationButton()
        {
            Store.Dispatch(new InitializeVariableAction(Model as IVariableDeclarationModel));
        }

        void OnTypeChanged(TypeHandle handle)
        {
            Store.Dispatch(new ChangeVariableTypeAction(Model as IVariableDeclarationModel, handle));
        }

        void OnExposedChanged(ChangeEvent<bool> evt)
        {
            Store.Dispatch(new UpdateExposedAction(Model as IVariableDeclarationModel, m_ExposedToggle.value));
        }

        void OnTooltipChanged(ChangeEvent<string> evt)
        {
            Store.Dispatch(new UpdateTooltipAction(Model as IVariableDeclarationModel, m_TooltipTextField.value));
        }
    }
}
