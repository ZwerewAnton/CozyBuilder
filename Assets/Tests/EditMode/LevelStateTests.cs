using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Data;
using _1_LEVEL_REWORK.New.Instances;
using FluentAssertions;
using NUnit.Framework;
using SaveSystem.DataObjects.Level.New;
using static Utils.CollectionsExt;

namespace Tests.EditMode
{
    public class LevelStateTests
    {
        [Test]
        public void WhenCreatingLevelStateWithOnlyGroundDetail_AndInstancesIsEmpty_ThenInstancesShouldBe1()
        {
            // Arrange.
            var levelState = new LevelState();
            var ground = Generator.CreateDetailData();

            // Act.
            levelState.CreateLevelState(ground, new List<DetailData>(), new List<DetailSaveData>());

            // Assert.
            levelState.Details.Should().HaveCount(1);
        }

        [Test]
        public void WhenCreatingLevelStateWithGroundAndDetails_AndInstancesIsEmpty_ThenInstancesShouldBeSameCount()
        {
            // Arrange.
            var levelState = new LevelState();
            var ground = Generator.CreateDetailData();
            var details = ListOf(Generator.CreateDetailData(), Generator.CreateDetailData());

            // Act.
            levelState.CreateLevelState(ground, details, new List<DetailSaveData>());

            // Assert.
            levelState.Details.Should().HaveCount(3);
        }

        [Test]
        public void WhenCreatingLevelState_AndAddDetailWithGroundParentConstraint_ThenPointIsReady()
        {
            // Arrange.
            var levelState = new LevelState();
            var ground = Generator.CreateDetailData();
            var parentConstraint = Generator.CreateParentConstraint(ground, ListOf(0));
            var point = Generator.CreatePointData(ListOf(parentConstraint));
            var detail = Generator.CreateDetailData(points: ListOf(point));

            // Act.
            levelState.CreateLevelState(ground, ListOf(detail), new List<DetailSaveData>());
            var pointInstance = levelState.Details[detail.Id].Points[0];

            // Assert.
            levelState.IsPointReady(pointInstance).Should().BeTrue();
        }

        [Test]
        public void WhenCreatingLevelState_AndAddDetailsWithConstraint_ThenPointIsNotReady()
        {
            // Arrange.
            var levelState = new LevelState();
            var groundPoint = Generator.CreatePointData();
            var ground = Generator.CreateDetailData(points: ListOf(groundPoint));

            var parentParentConstraint = Generator.CreateParentConstraint(ground, ListOf(0));
            var parentPoint = Generator.CreatePointData(ListOf(parentParentConstraint));
            var parentDetail = Generator.CreateDetailData(points: ListOf(parentPoint));

            var parentConstraint = Generator.CreateParentConstraint(parentDetail, ListOf(0));
            var point = Generator.CreatePointData(ListOf(parentConstraint));
            var detail = Generator.CreateDetailData(points: ListOf(point));

            // Act.
            levelState.CreateLevelState(ground, ListOf(parentDetail, detail), new List<DetailSaveData>());
            var pointInstance = levelState.Details[detail.Id].Points[0];

            // Assert.
            levelState.IsPointReady(pointInstance).Should().BeFalse();
        }

        [Test]
        public void WhenTryInstallingDetail_AndPointIsReady_ThenPointIsInstalled()
        {
            // Arrange.
            var levelState = new LevelState();
            var groundPoint = Generator.CreatePointData();
            var ground = Generator.CreateDetailData(points: ListOf(groundPoint));

            var parentConstraint = Generator.CreateParentConstraint(ground, ListOf(0));
            var point = Generator.CreatePointData(ListOf(parentConstraint));
            var detail = Generator.CreateDetailData(points: ListOf(point));

            levelState.CreateLevelState(ground, ListOf(detail), new List<DetailSaveData>());

            // Act.
            var result = levelState.TryInstallDetail(detail.Id, 0);

            // Assert.
            result.Should().BeTrue();
            levelState.Details[detail.Id].Points[0].IsInstalled.Should().BeTrue();
        }

        [Test]
        public void WhenTryInstallingDetail_AndPointIsNotReady_ThenPointIsNotInstalled()
        {
            // Arrange.
            var levelState = new LevelState();
            var groundPoint = Generator.CreatePointData();
            var ground = Generator.CreateDetailData(points: ListOf(groundPoint));

            var parentParentConstraint = Generator.CreateParentConstraint(ground, ListOf(0));
            var parentPoint = Generator.CreatePointData(ListOf(parentParentConstraint));
            var parentDetail = Generator.CreateDetailData(points: ListOf(parentPoint));

            var parentConstraint = Generator.CreateParentConstraint(parentDetail, ListOf(0));
            var point = Generator.CreatePointData(ListOf(parentConstraint));
            var detail = Generator.CreateDetailData(points: ListOf(point));

            levelState.CreateLevelState(ground, ListOf(parentDetail, detail), new List<DetailSaveData>());

            // Act.
            var result = levelState.TryInstallDetail(detail.Id, 0);

            // Assert.
            result.Should().BeFalse();
            levelState.Details[detail.Id].Points[0].IsInstalled.Should().BeFalse();
        }
    }
}