using System;
using NUnit.Framework;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.Graph
{
    class TestGraph : ICreatableGraphTemplate
    {
        public Type StencilType => typeof(ClassStencil);
        public string GraphTypeName => "Test Graph";
        public string DefaultAssetName => "testgraph";

        public void InitBasicGraph(IGraphModel graph)
        {
            AssetDatabase.SaveAssets();
        }
    }

    class GraphSerialization : BaseFixture
    {
        protected override bool CreateGraphOnStartup => false;

        [Test]
        public void LoadGraphActionLoadsCorrectGraph()
        {
            GraphAssetCreationHelpers<TestGraphAssetModel>.CreateGraphAsset(typeof(ClassStencil), "test", k_GraphPath);
            AssumeIntegrity();

            AssetDatabase.SaveAssets();
            Resources.UnloadAsset(m_Store.State.AssetModel as Object);
            m_Store.Dispatch(new LoadGraphAssetAction(k_GraphPath));
            Assert.AreEqual(k_GraphPath, AssetDatabase.GetAssetPath((Object)GraphModel.AssetModel));
            AssertIntegrity();

            AssetDatabase.DeleteAsset(k_GraphPath);
        }

        [Test]
        public void CreateGraphAssetBuildsValidGraphModel()
        {
            GraphAssetCreationHelpers<TestGraphAssetModel>.CreateInMemoryGraphAsset(typeof(ClassStencil), "test", k_GraphPath);
            AssumeIntegrity();
        }

        [Test]
        public void CreateGraphAssetWithTemplateBuildsValidGraphModel()
        {
            var graphTemplate = new TestGraph();
            GraphAssetCreationHelpers<TestGraphAssetModel>.CreateInMemoryGraphAsset(typeof(ClassStencil), graphTemplate.DefaultAssetName, k_GraphPath, graphTemplate);
            AssertIntegrity();
        }

        [Test]
        public void CreateTestGraphCanBeReloaded()
        {
            var graphTemplate = new TestGraph();
            GraphAssetCreationHelpers<TestGraphAssetModel>.CreateGraphAsset(typeof(ClassStencil), graphTemplate.DefaultAssetName, k_GraphPath, graphTemplate);

            GraphModel graph = AssetDatabase.LoadAssetAtPath<GraphAssetModel>(k_GraphPath)?.GraphModel as GraphModel;
            Resources.UnloadAsset((Object)graph?.AssetModel);
            m_Store.Dispatch(new LoadGraphAssetAction(k_GraphPath));

            AssertIntegrity();

            AssetDatabase.DeleteAsset(k_GraphPath);
        }

        [Test]
        public void CreateTestGraphFromAssetModel()
        {
            var graphTemplate = new TestGraph();
            var assetModel = ScriptableObject.CreateInstance<TestGraphAssetModel>();
            var doCreateAction = ScriptableObject.CreateInstance<DoCreateAsset>();
            doCreateAction.SetUp(null, assetModel, graphTemplate);
            doCreateAction.CreateAndLoadAsset(k_GraphPath);
            AssertIntegrity();
        }
    }
}
