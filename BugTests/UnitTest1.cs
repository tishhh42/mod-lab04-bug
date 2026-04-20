using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugTests;

[TestClass]
public class UnitTest1 {
  [TestMethod]
  public void CreateBug_ShouldStartInNewState() {
    var bug = new Bug("Тестовый баг");

    Assert.AreEqual(State.New, bug.CurrentState);
  }
  [TestMethod]
  public void Analyze_ShouldTransitionFromNewToAnalysis() {
    var bug = new Bug("Тестовый баг");

    bug.Fire(Trigger.Analyze);

    Assert.AreEqual(State.Analysis, bug.CurrentState);
  }
  [TestMethod]
  public void Analyze_ShouldNotBeAllowedFromWrongState() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);

    Assert.ThrowsException<InvalidOperationException>(() => bug.Fire(Trigger.Analyze));
  }
  [TestMethod]
  public void StartFixing_ShouldTransitionFromAnalysisToFixing() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);

    bug.Fire(Trigger.StartFixing);

    Assert.AreEqual(State.Fixing, bug.CurrentState);
  }
  [TestMethod]
  public void Ok_ShouldTransitionFromFixingToAnalysis() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);
    bug.Fire(Trigger.StartFixing);

    bug.Fire(Trigger.Ok);

    Assert.AreEqual(State.Analysis, bug.CurrentState);
  }
  [TestMethod]
  public void NotOk_ShouldTransitionFromFixingToReturn() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);
    bug.Fire(Trigger.StartFixing);

    bug.Fire(Trigger.NotOk);

    Assert.AreEqual(State.Return, bug.CurrentState);
  }
  [TestMethod]
  public void ProblemSolved_ShouldTransitionFromAnalysisToClosed() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);

    bug.Fire(Trigger.ProblemSolved);

    Assert.AreEqual(State.Closed, bug.CurrentState);
  }
  [TestMethod]
  public void ProblemNotSolved_ShouldTransitionFromAnalysisToReopened() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);

    bug.Fire(Trigger.ProblemNotSolved);

    Assert.AreEqual(State.Reopened, bug.CurrentState);
  }
  [TestMethod]
  public void Reopen_ShouldTransitionFromClosedToAnalysis() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);
    bug.Fire(Trigger.ProblemSolved);
    Assert.AreEqual(State.Closed, bug.CurrentState);

    bug.Fire(Trigger.Reopen);

    Assert.AreEqual(State.Analysis, bug.CurrentState);
  }
  [TestMethod]
  public void NotDefect_ShouldTransitionFromAnalysisToReturn() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);

    bug.Fire(Trigger.NotDefect);

    Assert.AreEqual(State.Return, bug.CurrentState);
  }
  [TestMethod]
  public void Returning_ShouldTransitionFromReturnToAnalysis() {
    var bug = new Bug("Тестовый баг");
    bug.Fire(Trigger.Analyze);
    bug.Fire(Trigger.NotDefect);
    Assert.AreEqual(State.Return, bug.CurrentState);

    bug.Fire(Trigger.Returning);

    Assert.AreEqual(State.Analysis, bug.CurrentState);
  }
}
