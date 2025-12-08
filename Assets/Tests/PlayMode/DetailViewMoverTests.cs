using System.Collections;
using Configs;
using FluentAssertions;
using Gameplay.Movement;
using Gameplay.Movement.Input;
using NSubstitute;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using static Utils.CollectionsExt;

namespace Tests.PlayMode
{
    public class DetailViewMoverTests : ZenjectIntegrationTestFixture
    {
        private IDetailViewMoverInputProvider _inputProvider;
        
        [SetUp]
        public void SetUp()
        {
            PreInstall();
            
            _inputProvider = Substitute.For<IDetailViewMoverInputProvider>();
            var configs = ScriptableObject.CreateInstance<ApplicationConfigs>();
            configs.gameplay = new GameplayConfig();
            var moverPrefab = AssetDatabase.LoadAssetAtPath<DetailViewMover>(
                "Assets/Tests/PlayMode/Prefabs/DetailViewMover.prefab");

            
            Container.Bind<IDetailViewMoverInputProvider>().FromInstance(_inputProvider);
            Container.Bind<ApplicationConfigs>().FromInstance(configs).AsSingle().NonLazy();
            var mover = Container.InstantiatePrefabForComponent<DetailViewMover>(moverPrefab.gameObject);
            Container.Bind<DetailViewMover>().FromInstance(mover).AsSingle().NonLazy();
            
            PostInstall();
        }
        
        
        [UnityTest]
        public IEnumerator WhenStopMove_AndDetailPositionIsOnPoint_ThenInvokePositivePlacementEnded()
        {
            // Arrange.
            var result = new PlacementResult
            {
                Success = false
            };
            var mover = Container.Resolve<DetailViewMover>();
            mover.PlacementEnded += placementResult => result = placementResult;
            var shader = Shader.Find("Standard");
            var material = new Material(shader);
            var pointPosition = new Vector3(5f, 6f, 10f);
            var point = Generator.CreatePointTransform(pointPosition, Quaternion.identity, 0);
            _inputProvider.GetDesiredPosition().Returns(pointPosition);
            _inputProvider.IsInputActive().Returns(true);
            mover.StartMove(new Mesh(), material, ListOf(point));
            yield return null;
            
            // Act.
            mover.StopMove();
            yield return null;
            
            // Assert.
            result.Success.Should().BeTrue();
        }
        
        [UnityTest]
        public IEnumerator WhenStopMove_AndDetailPositionIsNotOnPoint_ThenInvokeNegativePlacementEnded()
        {
            // Arrange.
            var result = new PlacementResult
            {
                Success = false
            };
            var mover = Container.Resolve<DetailViewMover>();
            mover.PlacementEnded += placementResult => result = placementResult;
            var shader = Shader.Find("Standard");
            var material = new Material(shader);
            var pointPosition = new Vector3(5f, 6f, 10f);
            var inputPosition = new Vector3(15f, 6f, 10f);
            var point = Generator.CreatePointTransform(pointPosition, Quaternion.identity, 0);
            _inputProvider.GetDesiredPosition().Returns(inputPosition);
            _inputProvider.IsInputActive().Returns(true);
            mover.StartMove(new Mesh(), material, ListOf(point));
            yield return null;
            
            // Act.
            mover.StopMove();
            yield return null;
            
            // Assert.
            result.Success.Should().BeFalse();
        }
    }
}