using System;
using Stateless;

namespace BugPro {
  public enum State { New, Analysis, Fixing, Closed, Return, Reopened }
  public enum Trigger {
    Analyze,
    NowNoTime,
    NeedSeparateSolution,
    AnotherProductProblem,
    NeedMoreInfo,
    StartFixing,
    NotDefect,
    DoNotFix,
    Dublication,
    NotRepotable,
    Ok,
    NotOk,
    Returning,
    ProblemSolved,
    ProblemNotSolved,
    Reopen
  }

  public class Bug {
    private readonly StateMachine<State, Trigger> _machine;

    public Bug(string title) {
      _machine = new StateMachine<State, Trigger>(State.New);

      _machine.Configure(State.New)
          .Permit(Trigger.Analyze, State.Analysis);

      _machine.Configure(State.Analysis)
        .PermitReentry(Trigger.NowNoTime)
        .PermitReentry(Trigger.NeedSeparateSolution)
        .PermitReentry(Trigger.AnotherProductProblem)
        .PermitReentry(Trigger.NeedMoreInfo)
        .Permit(Trigger.StartFixing, State.Fixing)
        .Permit(Trigger.NotDefect, State.Return)
        .Permit(Trigger.DoNotFix, State.Return)
        .Permit(Trigger.Dublication, State.Return)
        .Permit(Trigger.NotRepotable, State.Return)
        .Permit(Trigger.ProblemSolved, State.Closed)
        .Permit(Trigger.ProblemNotSolved, State.Reopened)
        .OnEntry(s => Console.WriteLine($"-> Состояние: Analysis (Баг: {title})"))
        .OnExit(s => Console.WriteLine($"-> Покидаем Analysis, переходим в {s.Destination}"));

      _machine.Configure(State.Fixing)
        .Permit(Trigger.Ok, State.Analysis)
        .Permit(Trigger.NotOk, State.Return)
        .OnEntry(s => Console.WriteLine($"-> Состояние: Fixing (Баг: {title})"))
        .OnExit(s => Console.WriteLine($"-> Покидаем Fixing, переходим в {s.Destination}"));

      _machine.Configure(State.Closed)
        .Permit(Trigger.Reopen, State.Analysis)
        .OnEntry(s => Console.WriteLine($"-> Состояние: Closed, закрываем {title}"))
        .OnExit(s => Console.WriteLine($"-> Покидаем Closed, переходим в {s.Destination}"));

      _machine.Configure(State.Return)
        .Permit(Trigger.Returning, State.Analysis)
        .OnEntry(s => Console.WriteLine($"-> Состояние: Return, возвращаемся к анализу"))
        .OnExit(s => Console.WriteLine($"-> Покидаем Return, переходим в {s.Destination}"));

      _machine.Configure(State.Reopened)
        .Permit(Trigger.Reopen, State.Analysis)
        .OnEntry(s => Console.WriteLine($"-> Состояние: Reopened, переоткрываем {title}"))
        .OnExit(s => Console.WriteLine($"-> Покидаем Reopened, переходим в {s.Destination}"));

      _machine.OnTransitioned(t =>
          Console.WriteLine($"Transition: {t.Source} -({t.Trigger})-> {t.Destination}"));
    }

    public void Fire(Trigger trigger) => _machine.Fire(trigger);
    public State CurrentState => _machine.State;
  }

  class Program {
    static void Main() {
      Console.WriteLine("BugPro: Stateless Workflow\n");

      var bug1 = new Bug("Критическая ошибка БД");

      bug1.Fire(Trigger.Analyze);
      bug1.Fire(Trigger.StartFixing);
      bug1.Fire(Trigger.Ok);
      bug1.Fire(Trigger.ProblemSolved);

      Console.WriteLine($"\nФинальный статус: {bug1.CurrentState}");

      var bug2 = new Bug("Утечка памяти");

      bug2.Fire(Trigger.Analyze);
      bug2.Fire(Trigger.NeedMoreInfo);
      bug2.Fire(Trigger.NowNoTime);
      bug2.Fire(Trigger.StartFixing);
      bug2.Fire(Trigger.Ok);
      bug2.Fire(Trigger.ProblemSolved);

      Console.WriteLine($"\nФинальный статус: {bug2.CurrentState}");
    }
  }
}