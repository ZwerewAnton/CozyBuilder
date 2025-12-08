using System.Threading;
using System.Threading.Tasks;
using _1_LEVEL_REWORK.New.Data;
using _1_LEVEL_REWORK.New.Instances;
using Common;
using FluentAssertions;
using Gameplay;
using Level;
using NSubstitute;
using NUnit.Framework;
using SaveSystem;
using Zenject;
using static Utils.CollectionsExt;

namespace Tests.EditMode
{
    public class LevelServiceTests : ZenjectUnitTestFixture
    {
        private ISaveLoadService _saveLoadService;
        private ILevelsRepository _levelsRepository;
        
        [SetUp]
        public void SetUp()
        {
            _saveLoadService = Substitute.For<ISaveLoadService>();
            _levelsRepository = Substitute.For<ILevelsRepository>();
            
            Container.Bind<ISaveLoadService>().FromInstance(_saveLoadService);
            Container.Bind<ILevelsRepository>().FromInstance(_levelsRepository);
            Container.Bind<LevelState>().AsSingle().NonLazy();
            Container.Bind<GameState>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelService>().AsSingle();
        }

        [Test]
        public void WhenInitializeLevelService_AndGotDetails_ThenCreateDetailDtos()
        {
            // Arrange.
            var levelService = Container.Resolve<LevelService>();
            var ground = Generator.CreateDetailData();
            var point = Generator.CreatePointData();
            var detail = Generator.CreateDetailData(points: ListOf(point));
            var levelData = Generator.CreateLevelData(ground, ListOf(detail));
            
            _levelsRepository.TryGetLevel(Arg.Any<string>(), out Arg.Any<LevelData>()).Returns(x =>
            {
                x[1] = levelData;
                return true;
            });

            // Act.
            levelService.InitializeLevel().GetAwaiter();

            // Assert.
            var detailDtos = levelService.GetDetailsInfo();
            detailDtos.Should().HaveCount(2);
            detailDtos.Should().ContainKey(ground.Id);
            detailDtos.Should().ContainKey(detail.Id);
            var detailDto = detailDtos[detail.Id];
            detailDto.CurrentCount.Should().Be(detail.Count);
            detailDto.IsGround.Should().BeFalse();
            detailDto.Points.Should().HaveCount(1);
        }

        [Test]
        public void WhenInitializeLevel_AndLevelIsCompleted_ThenInvokeLevelCompleteEvent()
        {
            // Arrange.
            var levelCompleted = false;
            var levelService = Container.Resolve<LevelService>();
            levelService.LevelCompleted += () => levelCompleted = true;
            var ground = Generator.CreateDetailData();
            var point = Generator.CreatePointData();
            var detail = Generator.CreateDetailData(points: ListOf(point));
            var levelData = Generator.CreateLevelData(ground, ListOf(detail));

            var pointSaveData = Generator.CreatePointSaveData(true);
            var detailSaveData = Generator.CreateDetailSaveData(id: detail.Id, count: 0, ListOf(pointSaveData));
            var levelSaveData = Generator.CreateLevelSaveData(ListOf(detailSaveData));
            
            _levelsRepository.TryGetLevel(Arg.Any<string>(), out Arg.Any<LevelData>()).Returns(x =>
            {
                x[1] = levelData;
                return true;
            });
            _saveLoadService.LoadLevelDataAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            ).Returns(Task.FromResult(levelSaveData));

            // Act.
            levelService.InitializeLevel().GetAwaiter();

            // Assert.
            levelCompleted.Should().BeTrue();
        }
    }
}