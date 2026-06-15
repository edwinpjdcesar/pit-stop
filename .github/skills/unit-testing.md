# Skill: Unit Testing

Use this skill whenever you are asked to write unit tests. It defines how to write tests that are fast, isolated, readable, and trustworthy.

This skill applies to any language that supports the following practices: **.NET (xUnit, NUnit, MSTest), Java (JUnit, TestNG), JavaScript/TypeScript (Jest, Vitest), Python (pytest, unittest), Go (testing package)**, and any other ecosystem that follows Arrange-Act-Assert patterns.

**Source:** [Microsoft — Best practices for writing unit tests](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

## Characteristics of a good unit test

Every test you write must be:

- **Fast** — takes milliseconds, not seconds
- **Isolated** — has no dependencies on a file system, database, network, or shared state
- **Repeatable** — always returns the same result when nothing has changed
- **Self-checking** — passes or fails automatically without human intervention
- **Timely** — the effort to write it should be proportional to the code under test

---

## Naming

Use the three-part naming convention: `MethodName_StateUnderTest_ExpectedBehavior`

```
// Bad
Test_Single()

// Good
Add_SingleNumber_ReturnsSameNumber()
GetUser_UserDoesNotExist_ReturnsNull()
PlaceOrder_InsufficientStock_ThrowsInvalidOperationException()
```

The name should read like a sentence describing a behavior. Anyone reading the test suite should understand what the code does without looking at the implementation.

---

## Arrange, Act, Assert (AAA)

Structure every test with three clearly separated sections:

```csharp
[Fact]
public void Add_EmptyString_ReturnsZero()
{
    // Arrange
    var calculator = new StringCalculator();

    // Act
    var actual = calculator.Add("");

    // Assert
    Assert.Equal(0, actual);
}
```

- **Arrange** — set up everything the test needs
- **Act** — execute exactly one action
- **Assert** — verify exactly one outcome

Never combine Act and Assert on the same line. The Act result must be captured in a variable first.

---

## One Act per test

Each test should have a single Act. Multiple acts require multiple asserts, and when one fails the others are skipped — making failures harder to diagnose.

```csharp
// Bad — two acts
var result1 = calculator.Add("");
var result2 = calculator.Add(",");
Assert.Equal(0, result1);
Assert.Equal(0, result2);

// Good — use parameterized tests instead
[Theory]
[InlineData("", 0)]
[InlineData(",", 0)]
public void Add_EmptyEntries_ReturnsZero(string input, int expected)
{
    var calculator = new StringCalculator();
    var actual = calculator.Add(input);
    Assert.Equal(expected, actual);
}
```

Use `[Theory]` / `@ParameterizedTest` / `pytest.mark.parametrize` / equivalent for data-driven cases — never a loop inside a single test.

---

## No logic in tests

Tests must not contain `if`, `for`, `while`, `switch`, string concatenation, or any other control flow. If you need branching, split into separate tests.

Logic in a test creates a bug surface inside your test suite — if the test itself has a bug, you can't trust it.

---

## No magic strings or numbers

Replace any hard-coded literal used more than once, or whose meaning is not obvious, with a named constant.

```csharp
// Bad
Assert.Throws<OverflowException>(() => calculator.Add("1001"));

// Good
const string MAXIMUM_RESULT = "1001";
Assert.Throws<OverflowException>(() => calculator.Add(MAXIMUM_RESULT));
```

---

## Minimally passing tests

Use the simplest input that proves the behavior. Don't set extra properties or use non-zero values unless the test specifically requires them — extra data obscures intent.

```csharp
// Bad — arbitrary value
calculator.Add("42");

// Good — minimal value that proves the behavior
calculator.Add("0");
```

---

## Helper methods, not Setup/Teardown

Avoid `[SetUp]` / `BeforeEach` / constructor-level shared state unless every test in the class genuinely needs identical setup. Prefer private helper methods that are called only where needed.

```csharp
// Preferred
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    var calculator = CreateDefaultCalculator();
    var actual = calculator.Add("0,1");
    Assert.Equal(1, actual);
}

private StringCalculator CreateDefaultCalculator() => new StringCalculator();
```

This keeps every test self-contained and avoids shared state bleeding between tests.

> **Note:** xUnit 2.x removed `[SetUp]` and `[TearDown]` intentionally. Constructor + `IDisposable` is the xUnit-approved pattern when shared setup is genuinely needed.

---

## Test public behavior, not private methods

Private methods are implementation details. Test the public method that calls the private one. If a private method seems to need its own test, it may need to become a public method on a new type.

---

## Mocks, stubs, and fakes

Use precise terminology:

- **Stub** — a controllable replacement that provides data to the system under test. You do *not* assert against it.
- **Mock** — a fake that you *do* assert against (verify it was called, or verify a property it recorded).
- **Fake** — a working alternative implementation (e.g., an in-memory database). Use for integration tests, not unit tests.

Use mocking frameworks (Moq, NSubstitute, Mockito, Jest mocks, etc.) to inject stubs/mocks via interfaces. Never mock concrete classes — if a class can't be mocked, it's a design signal to extract an interface.

---

## Static dependencies — use seams

If production code depends on a static call you can't control (e.g., `DateTime.Now`, `File.Exists`), wrap it behind an interface and inject the interface:

```csharp
public interface IDateTimeProvider
{
    DayOfWeek DayOfWeek();
}
```

This gives tests full control without changing runtime behavior.

---

## Avoid infrastructure in unit tests

Database access, file I/O, HTTP calls, and message queues belong in **integration tests**, not unit tests. If a class requires a real database to function:

1. Prefer extracting a repository interface and stubbing it in unit tests.
2. Use an in-memory provider or test container in a **separate** integration test project.
3. Never reference infrastructure packages (`EntityFrameworkCore`, `HttpClient`, etc.) from a unit test project.

---

## Test project structure

- Keep unit tests in a project separate from integration tests
- Mirror the source project's folder structure inside the test project
- Name test classes after the class under test: `VehicleServiceTests`, `OrderValidatorTests`
- One test class per class under test (can be split by scenario if the file grows large)

---

## Quick checklist before submitting tests

- [ ] Test name follows `MethodName_State_ExpectedBehavior`
- [ ] Test uses Arrange / Act / Assert sections with `// Arrange`, `// Act`, `// Assert` comments
- [ ] Exactly one Act per test
- [ ] No `if`, `for`, `while`, or `switch` inside a test method
- [ ] No magic strings or numbers — use named constants
- [ ] No dependencies on database, file system, or network
- [ ] Mocks/stubs are used for all external dependencies
- [ ] Test name clearly describes what will fail if the behavior breaks
