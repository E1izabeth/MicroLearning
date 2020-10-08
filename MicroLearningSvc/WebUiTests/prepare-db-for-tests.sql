
/****** Object:  Table [dbo].[DbResourceAssociationInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbResourceAssociationInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TagInstanceId] [bigint] NOT NULL,
	[ResourceId] [bigint] NOT NULL,
 CONSTRAINT [PK_DbResourceAssociationInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbResourceContentPortionInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbResourceContentPortionInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResourceId] [bigint] NOT NULL,
	[Order] [bigint] NOT NULL,
	[TextContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_DbResourceContentPortionInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbResourceInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbResourceInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](150) NULL,
	[Title] [nvarchar](150) NULL,
	[AuthorUserId] [bigint] NOT NULL,
	[CreationStamp] [datetime] NOT NULL,
	[IsValidated] [bit] NOT NULL,
	[ValidatorUserId] [bigint] NOT NULL,
	[ValidationStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_DbResourceInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbTagInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbTagInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_DbTagInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbTagInstanceInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbTagInstanceInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TagId] [bigint] NOT NULL,
	[Word] [nvarchar](150) NULL,
 CONSTRAINT [PK_DbTagInstanceInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbTopicAssociationInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbTopicAssociationInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TagInstanceId] [bigint] NOT NULL,
	[TopicId] [bigint] NOT NULL,
 CONSTRAINT [PK_DbTopicAssociationInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbTopicInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbTopicInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AuthorUserId] [bigint] NOT NULL,
	[CreationStamp] [datetime] NOT NULL,
	[Title] [nvarchar](150) NULL,
 CONSTRAINT [PK_DbTopicInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbUserInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbUserInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](150) NULL,
	[LoginKey] [nvarchar](150) NULL,
	[RegistrationStamp] [datetime] NOT NULL,
	[LastLoginStamp] [datetime] NOT NULL,
	[PasswordHash] [nvarchar](150) NULL,
	[HashSalt] [nvarchar](150) NULL,
	[Email] [nvarchar](150) NULL,
	[Activated] [bit] NOT NULL,
	[LastToken] [nvarchar](150) NULL,
	[LastTokenStamp] [datetime] NOT NULL,
	[LastTokenKind] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DbUserInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbUserSubscriptionContentPortionInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbUserSubscriptionContentPortionInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserSubscriptionId] [bigint] NOT NULL,
	[ResourceContentPortionId] [bigint] NOT NULL,
	[IsLearned] [bit] NOT NULL,
	[LearnedStamp] [datetime] NOT NULL,
	[IsDelivered] [bit] NOT NULL,
	[DeliveredStamp] [datetime] NOT NULL,
	[IsMarkedToRepeat] [bit] NOT NULL,
 CONSTRAINT [PK_DbUserSubscriptionContentPortionInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbUserSubscriptionInfo]    Script Date: 06.05.2020 17:13:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbUserSubscriptionInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[TopicId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Interval] [bigint] NOT NULL,
	[NextPortionTime] [datetime] NOT NULL,
 CONSTRAINT [PK_DbUserSubscriptionInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DbResourceAssociationInfo] ON 
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (1, 1, 1)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (2, 2, 1)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (3, 3, 1)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (4, 1, 2)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (5, 2, 2)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (6, 4, 3)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (7, 5, 4)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (8, 6, 4)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (9, 7, 5)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (10, 8, 5)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (11, 9, 5)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (12, 10, 5)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (13, 11, 5)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (10006, 7, 10003)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (10007, 10, 10003)
GO
INSERT [dbo].[DbResourceAssociationInfo] ([Id], [TagInstanceId], [ResourceId]) VALUES (10008, 7, 10006)
GO
SET IDENTITY_INSERT [dbo].[DbResourceAssociationInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbResourceContentPortionInfo] ON 
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (1, 1, 0, N'11/04/2016
Время чтения: 2 мин

В этой статье

Модульные тесты позволяют разработчикам и тест-инженерам быстро искать логические ошибки в методах классов для проектов на языках C#, Visual Basic и C++.Unit tests give developers and testers a quick way to look for logic errors in the methods of classes in C#, Visual Basic, and C++ projects.

Средства модульных тестов включают:The unit test tools include:

Обозреватель тестов—Вы можете запускать модульные тесты и просматривать их результаты с помощью обозревателя тестов.Test Explorer—Run unit tests and see their results in Test Explorer. Вы можете использовать любые тестовые платформы, в том числе сторонние платформы, которые имеют адаптер для обозревателя тестов.You can use any unit test framework, including a third-party framework, that has an adapter for Test Explorer.

Платформа модульного тестирования Майкрософт для управляемого кода—Платформа для тестирования Майкрософт для управляемого кода устанавливается с Visual Studio и предоставляет среду для тестирования кода в .NET.Microsoft unit test framework for managed code—The Microsoft unit test framework for managed code is installed with Visual Studio and provides a framework for testing .NET code.

Платформа модульного тестирования Майкрософт для C++ —Платформа модульного тестирования Майкрософт для C++ устанавливается в составе рабочей нагрузки Разработка классических приложений на C++ .Microsoft unit test framework for C++—The Microsoft unit test framework for C++ is installed as part of the Desktop development with C++ workload. Эта платформа обеспечивает тестирование машинного кода.It provides a framework for testing native code. Вдобавок включаются платформы Google Test, Boost.Test и CTest, а также сторонние адаптеры для дополнительных платформ тестирования.Google Test, Boost.Test, and CTest frameworks are also included, and third-party adapters are available for additional test frameworks. Дополнительные сведения см. в статье Создание модульных тестов для C/C++.For more information, see Write unit tests for C/C++.

Инструменты покрытия кода—Можно определить объем кода продукта, который покрывают модульные тесты, при помощи одной команды в обозревателе тестов.Code coverage tools—You can determine the amount of product code that your unit tests exercise from one command in Test Explorer.

Платформа изоляции Microsoft Fakes—Границы изоляции Microsoft Fakes могут создать постановочные классы и методы для рабочего кода и систем, которые создают зависимости в тестируемом коде.Microsoft Fakes isolation framework—The Microsoft Fakes isolation framework can create substitute classes and methods for production and system code that create dependencies in the code under test. Путем реализации подставных делегатов для функции можно контролировать поведение и возвращаемые значения объекта зависимости.By implementing the fake delegates for a function, you control the behavior and output of the dependency object.

Кроме того, можно использовать компонент IntelliTest, чтобы изучить код .NET и создать тестовые данные и набор модульных тестов.You can also use IntelliTest to explore your .NET code to generate test data and a suite of unit tests. Для каждого оператора в коде создаются входные данные теста, которые будут выполнять этот оператор.For every statement in the code, a test input is generated that will execute that statement. Анализ случая выполняется для каждой условной ветви в коде.A case analysis is performed for every conditional branch in the code.

Ключевые задачиKey tasks

Следующие разделы помогут в понимании и создании модульных тестов.Use the following articles to help with understanding and creating unit tests:

ЗадачиTasks
Связанные разделыAssociated Topics

Краткие и пошаговые руководства. Здесь можно изучить модульное тестирование в Visual Studio на конкретных примерах кода.Quickstarts and walkthroughs: Learn about unit testing in Visual Studio from code examples.
- Пошаговое руководство: создание и запуск модульных тестов для управляемого кода- Walkthrough: Create and run unit tests for managed code
- Краткое руководство. Разработка на основе тестирования с помощью обозревателя тестов- Quickstart: Test-driven development with Test Explorer
- Практическое руководство. Добавление модульных тестов для приложений на C++- How to: Add unit tests to C++ apps

Модульное тестирование с помощью обозревателя тестов. Узнайте, как с помощью обозревателя тестов создавать более производительные и более эффективные модульные тесты.Unit testing with Test Explorer: Learn how Test Explorer can help create more productive and efficient unit tests.
- Основные сведения о модульных тестах- Unit test basics
- Create a unit test project (Создание проекта модульного теста)- Create a unit test project
- Выполнение модульных тестов с помощью обозревателя тестов- Run unit tests with Test Explorer
- Install third-party unit test frameworks (Установка платформ модульного тестирования сторонних поставщиков)- Install third-party unit test frameworks


Модульное тестирование кода C++Unit test C++ code
- Написание модульных тестов для C и C++- Write unit tests for C/C++

Изоляция модульных тестовIsolating unit tests
- Изоляция тестируемого кода с помощью Microsoft Fakes- Isolate code under test with Microsoft Fakes

Использование покрытия кода для определения того, какая часть кода проекта тестируется. Изучите возможности покрытия кода, которые предоставляют средства тестирования Visual Studio.Use code coverage to identify what proportion of your project''s code is tested: Learn about the code coverage feature of Visual Studio testing tools.
- Использование параметра объема протестированного кода для определения объема протестированного кода- Use code coverage to determine how much code is being tested

Анализ нагрузки и производительности с помощью нагрузочных тестов. Вы можете создать нагрузочные тесты, чтобы выявить проблемы с нагрузкой и производительностью в приложении.Perform stress and performance analysis by using load tests: Learn how to create load tests to help isolate performance and stress issues in your application.
- Краткое руководство. Создание проекта нагрузочного тестирования.- Quickstart: Create a load test project
- Нагрузочное тестирование (Azure Test Plans и TFS)- Load testing (Azure Test Plans and TFS)

Установка системы контроля качества. Вы можете создать систему контроля качества, чтобы выполнять тесты перед сохранением или объединением кода.Set quality gates: Learn how to create quality gates to enforce that tests are run before code is checked in or merged.
- Политики возврата (Azure Repos TFVC)- Check-in policies (Azure Repos TFVC)

Задание параметров тестирования. Сведения о настройке параметров теста, например места, где хранятся результаты теста.Set testing options: Learn how to configure test options, for example, where test results are stored.
Настройка модульных тестов с помощью файла .runsettingsConfigure unit tests by using a .runsettings file

Справочная документация по APIAPI reference documentation

Microsoft.VisualStudio.TestTools.UnitTesting описывает пространство имен UnitTesting, предоставляющего атрибуты, исключения, утверждения и другие классы, поддерживающие модульное тестирование.Microsoft.VisualStudio.TestTools.UnitTesting describes the UnitTesting namespace, which provides attributes, exceptions, asserts, and other classes that support unit testing.
В Microsoft.VisualStudio.TestTools.UnitTesting.Web описано пространство имен UnitTesting.Web, расширяющее пространство имен UnitTesting за счет поддержки ASP.NET и модульных тестов веб-службы.Microsoft.VisualStudio.TestTools.UnitTesting.Web describes the UnitTesting.Web namespace, which extends the UnitTesting namespace by providing support for ASP.NET and web service unit tests.

См. такжеSee also

Улучшение качества кодаImprove code quality

Похожие статьи




Обратная связь

Отправить отзыв о следующем:
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (2, 2, 0, N'Software development
Core activities
Processes
Requirements
Design
Engineering
Construction
Testing
Debugging
Deployment
Maintenance

Paradigms and models
Agile
Cleanroom
Incremental
Prototyping
Spiral
V model
Waterfall

Methodologies and frameworks
ASD
DevOps
DAD
DSDM
FDD
IID
Kanban
Lean SD
LeSS
MDD
MSF
PSP
RAD
RUP
SAFe
Scrum
SEMAT
TSP
OpenUP
UP
XP

Supporting disciplines
Configuration management
Documentation
Software quality assurance (SQA)
Project management
User experience

Practices
ATDD
BDD
CCO
CI
CD
DDD
PP
SBE
Stand-up
TDD

Tools
Compiler
Debugger
Profiler
GUI designer
Modeling
IDE
Build automation
Release automation
Infrastructure as code
Testing

Standards and Bodies of Knowledge
BABOK
CMMI
IEEE standards
ISO 9001
ISO/IEC standards
PMBOK
SWEBOK
ITIL

Glossaries
Artificial intelligence
Computer science
Electrical and electronics engineering

Outlines
Outline of software development
vte

In computer programming, unit testing is a software testing method by which individual units of source code, sets of one or more computer program modules together with associated control data, usage procedures, and operating procedures, are tested to determine whether they are fit for use.[1]

Description[edit]

Unit tests are typically automated tests written and run by software developers to ensure that a section of an application (known as the "unit") meets its design and behaves as intended.[2] In procedural programming, a unit could be an entire module, but it is more commonly an individual function or procedure. In object-oriented programming, a unit is often an entire interface, such as a class, but could be an individual method.[3] By writing tests first for the smallest testable units, then the compound behaviors between those, one can build up comprehensive tests for complex applications.[2]

To isolate issues that may arise, each test case should be tested independently. Substitutes such as method stubs, mock objects,[4] fakes, and test harnesses can be used to assist testing a module in isolation.

During development, a software developer may code criteria, or results that are known to be good, into the test to verify the unit''s correctness. During test case execution, frameworks log tests that fail any criterion and report them in a summary.

Writing and maintaining unit tests can be made faster by using Parameterized Tests (PUTs). These allow the execution of one test multiple times with different input sets, thus reducing test code duplication. Unlike traditional unit tests, which are usually closed methods and test invariant conditions, PUTs take any set of parameters. PUTs have been supported by TestNG, JUnit and its .Net counterpart, XUnit. Suitable parameters for the unit tests may be supplied manually or in some cases are automatically generated by the test framework. In recent years support was added for writing more powerful (unit) tests, leveraging the concept of theories, test cases that execute the same steps, but using test data generated at runtime, unlike regular parameterized tests that use the same execution steps with input sets that are pre-defined.[5][6][7]

Advantages[edit]

The goal of unit testing is to isolate each part of the program and show that the individual parts are correct.[1] A unit test provides a strict, written contract that the piece of code must satisfy. As a result, it affords several benefits.

Unit testing finds problems early in the development cycle. This includes both bugs in the programmer''s implementation and flaws or missing parts of the specification for the unit. The process of writing a thorough set of tests forces the author to think through inputs, outputs, and error conditions, and thus more crisply define the unit''s desired behavior. The cost of finding a bug before coding begins or when the code is first written is considerably lower than the cost of detecting, identifying, and correcting the bug later. Bugs in released code may also cause costly problems for the end-users of the software.[8][9][10] Code can be impossible or difficult to unit test if poorly written, thus unit testing can force developers to structure functions and objects in better ways.

In test-driven development (TDD), which is frequently used in both extreme programming and scrum, unit tests are created before the code itself is written. When the tests pass, that code is considered complete. The same unit tests are run against that function frequently as the larger code base is developed either as the code is changed or via an automated process with the build. If the unit tests fail, it is considered to be a bug either in the changed code or the tests themselves. The unit tests then allow the location of the fault or failure to be easily traced. Since the unit tests alert the development team of the problem before handing the code off to testers or clients, potential problems are caught early in the development process.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (3, 2, 1, N'
Unit testing allows the programmer to refactor code or upgrade system libraries at a later date, and make sure the module still works correctly (e.g., in regression testing). The procedure is to write test cases for all functions and methods so that whenever a change causes a fault, it can be quickly identified. Unit tests detect changes which may break a design contract.

Unit testing may reduce uncertainty in the units themselves and can be used in a bottom-up testing style approach. By testing the parts of a program first and then testing the sum of its parts, integration testing becomes much easier.[citation needed]

Unit testing provides a sort of living documentation of the system. Developers looking to learn what functionality is provided by a unit, and how to use it, can look at the unit tests to gain a basic understanding of the unit''s interface (API).[citation needed]

Unit test cases embody characteristics that are critical to the success of the unit. These characteristics can indicate appropriate/inappropriate use of a unit as well as negative behaviors that are to be trapped by the unit. A unit test case, in and of itself, documents these critical characteristics, although many software development environments do not rely solely upon code to document the product in development.[citation needed]

When software is developed using a test-driven approach, the combination of writing the unit test to specify the interface plus the refactoring activities performed after the test has passed, may take the place of formal design. Each unit test can be seen as a design element specifying classes, methods, and observable behavior.[citation needed]

Limitations and disadvantages[edit]

Testing will not catch every error in the program, because it cannot evaluate every execution path in any but the most trivial programs. This problem is a superset of the halting problem, which is undecidable. The same is true for unit testing. Additionally, unit testing by definition only tests the functionality of the units themselves. Therefore, it will not catch integration errors or broader system-level errors (such as functions performed across multiple units, or non-functional test areas such as performance). Unit testing should be done in conjunction with other software testing activities, as they can only show the presence or absence of particular errors; they cannot prove a complete absence of errors. To guarantee correct behavior for every execution path and every possible input, and ensure the absence of errors, other techniques are required, namely the application of formal methods to proving that a software component has no unexpected behavior.[citation needed]

An elaborate hierarchy of unit tests does not equal integration testing. Integration with peripheral units should be included in integration tests, but not in unit tests.[citation needed] Integration testing typically still relies heavily on humans testing manually; high-level or global-scope testing can be difficult to automate, such that manual testing often appears faster and cheaper.[citation needed]

Software testing is a combinatorial problem. For example, every Boolean decision statement requires at least two tests: one with an outcome of "true" and one with an outcome of "false". As a result, for every line of code written, programmers often need 3 to 5 lines of test code.[11] This obviously takes time and its investment may not be worth the effort. There are problems that cannot easily be tested at all – for example those that are nondeterministic or involve multiple threads. In addition, code for a unit test is likely to be at least as buggy as the code it is testing. Fred Brooks in The Mythical Man-Month quotes: "Never go to sea with two chronometers; take one or three."[12] Meaning, if two chronometers contradict, how do you know which one is correct?

Another challenge related to writing the unit tests is the difficulty of setting up realistic and useful tests. It is necessary to create relevant initial conditions so the part of the application being tested behaves like part of the complete system. If these initial conditions are not set correctly, the test will not be exercising the code in a realistic context, which diminishes the value and accuracy of unit test results.[13]

To obtain the intended benefits from unit testing, rigorous discipline is needed throughout the software development process. It is essential to keep careful records not only of the tests that have been performed, but also of all changes that have been made to the source code of this or any other unit in the software. Use of a version control system is essential. If a later version of the unit fails a particular test that it had previously passed, the version-control software can provide a list of the source code changes (if any) that have been applied to the unit since that time.[citation needed]

It is also essential to implement a sustainable process for ensuring that test case failures are reviewed regularly and addressed immediately.[14] If such a process is not implemented and ingrained into the team''s workflow, the application will evolve out of sync with the unit test suite, increasing false positives and reducing the effectiveness of the test suite.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (4, 2, 2, N'
Unit testing embedded system software presents a unique challenge: Because the software is being developed on a different platform than the one it will eventually run on, you cannot readily run a test program in the actual deployment environment, as is possible with desktop programs.[15]

Unit tests tend to be easiest when a method has input parameters and some output. It is not as easy to create unit tests when a major function of the method is to interact with something external to the application. For example, a method that will work with a database might require a mock up of database interactions to be created, which probably won''t be as comprehensive as the real database interactions.[16][better source needed]

Example[edit]

Here is a set of test cases in Java that specify a number of elements of the implementation. First, that there must be an interface called Adder, and an implementing class with a zero-argument constructor called AdderImpl. It goes on to assert that the Adder interface should have a method called add, with two integer parameters, which returns another integer. It also specifies the behaviour of this method for a small range of values over a number of test methods.

import static org.junit.Assert.*;

import org.junit.Test;

public class TestAdder {

@Test
public void testSumPositiveNumbersOneAndOne() {
Adder adder = new AdderImpl();
assert(adder.add(1, 1) == 2);
}

// can it add the positive numbers 1 and 2?
@Test
public void testSumPositiveNumbersOneAndTwo() {
Adder adder = new AdderImpl();
assert(adder.add(1, 2) == 3);
}

// can it add the positive numbers 2 and 2?
@Test
public void testSumPositiveNumbersTwoAndTwo() {
Adder adder = new AdderImpl();
assert(adder.add(2, 2) == 4);
}

// is zero neutral?
@Test
public void testSumZeroNeutral() {
Adder adder = new AdderImpl();
assert(adder.add(0, 0) == 0);
}

// can it add the negative numbers -1 and -2?
@Test
public void testSumNegativeNumbers() {
Adder adder = new AdderImpl();
assert(adder.add(-1, -2) == -3);
}

// can it add a positive and a negative?
@Test
public void testSumPositiveAndNegative() {
Adder adder = new AdderImpl();
assert(adder.add(-1, 1) == 0);
}

// how about larger numbers?
@Test
public void testSumLargeNumbers() {
Adder adder = new AdderImpl();
assert(adder.add(1234, 988) == 2222);
}

}

In this case the unit tests, having been written first, act as a design document specifying the form and behaviour of a desired solution, but not the implementation details, which are left for the programmer. Following the "do the simplest thing that could possibly work" practice, the easiest solution that will make the test pass is shown below.

interface Adder {
int add(int a, int b);
}
class AdderImpl implements Adder {
public int add(int a, int b) {
return a + b;
}
}

As executable specifications[edit]

Using unit-tests as a design specification has one significant advantage over other design methods: The design document (the unit-tests themselves) can itself be used to verify the implementation. The tests will never pass unless the developer implements a solution according to the design.

Unit testing lacks some of the accessibility of a diagrammatic specification such as a UML diagram, but they may be generated from the unit test using automated tools. Most modern languages have free tools (usually available as extensions to IDEs). Free tools, like those based on the xUnit framework, outsource to another system the graphical rendering of a view for human consumption.

Applications[edit]
Extreme programming[edit]

Unit testing is the cornerstone of extreme programming, which relies on an automated unit testing framework. This automated unit testing framework can be either third party, e.g., xUnit, or created within the development group.

Extreme programming uses the creation of unit tests for test-driven development. The developer writes a unit test that exposes either a software requirement or a defect. This test will fail because either the requirement isn''t implemented yet, or because it intentionally exposes a defect in the existing code. Then, the developer writes the simplest code to make the test, along with other tests, pass.

Most code in a system is unit tested, but not necessarily all paths through the code. Extreme programming mandates a "test everything that can possibly break" strategy, over the traditional "test every execution path" method. This leads developers to develop fewer tests than classical methods, but this isn''t really a problem, more a restatement of fact, as classical methods have rarely ever been followed methodically enough for all execution paths to have been thoroughly tested.[citation needed] Extreme programming simply recognizes that testing is rarely exhaustive (because it is often too expensive and time-consuming to be economically viable) and provides guidance on how to effectively focus limited resources.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (5, 2, 3, N'
Crucially, the test code is considered a first class project artifact in that it is maintained at the same quality as the implementation code, with all duplication removed. Developers release unit testing code to the code repository in conjunction with the code it tests. Extreme programming''s thorough unit testing allows the benefits mentioned above, such as simpler and more confident code development and refactoring, simplified code integration, accurate documentation, and more modular designs. These unit tests are also constantly run as a form of regression test.

Unit testing is also critical to the concept of Emergent Design. As emergent design is heavily dependent upon refactoring, unit tests are an integral component.[17]

Unit testing frameworks[edit]

Unit testing frameworks are most often third-party products that are not distributed as part of the compiler suite. They help simplify the process of unit testing, having been developed for a wide variety of languages.

It is generally possible to perform unit testing without the support of a specific framework by writing client code that exercises the units under test and uses assertions, exception handling, or other control flow mechanisms to signal failure. Unit testing without a framework is valuable in that there is a barrier to entry for the adoption of unit testing; having scant unit tests is hardly better than having none at all, whereas once a framework is in place, adding unit tests becomes relatively easy.[18] In some frameworks many advanced unit test features are missing or must be hand-coded.

Language-level unit testing support[edit]

Some programming languages directly support unit testing. Their grammar allows the direct declaration of unit tests without importing a library (whether third party or standard). Additionally, the boolean conditions of the unit tests can be expressed in the same syntax as boolean expressions used in non-unit test code, such as what is used for if and while statements.

Languages with built-in unit testing support include:

Some languages without built-in unit-testing support have very good unit testing libraries/frameworks. Those languages include:

See also[edit]

References[edit]

^ a b Kolawa, Adam; Huizinga, Dorota (2007). Automated Defect Prevention: Best Practices in Software Management. Wiley-IEEE Computer Society Press. p. 75. ISBN 978-0-470-04212-0.

^ a b Hamill, Paul (2004). Unit Test Frameworks: Tools for High-Quality Software Development. "O''Reilly Media, Inc.". ISBN 9780596552817. Retrieved 3 June 2019.

^ Xie, Tao. "Towards a Framework for Differential Unit Testing of Object-Oriented Programs" (PDF). Retrieved 23 July 2012.

^ Fowler, Martin (2 January 2007). "Mocks aren''t Stubs". Retrieved 1 April 2008.

^ "Getting Started with xUnit.net (desktop)".

^ "Theories".

^ "Parameterized tests".

^ Boehm, Barry W.; Papaccio, Philip N. (October 1988). "Understanding and Controlling Software Costs" (PDF). IEEE Transactions on Software Engineering. 14 (10): 1462–1477. doi:10.1109/32.6191. Retrieved 13 May 2016.

^ "Test Early and Often". Microsoft.

^ "Prove It Works: Using the Unit Test Framework for Software Testing and Validation". National Instruments. 21 August 2017.

^ Cramblitt, Bob (20 September 2007). "Alberto Savoia sings the praises of software testing". Retrieved 29 November 2007.

^ Brooks, Frederick J. (1995) [1975]. The Mythical Man-Month. Addison-Wesley. p. 64. ISBN 978-0-201-83595-3.

^ Kolawa, Adam (1 July 2009). "Unit Testing Best Practices". Retrieved 23 July 2012.

^ daVeiga, Nada (6 February 2008). "Change Code Without Fear: Utilize a regression safety net". Retrieved 8 February 2008.

^ Kucharski, Marek (23 November 2011). "Making Unit Testing Practical for Embedded Development". Retrieved 8 May 2012.

^ http://wiki.c2.com/?UnitTestsAndDatabases

^ "Agile Emergent Design". Agile Sherpa. 3 August 2010. Archived from the original on 22 March 2012. Retrieved 8 May 2012.

^ Bullseye Testing Technology (2006–2008). "Intermediate Coverage Goals". Retrieved 24 March 2009.

^ "Crystal Spec". crystal-lang.org. Retrieved 18 September 2017.

^ "Unit Tests - D Programming Language". D Programming Language. D Language Foundation. Retrieved 5 August 2017.

^ "testing - The Go Programming Language". golang.org. Retrieved 3 December 2013.

^ Python Documentation (2016). "unittest -- Unit testing framework". Retrieved 18 April 2016.

^ Welsh, Noel; Culpepper, Ryan. "RackUnit: Unit Testing". PLT Design Inc. Retrieved 26 February 2019.

^ Welsh, Noel; Culpepper, Ryan. "RackUnit Unit Testing package part of Racket main distribution". PLT Design Inc. Retrieved 26 February 2019.

^ "Minitest (Ruby 2.0)". Ruby-Doc.org.

^ The Rust Project Developers (2011–2014). "The Rust Testing Guide (Rust 0.12.0-pre-nightly)". Retrieved 12 August 2014.

^ Sierra, Stuart. "API for clojure.test - Clojure v1.6 (stable)". Retrieved 11 February 2015.


^ "Pester Framework". Retrieved 28 January 2016.

External links[edit]
Test Driven Development (Ward Cunningham''s Wiki)
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (6, 3, 0, N'm1rko

сегодня в 13:41

Усложнение команд консоли, 1979−2020

Перевод

Моё хобби — открыть «Философию UNIX» Макилроя на одном мониторе, одновременно читая маны на другом.

Первый из принципов Макилроя часто перефразируют как «Делайте что-то одно, но делайте хорошо». Это сокращение от его слов «Создавайте программы, которые делают одну вещь хорошо. Для новой работы создавайте новые программы, а не усложняйте старые добавлением новые "функций"».

Макилрой приводит пример:
Для посторонних кажется удивительным тот факт, что компиляторы UNIX не выдают листинги: печать лучше осуществляется и более гибко настраивается с помощью отдельной программы.

Если вы откроете справку для ls, то она начинается с

ls [-ABCFGHLOPRSTUW@abcdefghiklmnopqrstuwx1] [file ...]

То есть однобуквенные флаги для ls включают все строчные буквы, кроме {jvyz}, 14-ти прописных букв, @ и 1. Это 22 + 14 + 2 = 38 только односимвольных вариантов.

Читать дальше →

ru_vds

сегодня в 15:30

[ В закладки ] CSS: использование внутренних и внешних отступов

Перевод

Если несколько элементов веб-страницы расположены близко друг к другу, то у пользователей возникает такое ощущение, что у этих элементов есть что-то общее. Группировка элементов помогает пользователю понять их взаимосвязь благодаря оценке расстояния между ними. Если бы все элементы были бы расположены на одинаковом расстоянии друг от друга, пользователю сложно было бы, просматривая страницу, узнать о том, какие из них связаны друг с другом, а какие — нет.

Эта статья посвящена всему, что нужно знать о настройке расстояний между элементами и о настройке внутренних пространств элементов. В частности, речь пойдёт о том, в каких ситуациях стоит использовать внутренние отступы (padding), а в каких — внешние (margin).

Читать дальше →




m1rko

сегодня в 18:46

Реверс-инжиниринг антиблокировщика рекламы BlockAdBlock

Перевод

Если вы пользуетесь блокировщиками рекламы, то могли встречать BlockAdBlock. Этот скрипт обнаруживает ваш блокировщик и не пускает на сайт, пока вы его не отключите. Но мне стало интересно, как он работает. Как антиблокировщик обнаруживает блокировщики? А как на это реагируют блокировщики и как они блокируют антиблокировщики?

Первым делом я взглянул на их сайт. BlockAdBlock предлагает конфигуратор с настройками: интервал ожидания и как будет выглядеть предупреждение, генерируя разные версии скрипта.

Это натолкнуло меня на мысль о версиях. А что, если мог посмотреть не на одну версию, а на все сразу? Так я и сделал. Я вернулся назад во времени с помощью Wayback Machine. После этого скачал все версии BlockAdBlock и хэшировал их.

Читать дальше →




MirAleAnu

сегодня в 14:31

Маленькие задачи по физике

Приведу несколько задач, в основном из физики. Мне они нравятся. Надеюсь они понравятся и Вам.

Забудем о черных дырах, темной энергии и материи; забудем о коте Шредингера, большом взрыве и эволюции Вселенной; забудем о струнах и суперструнах; и даже о фракталах забудем. В этих темах, как и в политике, большинство считает себя возможным высказаться. И высказываются. И много говорилось и говорится дельного, а еще больше говорилось и говорится путаницы и просто нелепицы. Каюсь, к этому и я приложил руку. А давайте вернемся к простоте классической физики и к ее понятным задачам. Иногда полезно спуститься с небес на землю.

Для большинства задач я не привожу решения. Самое полезное – найти самому решение. Конечно, задачи не для профессионального физика, исключая задачу о ленте и о пушке.

Большинство задач, так или иначе, обсуждалось в Internete. Но время идет и приходят новые поколения и, может быть, для них задачи будут в новинку.

Читать дальше →






guseynchick

сегодня в 13:45

Расширенный HTML

В этой статье хотел бы рассказать немного про библиотеку, первую версию которой я создал еще в конце прошлого года. Суть очень простая — расширить возможности языка HTML, чтобы можно было без JavaScript''а писать простые и рутинные вещи: отправка формы в json формате, загрузка HTML тимплейтов на определенную страницу(по сути модульная система для HTML через http/s запросы), турболинки(привет пользователям RoR), простая шаблонизация на основе ответов ajax запросов и немного еще.

Библиотека называется EHTML или Extended HTML. Основана она на небезызвестной идее веб компонентов. Она доступна на гитхабе, там довольно таки структурированная документация с примерами. В этой статье я просто опишу основные идеи, возможно кому-то это зайдет.

Читать дальше →




TranE91

сегодня в 00:37

ПШЕ AndroidStudio или как использовать VCS Tools по полной

Tutorial

- Все хорошо, только перед влитием обязательно засквошь коммиты.
- Заскво...Что?

Примерно такая реакция была у меня после получения апрува первого пул реквеста на первой неделе работы в одной крупной компании. Причина такой реакции весьма простая — далеко не каждый заказчик/работодатель может себе позволить такую роскошь как большая команда на одну платформу, в особенности это касается мобильной разработки. Из-за ненадобности и возможности быстрой коммуникации в своем мирке, далеко не все вещи, которые используют крупные мастера своего дела, обретают актуальность в небольших командах. Говоря проще — а на кой мне это надо, если мы и так хорошо без этого жили и хорошо справлялись?
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (7, 3, 1, N'
После перехода в новую компанию я столкнулся с той же проблемой, но уже по другую сторону баррикад. Если вы еще не догадались о чем пойдет речь дальше — это GIT, говоря точнее, его встроенный инструментарий в AndroidStudio и как он позволяет сделать нашу работу проще.

Я постараюсь не обращать внимания на банальные вещи: init VCS; new/rename/push branch; rebase/merge onto branch; setup remotes e.t.c. Я постараюсь обратить внимание на те элементы, которые по боязни своего незнания, я долгое время избегал(и жалею).

Читать дальше →




HabraZ0

сегодня в 17:31

Легкий способ защитить свой Mikrotik от атак

Хочу поделиться с сообществом простым и рабочим способом, как при помощи Mikrotik защитить свою сеть и «выглядывающие» из-за него сервисы от внешних атак. А именно всего четырьмя правилами организовать на Микротике honeypot.

Итак, представим, что у нас небольшой офис, внешний IP за которым стоит RDP сервер, для работы сотрудников по удаленке. Первое правило это конечно сменить порт 3389 на внешнем интерфейсе на другой. Но это не на долго, спустя пару дней журнал аудита терминального сервера начнет показывать по несколько неудачных авторизаций в секунду от неизвестных клиентов.

Другая ситуация, у Вас за Mikrotik спрятан asterisk, естественно не на 5060 udp порту, и через пару дней также начинается перебор паролей… да да, знаю, fail2ban наше вcё, но над ним еще попыхтеть придется… вот я например недавно поднял его на ubuntu 18.04 и с удивлением обнаружил, что из коробки fail2ban не содержит актуальных настроек для asterisk из той-же коробки того-же ubuntu дистрибутива… а гуглить быстрые настройки готовых «рецептов» уже не получается, цифры у релизов с годами растут, а статьи с «рецептами» для старых версий уже не работают, а новых почти не появляется… Но что-то я отвлекся…

Читать дальше →






Audioman

вчера в 21:43

Кто занимается дипфейк-аудио и зачем это нужно

С начала года появилось сразу несколько новых систем ИИ, способных синтезировать видеозапись с говорящим человеком на основе аудио. Расскажем, кто и с какой целью занимается подобными разработками. Также поговорим о других инструментах, позволяющих редактировать аудиозаписи.




Читать дальше →




Programmer1234

сегодня в 19:26

Российский SCRUM. Бессмысленный и беспощадный

Доброе время суток, уважаемый Хабр!

Я программист «старой школы», с опытом работы более 20 лет. Участвовал в разработке многих проектов, большая часть из которых довольно известные и успешные. В некоторых проектах занимал руководящие должности, достиг неплохого уровня зарплаты. Но ведь мы собрались здесь не для того, чтобы помериться стажем, опытом, зарплатой и т.д., верно? Поговорим лучше о том, как стартапы используют современные методы управления разработкой программного обеспечения. И что из этого получается.

Читать дальше →




kokunovartem

сегодня в 11:28

Эффективная система аттестаций, которая позволит стать лучшим руководителем

Перевод

В прошлом, когда я был молодым менеджером продукта, один из моих руководителей в корне изменил мою карьеру. Шел второй год моей работы в Airbnb. Я справлялся со своими обязанностями, но не более. Мой новый руководитель Влад Локтев обратил внимание, что завершение проекта, который я вел, откладывалось на недели. Он не был удивлен, помог вернуть проект в нужное русло и завершить его. Но я знал, что на очередной аттестации мне припомнят эту ошибку. Когда пришло время, я действительно получил далеко не блестящие оценки. Влад обозначил мои точки роста, в том числе рекомендовал сфокусироваться на коммуникациях, направленных на контроль статусов задач, и их жесткой приоритизации. После этого разговора я мог бы выйти подавленным. Но вместо этого, наоборот, я почувствовал небывалый эмоциональный подъем и желание действовать – мне стало ясно, что нужно делать.

Читать дальше →




Malikspace

вчера в 21:19

Как организована работа в Amazon

Как и во многих других американских компаниях, организация рабочих процессов в Amazon построена на базовых принципах, основная цель которых – помочь сотрудникам принять правильное решение, основываясь на ценностях компании. Мы поговорили с продукт-менеджером в Amazon, который рассказал о том, каким принципам следуют в компании, как они помогают при выполнении задач, и через какие процессы проходит команда при разработке нового продукта. Ниже мы оставили ссылку на видео с полным интервью.
Миссия, видение и принципы Amazon

В моем понимании, миссия Amazon – быть самой клиентоориентированной компанией в мире. Все продукты, над которыми работает корпорация, разрабатываются с целью сначала сделать продукт для клиента, а потом уже увеличить его продажи.

Есть 14 принципов, которыми живет компания, и они используются во всех рабочих процессах. Эти принципы достаточно базовые, в них нет ничего особенного. Ими руководствуются при запуске нового продукта, во время собеседований, или когда даешь обратную связь коллеге. Их не заставляют заучивать, но когда ты работаешь в компании, хочешь-не хочешь начинаешь следовать этим принципам.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (8, 3, 2, N'
Читать дальше →




achekalin

сегодня в 16:19

#BeeFree, или… Билайн — ты не смог удержаться?

Кажется, не так давно гремели дебаты вокруг того, что маркетологи иных сотовых компаний (не будем перечислять их по именам, а цвета лого и так все знают) обнаружили, что показываемое пользователю название сотовой сети можно изменить (и им за это ничего не будет). Тогда я ещё порадовался, что Билайн — молодцы, до сих пор вроде бы не опускались до столь дешёвых трюков. Как говорится, блажен, кто верует!

«Не осилили» — именно это я подумал сегодня, когда случайно в «шторке» телефона обнаружил на месте имени сети «Beeline» невнятное «#beefree».


реклама - она такая!




RostislavDugin

сегодня в 04:14

The Clear Architecture на примере TypeScript и React

Добрый день, уважаемые читатели. В этой статье мы поговорим об архитектуре программного обеспечения в веб-разработке. Довольно долгое время я и мои коллеги используем вариацию The Clean Architecture для построения архитектуры в своих проектах Frontend проектах. Изначально я взял ее на вооружение с переходом на TypeScript, так как не нашел других подходящих общепринятых архитектурных подходов в мире разработки на React (а пришел я из Android-разработки, где давным-давно, еще до Kotlin, наделала шумихи статья от Fernando Cejas, на которую я до сих пор иногда ссылаюсь).

В данной статье я хочу рассказать вам о нашем опыте применения The Clean Architecture в React-приложениях с использованием TypeScript. Зачем я это рассказываю? — Иногда мне приходится разъяснять и обосновывать ее использование разработчикам, которые еще не знакомы с таким подходом. Поэтому здесь я сделаю детальный разбор с наглядными пояснениями на которое я смогу ссылаться в будущем.

Читать дальше →








BinCat

сегодня в 14:50

Экспорт плана нумерации Федерального Агентства Связи в реляционную БД

Федеральное Агентство Связи регулярно обновляет размещённый в открытом доступе план нумерации. Если вы используете этот план для определения региона или провайдера абонента в своём диалплане, то скорее всего заинтересованы в актуальности этой информации. На первый взгляд нет ничего сложного в том, чтобы написать приложение, которое загрузит, обработает и отправит данные в БД, однако приступив к реализации, вы неизбежно наткнётесь на подводные камни, о которых я сейчас расскажу.


Читать дальше →




alinatestova

сегодня в 16:44

Как наладить работу в постоянно меняющихся условиях: бесплатные антикризисные курсы «Актиона»

В конце марта наш медиахолдинг подготовил специальные антикризисные курсы о том, как организовать работу бизнеса в новых условиях. Мы разработали программы для руководителей компаний, бухгалтеров, юристов, кадровиков и врачей — они помогают разобраться в правовых аспектах сложившейся ситуации и адаптироваться к ней. Курсы рассчитаны на самые разные компании — от микробизнеса до предприятий госсектора и ТСЖ. Пройти их можно удаленно и бесплатно.

О «фишках» курсов, их подготовке, а также подробнее о курсе для руководителей бизнеса рассказывают директор образовательного бизнеса «Актион-МЦФЭР» Дмитрий Зацепин и Главный редактор Школы Генерального директора «Актион» Антонина Павлова.


Читать дальше →




sabirovilya

сегодня в 15:59

4 шага к увеличению LTV продукта через коммуникации c пользователем

Расскажу на примере Timepad (сервиса для организаторов ивентов), который зарабатывает комиссии с продаж билетов и апсейле. Вместо некоторых цифр, которые нельзя найти с помощью внешних источников, будут X, Y и Z, как-никак NDA :)

Но прежде ключевая информация для понимания: Product Management воздействуют на показатели через разработку, а Product Marketing (а я продуктовый маркетолог) — через коммуникации с пользователями.
Шаг 1: составить пирамиду метрик


Читать дальше →




Retronaut

сегодня в 15:36

Сердце разработчика: девкиты Sony PlayStation 1

Отец платформы, Кен Кутараги, проектировал PlayStation не просто как ответ на неуместные действия со стороны Nintendo, он стремился создать эталонный продукт, на который будут ориентироваться все геймдевы поколения. Учитывая опыт и ошибки ближайших конкурентов, Кутараги создал максимально дружественную разработчику систему, снискавшую в итоге феноменальный успех у геймеров девяностых. И причиной этому, с одной стороны, послужили наработки по Nintendo Play Station, с другой стороны, определённый опыт в производстве семейства компьютеров Sony Network Engineering Workstation (NEWS), но главное, неуёмное желание Кутараги создать консоль своей мечты.

Логично, что первые прототипы PlayStation под названием MW.2 (ревизия DTL-H500), а затем и доработанный MW.3 (ревизия DTL-H505) получили полностью совместимое с Sony NEWS железо, тем самым, открывая возможность к разработке первых игр для ещё проектируемой консоли. В принципе, подобная практика была не нова и активно использовалась при создании FM Towns Marty и Amiga CD32 силами Fujitsu и Commodore соответственно. MW.3 по факту был и девкитом, и самой консолью. А дабы на первых порах не пришлось мудрить с экстерьером, его полностью заимствовали у Sony NWS-3710.


Читать дальше →




Kouki_RUS

сегодня в 13:01

Ваш первый BERT: иллюстрированное руководство

ПереводTutorial

Прогресс в области машинного обучения для обработки естественного языка существенно ускорился за последние несколько лет. Модели покинули исследовательские лаборатории и стали основой ведущих цифровых продуктов. Хорошей иллюстрацией этому служит недавнее заявление о том, что основным компонентом, стоящим за поиском Google, стала модель BERT. Google верит, что этот шаг (т.е. внедрение передовой модели понимания естественного языка в поисковую систему) представляет собой «величайший прорыв за последние пять лет и один из знаменательнейших во всей истории поисковых систем».

Данная статья – это простое руководство по использованию одной из версий BERT''а для классификации предложений. Пример, рассмотренный нами, одновременно и достаточно простой для первого знакомства с моделью, и достаточно продвинутый для того, чтобы продемонстрировать ключевые концепты.

Помимо этой статьи был подготовлен ноутбук, который можно посмотреть в репозитории или запустить в Colab.

Читать дальше →




averin61

сегодня в 08:44

Филина грамота

Сегодня мы продолжаем тему управляемого обучения публикуем интервью с обученным ботом.
Привет

Здравствуйте. Я робот Филя. Могу беседовать на темы определенного круга.
На какие темы ты можешь беседовать?

Круг тем для обсуждения: эволюция человека — чем отличается от животных — искусственный интеллект, смена экономических формаций капитализм — коммунизм, пропаганда и фейки, религия, смысл жизни.


Читать дальше →




bravik

сегодня в 05:02

Как переиспользовать код с бандлами Symfony 5? Часть 4. Расширение бандла в хосте

Поговорим о том, как прекратить копипастить между проектами и вынести код в переиспользуемый подключаемый бандл Symfony 5. Серия статей, обобщающих мой опыт работы с бандлами, проведет на практике от создания минимального бандла и рефакторинга демо-приложения, до тестов и релизного цикла бандла.

Проектируя бандл, надо думать, что должно быть инкапсулировано внутри него, а что — доступно для пользователя. Должен ли бандл иметь фиксированную функциональность или быть гибким и позволять себя расширять? Если требуется гибкость, то нужно предусмотреть какие-то точки-интеграции для расширения бандла, его интерфейс.

Попробуем предусмотреть такие точки в нашем демо-приложении. В этой статье:

Подключение пользовательской логики к бандлу
Работа с тегами
Compiler Pass
Автоконфигурация сервисов

Читать дальше →
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (9, 4, 0, N'
Ошибка загрузки настроек.

Для доступа ко всем настройкам введите логин и пароль на Яндексе или зарегистрируйтесь.

Мои источники

Добавьте издания, материалы которых вы хотите видеть в первую очередь:

Нежелательные источники

Вы будете реже видеть материалы изданий из этого списка:
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10, 5, 0, N'10 горячих клавиш VS Code, которые ускорят вашу работу
Горячие клавиши — добро, польза и экономия времени. Ребята из HTML Academy рассказали, какие комбинации использовать, чтобы упростить работу с Visual Studio Code.

Вы узнаете, как:

быстро добавить комментарий;
перейти к строке под номером;
поменять строку местами с соседними;
дублировать строку;
перейти к парной сборке;
переименовать переменную;
отформатировать документ;
перейти к объявлению переменной;
включить/выключить перенос слов;
включить дзен-режим.

Подробнее о каждой комбинации читайте в этой статье.

Введение в Chrome DevTools. Панель Elements
В каждый современный браузер встроены инструменты разработчика — они позволяют быстро отловить и исправить ошибки в разметке или в коде. С их помощью можно узнать, как построилось DOM-дерево, какие теги и атрибуты есть на странице, почему не подгрузились шрифты и многое другое.

Подробно о настройке Chrome DevTools и панеле Elements читайте в статье от HTML Academy.

Интерактивная SVG-диаграмма
Сейчас все активно обсуждают радости и гадости удалённой работы. Тем временем ребята из HTML Academy провели небольшой опрос и попросили пользователей рассказать, с какими трудностями они сталкиваются во время работы из дома (спойлер — все ленятся).

Статистику нужно было как-то красиво оформить, поэтому они подготовили туториал о том, как сделать интерактивный SVG-график. Очень подробная инструкция по ссылке.

Как написать и запустить HTML на компьютере?
Чтобы стать профессиональным разработчиком, нужно уметь пользоваться инструментами. Сегодня попробуем разобраться, как и в чём написать и запустить HTML-код на своём компьютере.

Действуем по плану:

устанавливаем и настраиваем редактор кода;
создаем рабочую папку и добавляем туда файлы вашего проекта;
редактируем и сохраняем файлы с кодом;
устанавливаем расширение, чтобы сразу видеть результат верстки.

Подробно о каждом шаге читайте в статье от HTML Academy.

Одно письмо в неделю с полезными статьями о веб-разработке
Ребята из HTML Academy запустили редакторскую рассылку. Теперь у вас есть возможность тратить дополнительные 10-15 минут в неделю на самообразование. Здорово же!

В чем суть?

Каждую неделю вы будете получать одно письмо со статьями.

О чем статьи?

О том, как верстать, пользоваться инструментами, учиться и строить карьеру в IT. Будет много интересного для начинающих веб-разработчиков.

И все?

Конечно, нет! Каждую неделю все подписчики будут получать приятный бонус.

Подписаться на рассылку можно по тут.

HTML и CSS на примерах
Обновил мобильное приложение под Android в виде книжки для изучения HTML и CSS — HTML и CSS на примерах.

Теперь по всем темам сделаны интерактивные задания для тренировки и закрепления навыков (почти 150 штук). Задания простые и с автоматической проверкой.

Устанавливайте сами и советуйте друзьям.
Как сменить профессию на карантине?
Реклама

Пока весь мир обсуждает коронавирус, кризис и отсутствие туалетной бумаги на прилавках, ребята из HTML Academy предлагают не паниковать, но задуматься о своем будущим.

Несмотря на то, что многие сотрудники могут оказаться в очереди на увольнение, айтишники, скорее всего, не попадут под удар. А хорошие айтишники, которые приносят деньги компании, — тем более.

В любом случае лучшее решение в сложившейся ситуации — попытаться увидеть возможности. Например, получить новую профессию.

В этой статье вы узнаете, какие отрасли пострадают больше других, и чем можно (и нужно!) заниматься на заре новой карьеры.

Справочник HTML
Большой популярностью пользуется справочник HTML, поэтому обновил локальную версию, которая не требует подключения к Интернету и пользоваться ей можно даже в деревне. Был бы компьютер только.

Все примеры со всеми картинками теперь можно смотреть сразу в браузере!

Вот форма сразу для оплаты. Справочник стоит 59 рублей; как обычно, считайте это поддержкой сайта, чтобы в будущем были новые учебные курсы и руководства. Без вашей поддержки ничего этого не будет.

Кто уже приобретал, обновление должно прийти автоматом, лишний раз платить ничего не нужно.

Переход на Vue
Для webref.ru сделал рефакторинг кода, т.е. переделал внутреннюю часть без изменения дизайна. Отказался полностью от jQuery и перешёл на Vue. Основные изменения коснулись примеров — теперь это не просто код, а небольшие приложения. Во-первых, результат примера виден сразу, во-вторых, код можно открыть в популярных песочницах JSFiddle и CodePen. А также есть переключатель светлой/тёмной темы, копирование кода в буфер и обновление результата. Учтите, что эффект обновления заметен только для примеров с анимацией.

Можно перейти к одному из примеров и посмотреть всё самому.

Как не устроиться на работу фронтенд-разработчиком
Есть несколько причин, по которым программист не может найти работу — плохо разбирается в технологиях, не знает свой язык достаточно хорошо или вообще не программист (так тоже бывает). Но даже если все теги отскакивают от зубов, а нюансы языка ясны, можно засыпаться на более простых моментах.


О том, как провалиться на этапе тестового задания, на собеседовании или при выходе на работу — в статье от HTML Academy.

Как получать 100 тысяч в месяц за код. 8 шагов к шикарной карьере
Список того, что нужно знать фронтендеру — как фильмы из киновселенной Марвел. Непонятно, в каком порядке их смотреть, чтобы во всём разобраться, кто все эти люди, и что будет в итоге.

Наконец-то появился понятный список того, в каком порядке изучать и практиковаться в вёрстке и коде, чтобы получить новую работу за кучу денег.

Первые четыре шага такие.

Понять основы HTML и CSS по книжкам, ютюбу или онлайн-тренажёрам.
Вникнуть в тонкости вёрстки (и завести себе Гитхаб).
Пройти курсы по вёрстке и/или фронтенд-разработке и разобраться в нужных инструментах.
Найти наставника и показывать ему свой код.

Ещё 4 шага — в этой статье. Добавьте в закладки, скажете спасибо через год.

Ошибки, которые испортят ваше резюме
Порой у начинающий специалистов не получается найти работу не потому, что их плохо учили, а потому что не могут пройти собеседование или попасть на него.

Серёжа Попов, руководитель фронтенд-аутсорса «Лига А.», где трудятся выпускники HTML Academy, рассказал, какие 17 ошибок нельзя делать при составлении резюме и на других этапах поиска работы:

Указывать несколько должностей в одном резюме
Не загружать фотографию
Загрузить фотографию, где есть не только вы
Скрывать информацию в резюме
Писать «хочу переехать», но откликаться на вакансии в своём городе
Скрывать свой регион до собеседования
Отсутствие ключевых навыков
Нерелевантный опыт
Не написать, какие задачи решали на прошлых местах работы
Не писать (почти) ничего в разделе «О себе»
Не указывать желаемую зарплату
Зарплата, не соответствующая уровню рынка или позиции
Не написать сопроводительное письмо
Сопроводительное письмо в стиле «Прошу рассмотреть меня на вакансию»
Сопроводительное письмо из одного слова
Написать «Я не готов к тестовому заданию»
Написать одно сопроводительное письмо для десяти компаний

Подробнее, об ошибках и том как их правильно исправить читайте в статье, кстати её вторая часть на подходе.

Как сделать счётчик лайков на JavaScript
Если хотите узнать, как сделать счётчик лайков и систему комментирования, загляните в новую главу интерактивного курса «Знакомство с JavaScript» — «Условия и создание элементов».

В ней вы будете работать с новостным сайтом, познакомитесь с условиями, научитесь создавать и добавлять на страницу новые элементы. В конце вас ждёт испытание: нужно будет запрограммировать список дел.

Кажется слишком сложным? Сохраняйте спокойствие! Под чутким руководством инструктора Кекса сложное превращается в простое и понятное. Вперёд к знаниям!

Прокачайтесь в Акселераторе от HTML Academy
Реклама

Если вам не хватает проектов в портфолио, навыков работы в команде и профессиональной оценки вашего кода, тогда «Акселератор» — то, что вам нужно.

Что вас ждёт в «Акселераторе»:

проекты коммерческого уровня в команде с менеджером и тестировщиком;
личный наставник проверит качество кода и расскажет, как сделать лучше;
в конце мы сравним ваши показатели с эталонными и расскажем, что можно улучшить.

Если вы не можете оценить уровень ваших навыков — мы подберём вам макет.

До конца января проекты «Акселератора» можно купить со скидкой 20%

Канал на YouTube
Создал канал на YouTube, где буду выкладывать разные ролики по теме веб-технологий. Вот пример небольшого видео-курса по стилевым селекторам.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10006, 10003, 0, N'HTML

Расширение

.html или .htm

MIME-тип

text/html[1]

Разработчик

Консорциум Всемирной паутины, Инженерный совет Интернета и WHATWG

Опубликован

1992

Тип формата

Язык разметки

Расширен из

SGML[2]

Стандарт(ы)

W3C HTML 5.2
WHATWG HTML Living Standard

Сайт

w3.org/html/
html.spec.whatwg.org/mul…
w3c.github.io/html/

HTML

HTML и HTML5
Динамический HTML
XHTML
XHTML Mobile Profile и CHTML
Document Object Model
Семейство шрифтов
Кодировки символов
Мнемоники в HTML
Редактор HTML
Элементы HTML
Семантическая вёрстка
Карта изображений
Цвета HTML
Формы HTML
Фреймы HTML
HTML5 audio и HTML5 video
Canvas
Скрипты в HTML
Unicode и HTML
Браузерный движок
Quirks mode
Каскадные таблицы стилей
W3C и WHATWG
Web Storage
WebGL
Сравнение
языков разметки документов
браузеров
браузерных движков

HTML (от англ. HyperText Markup Language — «язык гипертекстовой разметки») — стандартизированный язык разметки документов во Всемирной паутине. Большинство веб-страниц содержат описание разметки на языке HTML (или XHTML). Язык HTML интерпретируется браузерами; полученный в результате интерпретации форматированный текст отображается на экране монитора компьютера или мобильного устройства.

Язык HTML до 5-й версии определялся как приложение SGML (стандартного обобщённого языка разметки по стандарту ISO 8879). Спецификации HTML5 формулируются в терминах DOM (объектной модели документа).

Язык XHTML является более строгим вариантом HTML, он следует синтаксису XML и является приложением языка XML в области разметки гипертекста.

Во всемирной паутине HTML-страницы, как правило, передаются браузерам от сервера по протоколам HTTP или HTTPS, в виде простого текста или с использованием шифрования.

Общее представление[править | править код]

Язык гипертекстовой разметки HTML был разработан британским учёным Тимом Бернерсом-Ли приблизительно в 1986—1991 годах в стенах ЦЕРНа в Женеве в Швейцарии[3]. HTML создавался как язык для обмена научной и технической документацией, пригодный для использования людьми, не являющимися специалистами в области вёрстки. HTML успешно справлялся с проблемой сложности SGML путём определения небольшого набора структурных и семантических элементов — дескрипторов. Дескрипторы также часто называют «тегами». С помощью HTML можно легко создать относительно простой, но красиво оформленный документ. Помимо упрощения структуры документа, в HTML внесена поддержка гипертекста. Мультимедийные возможности были добавлены позже.

Первым общедоступным описанием HTML был документ «Теги HTML», впервые упомянутый в Интернете Тимом Бернерсом-Ли в конце 1991 года,[4][5]. В нём описываются 18 элементов, составляющих первоначальный, относительно простой дизайн HTML. За исключением тега гиперссылки, на них сильно повлиял SGMLguid, внутренний формат документации, основанный на стандартном обобщенном языке разметки (SGML), в CERN. Одиннадцать из этих элементов всё ещё существуют в HTML 4[6].

Изначально язык HTML был задуман и создан как средство структурирования и форматирования документов без их привязки к средствам воспроизведения (отображения). В идеале, текст с разметкой HTML должен был без стилистических и структурных искажений воспроизводиться на оборудовании с различной технической оснащённостью (цветной экран современного компьютера, монохромный экран органайзера, ограниченный по размерам экран мобильного телефона или устройства и программы голосового воспроизведения текстов). Однако современное применение HTML очень далеко от его изначальной задачи. Например, тег <table> предназначен для создания в документах таблиц, но иногда используется и для оформления размещения элементов на странице. С течением времени основная идея платформонезависимости языка HTML была принесена в жертву современным потребностям в мультимедийном и графическом оформлении.

Браузеры[править | править код]

Текстовые документы, содержащие разметку на языке HTML (такие документы традиционно имеют расширение .html или .htm), обрабатываются специальными приложениями, которые отображают документ в его форматированном виде. Такие приложения, называемые «браузерами» или «интернет-обозревателями», обычно предоставляют пользователю удобный интерфейс для запроса веб-страниц, их просмотра (и вывода на иные внешние устройства) и, при необходимости, отправки введённых пользователем данных на сервер. Наиболее популярными на сегодняшний день браузерами являются Google Chrome, Mozilla Firefox, Opera, Internet Explorer и Safari (см.: Браузер#Рыночные доли).

Версии[править | править код]
HTML 2.0 — опубликован IETF как RFC 1866 в статусе Proposed Standard (24 ноября 1995 года)[7];
HTML 3.0 — 28 марта 1995 года — IETF Internet Draft (до 28 сентября 1995 года);
HTML 3.2[8] — 14 января 1997 года;
HTML 4.0[9] — 18 декабря 1997 года;
HTML 4.01[10] — 24 декабря 1999 года;
ISO/IEC 15445:2000[11] (так называемый ISO HTML, основан на HTML 4.01 Strict) — 15 мая 2000 года;
HTML5[12] — 28 октября 2014 года[13];
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10007, 10003, 1, N'HTML 5.1 начал разрабатываться 17 декабря 2012 года[14][15]. Рекомендован к применению с 1 ноября 2016 года[16][17][18][19].
HTML 5.2 был представлен 14 декабря 2017 года[20][21][22].
HTML 5.3 был представлен 24 декабря 2018 года.

Официальной спецификации HTML 1.0 не существует. До 1995 года существовало множество неофициальных стандартов HTML. Чтобы стандартная версия отличалась от них, ей сразу присвоили второй номер.

Версия 3 была предложена Консорциумом Всемирной паутины (W3C) в марте 1995 года и обеспечивала много новых возможностей, таких как создание таблиц, «обтекание» изображений текстом и отображение сложных математических формул, поддержка gif формата. Даже при том, что этот стандарт был совместим со второй версией, реализация его была сложна для браузеров того времени. Версия 3.1 официально никогда не предлагалась, и следующей версией стандарта HTML стала 3.2, в которой были опущены многие нововведения версии 3.0, но добавлены нестандартные элементы, поддерживаемые браузерами Netscape Navigator и Mosaic.

В версии HTML 4.0 произошла некоторая «очистка» стандарта. Многие элементы были отмечены как устаревшие и не рекомендованные (англ. deprecated). В частности, тег <font>, используемый для изменения свойств шрифта, был помечен как устаревший (вместо него рекомендуется использовать таблицы стилей CSS).

В 1998 году Консорциум Всемирной паутины начал работу над новым языком разметки, основанным на HTML 4, но соответствующим синтаксису XML. Впоследствии новый язык получил название XHTML. Первая версия XHTML 1.0 одобрена в качестве Рекомендации консорциума Всемирной паутины 26 января 2000 года.

Планируемая версия XHTML 2.0 должна была разорвать совместимость со старыми версиями HTML и XHTML, но 2 июля 2009 года Консорциум Всемирной паутины объявил, что полномочия рабочей группы XHTML2 истекают в конце 2009 года. Таким образом, была приостановлена вся дальнейшая разработка стандарта XHTML 2.0[23].

Перспективы[править | править код]

В настоящее время Консорциум Всемирной паутины разработал HTML версии 5. Черновой вариант спецификации языка появился в Интернете 20 ноября 2007 года.

Сообществом WHATWG (англ. Web Hypertext Application Technology Working Group), начиная с 2004 года[24], разрабатывается спецификация Web Applications 1.0, часто неофициально называемая «HTML 5», которая расширяет HTML (впрочем, имея и совместимый с XHTML 1.0 XML-синтаксис) для лучшего представления семантики различных типичных страниц, например форумов, сайтов аукционов, поисковых систем, онлайн-магазинов и т. д., которые не очень удачно вписываются в модель XHTML 2.0.

Структура HTML-документа[править | править код]

HTML — теговый язык разметки документов. Любой документ на языке HTML представляет собой набор элементов, причём начало и конец каждого элемента обозначается специальными пометками — тегами. Элементы могут быть пустыми, то есть не содержащими никакого текста и других данных. В этом случае обычно не указывается закрывающий тег (например, тег переноса строки <br> — одиночный и закрывать его не нужно) . Кроме того, элементы могут иметь атрибуты, определяющие какие-либо их свойства (например, атрибут href=" у ссылки). Атрибуты указываются в открывающем теге. Вот примеры фрагментов HTML-документа:

<strong>Текст между двумя тегами — открывающим и закрывающим.</strong>
<a href="http://www.example.com">Здесь элемент содержит атрибут href, то есть гиперссылку.</a>
А вот пример пустого элемента: <br>

Регистр, в котором набрано имя элемента и имена атрибутов, в HTML значения не имеет (в отличие от XHTML). Элементы могут быть вложенными. Например, следующий код:

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8" />
<title>HTML Document</title>
</head>
<body>
<p>
<b>
Этот текст будет полужирным, <i>а этот — ещё и курсивным</i>.
</b>
</p>
</body>
</html>

даст такой результат:

Этот текст будет полужирным, а этот — ещё и курсивным.

Кроме элементов, в HTML-документах есть и сущности (англ. entities) — «специальные символы». Сущности начинаются с символа амперсанда и имеют вид &имя; или &#NNNN;, где NNNN — код символа в Юникоде в десятичной системе счисления.

Например, &copy; — знак авторского права (©). Как правило, сущности используются для представления символов, отсутствующих в кодировке документа, или же для представления «специальных» символов: &amp; — амперсанда (&), &lt; — символа «меньше» (<) и &gt; — символа «больше» (>), которые некорректно записывать «обычным» образом, из-за их особого значения в HTML.

Каждый HTML-документ, отвечающий спецификации HTML какой-либо версии, должен начинаться со строки объявления версии HTML <!DOCTYPE…>, которая обычно выглядит примерно так:

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN"
"http://www.w3.org/TR/html4/strict.dtd">

Если эта строка не указана, то добиться корректного отображения документа в браузере становится труднее.

Далее обозначается начало и конец документа тегами <html> и </html> соответственно. Внутри этих тегов должны находиться теги заголовка (<head></head>) и тела (<body></body>) документа.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10008, 10003, 2, N'
Варианты DOCTYPE для HTML 4.01[править | править код]
Строгий (Strict): не содержит элементов, помеченных как «устаревшие» или «не одобряемые» (deprecated).
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

Переходный (Transitional): содержит устаревшие теги в целях совместимости и упрощения перехода со старых версий HTML.
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
"http://www.w3.org/TR/html4/loose.dtd">

С фреймами (Frameset): аналогичен переходному, но содержит также теги для создания наборов фреймов.
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN"
"http://www.w3.org/TR/html4/frameset.dtd">

Варианты DOCTYPE для HTML 5[править | править код]

В HTML 5 используется только один вариант DOCTYPE:

Браузерные войны[править | править код]

В середине 1990-х годов основные производители браузеров — компании Netscape и Microsoft — начали внедрять собственные наборы элементов в HTML-разметку. Создалась путаница из различных конструкций для работы во Всемирной паутине, доступных для просмотра то в одном, то в другом браузере. Особенно большие трудности были при создании кросс-браузерных программ на языке JavaScript. Веб-мастерам приходилось создавать несколько вариантов страниц или прибегать к другим ухищрениям. На какое-то время проблема потеряла актуальность по двум причинам:

Из-за вытеснения браузером Internet Explorer всех остальных браузеров. Соответственно, проблема веб-мастеров становилась проблемой пользователей альтернативных браузеров.
Благодаря усилиям производителей других браузеров, которые либо следовали стандартам W3C (как Mozilla и Opera), либо пытались создать максимальную совместимость с Internet Explorer.

На современном этапе можно констатировать рост популярности браузеров, следующих рекомендациям W3C (это Mozilla Firefox и другие браузеры на движке Gecko; Safari, Google Chrome, Opera и другие браузеры на движке WebKit). Доля Internet Explorer на январь 2016 года составляет менее 15 %[25].

В современной практике существует возможность упростить разработку кросс-браузерных программ на языке JavaScript с помощью различных библиотек и фреймворков. Например, таких как jQuery, sIFR и др.

См. также[править | править код]

SGML
XHTML
DHTML
HTML5
MHTML (сокращение для MIME HTML) — архивный формат веб-страниц, используемый для комбинирования кода HTML и ресурсов, которые обычно представлены в виде внешних ссылок в один файл.
Примечания[править | править код]

↑ D. Connolly, L. Masinter The ''text/html'' Media Type — Internet Engineering Task Force, 2000. — 8 p. — doi:10.17487/RFC2854

↑ https://www.w3.org/People/Raggett/book4/ch02.html

↑ Tim Berners-Lee, «Information Management: A Proposal.» CERN (March 1989, May 1990). W3.org

↑ Tags used in HTML. World Wide Web Consortium (November 3, 1992). Дата обращения 16 ноября 2008.

↑ First mention of HTML Tags on the www-talk mailing list. World Wide Web Consortium (October 29, 1991). Дата обращения 8 апреля 2007.

↑ Index of elements in HTML 4. World Wide Web Consortium (December 24, 1999). Дата обращения 8 апреля 2007.

↑ Berners-Lee, Tim; Connelly, Daniel Hypertext Markup Language – 2.0. Internet Engineering Task Force (November 1995). Дата обращения 1 декабря 2010.

↑ HTML 3.2 Reference Specification

↑ HTML 4.0 Specification

↑ Спецификация HTML 4.01 (англ.)

↑ ISO/IEC 15445:2000(E) ISO-HTML

↑ HTML5 W3C Recommendation

↑ Open Web Platform Milestone Achieved with HTML5 Recommendation

↑ Начало разработки версии 5.1 W3C

↑ HTML 5.1 W3C Working Draft 17 December 2012

↑ HTML 5.1 W3C Recommendation

↑ HTML 5.1 is a W3C Recommendation | W3C News

↑ HTML 5.1 2nd Edition is a W3C Recommendation | W3C News

↑ HTML 5.1 is the gold standard | W3C Blog

↑ HTML 5.2 W3C Recommendation

↑ HTML 5.2 is now a W3C Recommendation | W3C News

↑ HTML 5.2 is done, HTML 5.3 is coming | W3C Blog

↑ XHTML FAQ (англ.)

↑ A feature history of the modern Web Platform

↑ StatCounter Global Stats — Browser, OS, Search Engine including Mobile Usage Share

Литература[править | править код]
Фримен Эрик, Фримен Элизабет. Изучаем HTML, XHTML и CSS = Head First HTML with CSS & XHTML. — П.: «Питер», 2010. — 656 с. — ISBN 978-5-49807-113-8.
Эд Титтел, Джефф Ноубл. HTML, XHTML и CSS для чайников, 7-е издание = HTML, XHTML & CSS For Dummies, 7th Edition. — М.: «Диалектика», 2011. — 400 с. — ISBN 978-5-8459-1752-2.
Питер Лабберс, Брайан Олберс, Фрэнк Салим. HTML5 для профессионалов: мощные инструменты для разработки современных веб-приложений = Pro HTML5 Programming: Powerful APIs for Richer Internet Application Development. — М.: «Вильямс», 2011. — 272 с. — ISBN 978-5-8459-1715-7.
Стивен Шафер. HTML, XHTML и CSS. Библия пользователя, 5-е издание = HTML, XHTML, and CSS Bible, 5th Edition. — М.: «Диалектика», 2010. — 656 с. — ISBN 978-5-8459-1676-1.
Фримен Эрик, Фримен Элизабет. Изучаем HTML, XHTML и CSS = Head First HTML with CSS & XHTML. — 1-е изд. — М.: «Питер», 2010. — С. 656. — ISBN 978-5-49807-113-8.

Ссылки[править | править код]
HTML4.01 W3C Recommendation (англ.)
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10009, 10004, 0, N'
Нам очень жаль, но запросы, поступившие с вашего IP-адреса, похожи на автоматические.

Произнести

Воспроизводится

Другой код

Аудио

Изображение

Введите код

Неверно, попробуйте еще раз

Используйте строчные и прописные буквы, знаки препинания. Между словами поставьте пробел.

Если у вас возникли проблемы, пожалуйста, воспользуйтесь формой обратной связи.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10010, 10005, 0, N'
Нам очень жаль, но запросы, поступившие с вашего IP-адреса, похожи на автоматические.

Произнести

Воспроизводится

Другой код

Аудио

Изображение

Введите код

Неверно, попробуйте еще раз

Используйте строчные и прописные буквы, знаки препинания. Между словами поставьте пробел.

Если у вас возникли проблемы, пожалуйста, воспользуйтесь формой обратной связи.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10011, 10006, 0, N'HTML

Расширение

.html или .htm

MIME-тип

text/html[1]

Разработчик

Консорциум Всемирной паутины, Инженерный совет Интернета и WHATWG

Опубликован

1992

Тип формата

Язык разметки

Расширен из

SGML[2]

Стандарт(ы)

W3C HTML 5.2
WHATWG HTML Living Standard

Сайт

w3.org/html/
html.spec.whatwg.org/mul…
w3c.github.io/html/

HTML

HTML и HTML5
Динамический HTML
XHTML
XHTML Mobile Profile и CHTML
Document Object Model
Семейство шрифтов
Кодировки символов
Мнемоники в HTML
Редактор HTML
Элементы HTML
Семантическая вёрстка
Карта изображений
Цвета HTML
Формы HTML
Фреймы HTML
HTML5 audio и HTML5 video
Canvas
Скрипты в HTML
Unicode и HTML
Браузерный движок
Quirks mode
Каскадные таблицы стилей
W3C и WHATWG
Web Storage
WebGL
Сравнение
языков разметки документов
браузеров
браузерных движков

HTML (от англ. HyperText Markup Language — «язык гипертекстовой разметки») — стандартизированный язык разметки документов во Всемирной паутине. Большинство веб-страниц содержат описание разметки на языке HTML (или XHTML). Язык HTML интерпретируется браузерами; полученный в результате интерпретации форматированный текст отображается на экране монитора компьютера или мобильного устройства.

Язык HTML до 5-й версии определялся как приложение SGML (стандартного обобщённого языка разметки по стандарту ISO 8879). Спецификации HTML5 формулируются в терминах DOM (объектной модели документа).

Язык XHTML является более строгим вариантом HTML, он следует синтаксису XML и является приложением языка XML в области разметки гипертекста.

Во всемирной паутине HTML-страницы, как правило, передаются браузерам от сервера по протоколам HTTP или HTTPS, в виде простого текста или с использованием шифрования.

Общее представление[править | править код]

Язык гипертекстовой разметки HTML был разработан британским учёным Тимом Бернерсом-Ли приблизительно в 1986—1991 годах в стенах ЦЕРНа в Женеве в Швейцарии[3]. HTML создавался как язык для обмена научной и технической документацией, пригодный для использования людьми, не являющимися специалистами в области вёрстки. HTML успешно справлялся с проблемой сложности SGML путём определения небольшого набора структурных и семантических элементов — дескрипторов. Дескрипторы также часто называют «тегами». С помощью HTML можно легко создать относительно простой, но красиво оформленный документ. Помимо упрощения структуры документа, в HTML внесена поддержка гипертекста. Мультимедийные возможности были добавлены позже.

Первым общедоступным описанием HTML был документ «Теги HTML», впервые упомянутый в Интернете Тимом Бернерсом-Ли в конце 1991 года,[4][5]. В нём описываются 18 элементов, составляющих первоначальный, относительно простой дизайн HTML. За исключением тега гиперссылки, на них сильно повлиял SGMLguid, внутренний формат документации, основанный на стандартном обобщенном языке разметки (SGML), в CERN. Одиннадцать из этих элементов всё ещё существуют в HTML 4[6].

Изначально язык HTML был задуман и создан как средство структурирования и форматирования документов без их привязки к средствам воспроизведения (отображения). В идеале, текст с разметкой HTML должен был без стилистических и структурных искажений воспроизводиться на оборудовании с различной технической оснащённостью (цветной экран современного компьютера, монохромный экран органайзера, ограниченный по размерам экран мобильного телефона или устройства и программы голосового воспроизведения текстов). Однако современное применение HTML очень далеко от его изначальной задачи. Например, тег <table> предназначен для создания в документах таблиц, но иногда используется и для оформления размещения элементов на странице. С течением времени основная идея платформонезависимости языка HTML была принесена в жертву современным потребностям в мультимедийном и графическом оформлении.

Браузеры[править | править код]

Текстовые документы, содержащие разметку на языке HTML (такие документы традиционно имеют расширение .html или .htm), обрабатываются специальными приложениями, которые отображают документ в его форматированном виде. Такие приложения, называемые «браузерами» или «интернет-обозревателями», обычно предоставляют пользователю удобный интерфейс для запроса веб-страниц, их просмотра (и вывода на иные внешние устройства) и, при необходимости, отправки введённых пользователем данных на сервер. Наиболее популярными на сегодняшний день браузерами являются Google Chrome, Mozilla Firefox, Opera, Internet Explorer и Safari (см.: Браузер#Рыночные доли).

Версии[править | править код]
HTML 2.0 — опубликован IETF как RFC 1866 в статусе Proposed Standard (24 ноября 1995 года)[7];
HTML 3.0 — 28 марта 1995 года — IETF Internet Draft (до 28 сентября 1995 года);
HTML 3.2[8] — 14 января 1997 года;
HTML 4.0[9] — 18 декабря 1997 года;
HTML 4.01[10] — 24 декабря 1999 года;
ISO/IEC 15445:2000[11] (так называемый ISO HTML, основан на HTML 4.01 Strict) — 15 мая 2000 года;
HTML5[12] — 28 октября 2014 года[13];
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10012, 10006, 1, N'HTML 5.1 начал разрабатываться 17 декабря 2012 года[14][15]. Рекомендован к применению с 1 ноября 2016 года[16][17][18][19].
HTML 5.2 был представлен 14 декабря 2017 года[20][21][22].
HTML 5.3 был представлен 24 декабря 2018 года.

Официальной спецификации HTML 1.0 не существует. До 1995 года существовало множество неофициальных стандартов HTML. Чтобы стандартная версия отличалась от них, ей сразу присвоили второй номер.

Версия 3 была предложена Консорциумом Всемирной паутины (W3C) в марте 1995 года и обеспечивала много новых возможностей, таких как создание таблиц, «обтекание» изображений текстом и отображение сложных математических формул, поддержка gif формата. Даже при том, что этот стандарт был совместим со второй версией, реализация его была сложна для браузеров того времени. Версия 3.1 официально никогда не предлагалась, и следующей версией стандарта HTML стала 3.2, в которой были опущены многие нововведения версии 3.0, но добавлены нестандартные элементы, поддерживаемые браузерами Netscape Navigator и Mosaic.

В версии HTML 4.0 произошла некоторая «очистка» стандарта. Многие элементы были отмечены как устаревшие и не рекомендованные (англ. deprecated). В частности, тег <font>, используемый для изменения свойств шрифта, был помечен как устаревший (вместо него рекомендуется использовать таблицы стилей CSS).

В 1998 году Консорциум Всемирной паутины начал работу над новым языком разметки, основанным на HTML 4, но соответствующим синтаксису XML. Впоследствии новый язык получил название XHTML. Первая версия XHTML 1.0 одобрена в качестве Рекомендации консорциума Всемирной паутины 26 января 2000 года.

Планируемая версия XHTML 2.0 должна была разорвать совместимость со старыми версиями HTML и XHTML, но 2 июля 2009 года Консорциум Всемирной паутины объявил, что полномочия рабочей группы XHTML2 истекают в конце 2009 года. Таким образом, была приостановлена вся дальнейшая разработка стандарта XHTML 2.0[23].

Перспективы[править | править код]

В настоящее время Консорциум Всемирной паутины разработал HTML версии 5. Черновой вариант спецификации языка появился в Интернете 20 ноября 2007 года.

Сообществом WHATWG (англ. Web Hypertext Application Technology Working Group), начиная с 2004 года[24], разрабатывается спецификация Web Applications 1.0, часто неофициально называемая «HTML 5», которая расширяет HTML (впрочем, имея и совместимый с XHTML 1.0 XML-синтаксис) для лучшего представления семантики различных типичных страниц, например форумов, сайтов аукционов, поисковых систем, онлайн-магазинов и т. д., которые не очень удачно вписываются в модель XHTML 2.0.

Структура HTML-документа[править | править код]

HTML — теговый язык разметки документов. Любой документ на языке HTML представляет собой набор элементов, причём начало и конец каждого элемента обозначается специальными пометками — тегами. Элементы могут быть пустыми, то есть не содержащими никакого текста и других данных. В этом случае обычно не указывается закрывающий тег (например, тег переноса строки <br> — одиночный и закрывать его не нужно) . Кроме того, элементы могут иметь атрибуты, определяющие какие-либо их свойства (например, атрибут href=" у ссылки). Атрибуты указываются в открывающем теге. Вот примеры фрагментов HTML-документа:

<strong>Текст между двумя тегами — открывающим и закрывающим.</strong>
<a href="http://www.example.com">Здесь элемент содержит атрибут href, то есть гиперссылку.</a>
А вот пример пустого элемента: <br>

Регистр, в котором набрано имя элемента и имена атрибутов, в HTML значения не имеет (в отличие от XHTML). Элементы могут быть вложенными. Например, следующий код:

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8" />
<title>HTML Document</title>
</head>
<body>
<p>
<b>
Этот текст будет полужирным, <i>а этот — ещё и курсивным</i>.
</b>
</p>
</body>
</html>

даст такой результат:

Этот текст будет полужирным, а этот — ещё и курсивным.

Кроме элементов, в HTML-документах есть и сущности (англ. entities) — «специальные символы». Сущности начинаются с символа амперсанда и имеют вид &имя; или &#NNNN;, где NNNN — код символа в Юникоде в десятичной системе счисления.

Например, &copy; — знак авторского права (©). Как правило, сущности используются для представления символов, отсутствующих в кодировке документа, или же для представления «специальных» символов: &amp; — амперсанда (&), &lt; — символа «меньше» (<) и &gt; — символа «больше» (>), которые некорректно записывать «обычным» образом, из-за их особого значения в HTML.

Каждый HTML-документ, отвечающий спецификации HTML какой-либо версии, должен начинаться со строки объявления версии HTML <!DOCTYPE…>, которая обычно выглядит примерно так:

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN"
"http://www.w3.org/TR/html4/strict.dtd">

Если эта строка не указана, то добиться корректного отображения документа в браузере становится труднее.

Далее обозначается начало и конец документа тегами <html> и </html> соответственно. Внутри этих тегов должны находиться теги заголовка (<head></head>) и тела (<body></body>) документа.
')
GO
INSERT [dbo].[DbResourceContentPortionInfo] ([Id], [ResourceId], [Order], [TextContent]) VALUES (10013, 10006, 2, N'
Варианты DOCTYPE для HTML 4.01[править | править код]
Строгий (Strict): не содержит элементов, помеченных как «устаревшие» или «не одобряемые» (deprecated).
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

Переходный (Transitional): содержит устаревшие теги в целях совместимости и упрощения перехода со старых версий HTML.
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
"http://www.w3.org/TR/html4/loose.dtd">

С фреймами (Frameset): аналогичен переходному, но содержит также теги для создания наборов фреймов.
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN"
"http://www.w3.org/TR/html4/frameset.dtd">

Варианты DOCTYPE для HTML 5[править | править код]

В HTML 5 используется только один вариант DOCTYPE:

Браузерные войны[править | править код]

В середине 1990-х годов основные производители браузеров — компании Netscape и Microsoft — начали внедрять собственные наборы элементов в HTML-разметку. Создалась путаница из различных конструкций для работы во Всемирной паутине, доступных для просмотра то в одном, то в другом браузере. Особенно большие трудности были при создании кросс-браузерных программ на языке JavaScript. Веб-мастерам приходилось создавать несколько вариантов страниц или прибегать к другим ухищрениям. На какое-то время проблема потеряла актуальность по двум причинам:

Из-за вытеснения браузером Internet Explorer всех остальных браузеров. Соответственно, проблема веб-мастеров становилась проблемой пользователей альтернативных браузеров.
Благодаря усилиям производителей других браузеров, которые либо следовали стандартам W3C (как Mozilla и Opera), либо пытались создать максимальную совместимость с Internet Explorer.

На современном этапе можно констатировать рост популярности браузеров, следующих рекомендациям W3C (это Mozilla Firefox и другие браузеры на движке Gecko; Safari, Google Chrome, Opera и другие браузеры на движке WebKit). Доля Internet Explorer на январь 2016 года составляет менее 15 %[25].

В современной практике существует возможность упростить разработку кросс-браузерных программ на языке JavaScript с помощью различных библиотек и фреймворков. Например, таких как jQuery, sIFR и др.

См. также[править | править код]

SGML
XHTML
DHTML
HTML5
MHTML (сокращение для MIME HTML) — архивный формат веб-страниц, используемый для комбинирования кода HTML и ресурсов, которые обычно представлены в виде внешних ссылок в один файл.
Примечания[править | править код]

↑ D. Connolly, L. Masinter The ''text/html'' Media Type — Internet Engineering Task Force, 2000. — 8 p. — doi:10.17487/RFC2854

↑ https://www.w3.org/People/Raggett/book4/ch02.html

↑ Tim Berners-Lee, «Information Management: A Proposal.» CERN (March 1989, May 1990). W3.org

↑ Tags used in HTML. World Wide Web Consortium (November 3, 1992). Дата обращения 16 ноября 2008.

↑ First mention of HTML Tags on the www-talk mailing list. World Wide Web Consortium (October 29, 1991). Дата обращения 8 апреля 2007.

↑ Index of elements in HTML 4. World Wide Web Consortium (December 24, 1999). Дата обращения 8 апреля 2007.

↑ Berners-Lee, Tim; Connelly, Daniel Hypertext Markup Language – 2.0. Internet Engineering Task Force (November 1995). Дата обращения 1 декабря 2010.

↑ HTML 3.2 Reference Specification

↑ HTML 4.0 Specification

↑ Спецификация HTML 4.01 (англ.)

↑ ISO/IEC 15445:2000(E) ISO-HTML

↑ HTML5 W3C Recommendation

↑ Open Web Platform Milestone Achieved with HTML5 Recommendation

↑ Начало разработки версии 5.1 W3C

↑ HTML 5.1 W3C Working Draft 17 December 2012

↑ HTML 5.1 W3C Recommendation

↑ HTML 5.1 is a W3C Recommendation | W3C News

↑ HTML 5.1 2nd Edition is a W3C Recommendation | W3C News

↑ HTML 5.1 is the gold standard | W3C Blog

↑ HTML 5.2 W3C Recommendation

↑ HTML 5.2 is now a W3C Recommendation | W3C News

↑ HTML 5.2 is done, HTML 5.3 is coming | W3C Blog

↑ XHTML FAQ (англ.)

↑ A feature history of the modern Web Platform

↑ StatCounter Global Stats — Browser, OS, Search Engine including Mobile Usage Share

Литература[править | править код]
Фримен Эрик, Фримен Элизабет. Изучаем HTML, XHTML и CSS = Head First HTML with CSS & XHTML. — П.: «Питер», 2010. — 656 с. — ISBN 978-5-49807-113-8.
Эд Титтел, Джефф Ноубл. HTML, XHTML и CSS для чайников, 7-е издание = HTML, XHTML & CSS For Dummies, 7th Edition. — М.: «Диалектика», 2011. — 400 с. — ISBN 978-5-8459-1752-2.
Питер Лабберс, Брайан Олберс, Фрэнк Салим. HTML5 для профессионалов: мощные инструменты для разработки современных веб-приложений = Pro HTML5 Programming: Powerful APIs for Richer Internet Application Development. — М.: «Вильямс», 2011. — 272 с. — ISBN 978-5-8459-1715-7.
Стивен Шафер. HTML, XHTML и CSS. Библия пользователя, 5-е издание = HTML, XHTML, and CSS Bible, 5th Edition. — М.: «Диалектика», 2010. — 656 с. — ISBN 978-5-8459-1676-1.
Фримен Эрик, Фримен Элизабет. Изучаем HTML, XHTML и CSS = Head First HTML with CSS & XHTML. — 1-е изд. — М.: «Питер», 2010. — С. 656. — ISBN 978-5-49807-113-8.

Ссылки[править | править код]
HTML4.01 W3C Recommendation (англ.)
')
GO
SET IDENTITY_INSERT [dbo].[DbResourceContentPortionInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbResourceInfo] ON 
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (1, N'https://docs.microsoft.com/ru-ru/visualstudio/test/unit-test-your-code?view=vs-2019', N'Unit testing', 1, CAST(N'2020-04-11T12:25:48.683' AS DateTime), 0, 4, CAST(N'2020-04-26T16:03:16.160' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (2, N'https://en.wikipedia.org/wiki/unit_testing', N'Wiki // Unit testing', 4, CAST(N'2020-04-11T12:27:03.237' AS DateTime), 1, 2, CAST(N'2020-04-11T12:28:34.293' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (3, N'http://habr.com/', N'Лучшие публикации за сутки / Хабр', 4, CAST(N'2020-04-26T17:54:29.140' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (4, N'http://news.yandex.ru/', N'Яндекс.Новости: Главные новости сегодня, самые свежие и последние новости России онлайн', 4, CAST(N'2020-04-26T18:08:24.800' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (5, N'http://htmlbook.ru/', N'htmlbook.ru | Для тех, кто делает сайты', 4, CAST(N'2020-04-26T20:57:05.987' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (10003, N'http://ru.wikipedia.org/wiki/html', N'HTML — Википедия', 4, CAST(N'2020-04-28T23:18:54.373' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (10004, N'https://ya.ru/', N'SOmeTitle', 4, CAST(N'2020-05-03T17:27:54.320' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (10005, N'http://ya.ru/', N'SOmeTitle', 4, CAST(N'2020-05-03T17:30:15.423' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbResourceInfo] ([Id], [Url], [Title], [AuthorUserId], [CreationStamp], [IsValidated], [ValidatorUserId], [ValidationStamp]) VALUES (10006, N'https://ru.wikipedia.org/wiki/html', N'HTML', 4, CAST(N'2020-05-05T23:03:35.453' AS DateTime), 0, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[DbResourceInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbTagInfo] ON 
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (1)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (2)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (3)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (4)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (5)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (6)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (7)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (8)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (9)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (10)
GO
INSERT [dbo].[DbTagInfo] ([Id]) VALUES (11)
GO
SET IDENTITY_INSERT [dbo].[DbTagInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbTagInstanceInfo] ON 
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (1, 1, N'unit')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (2, 2, N'testing')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (3, 3, N'microsoftvisualstudio')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (4, 4, N'it news')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (5, 5, N'world news')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (6, 6, N'event')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (7, 7, N'html')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (8, 8, N'js')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (9, 9, N'javascript')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (10, 10, N'dom')
GO
INSERT [dbo].[DbTagInstanceInfo] ([Id], [TagId], [Word]) VALUES (11, 11, N'css')
GO
SET IDENTITY_INSERT [dbo].[DbTagInstanceInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbTopicAssociationInfo] ON 
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (2, 2, 2)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (4, 1, 4)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (5, 2, 4)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (8, 7, 7)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (9, 8, 7)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (10, 10, 7)
GO
INSERT [dbo].[DbTopicAssociationInfo] ([Id], [TagInstanceId], [TopicId]) VALUES (11, 11, 7)
GO
SET IDENTITY_INSERT [dbo].[DbTopicAssociationInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbTopicInfo] ON 
GO
INSERT [dbo].[DbTopicInfo] ([Id], [AuthorUserId], [CreationStamp], [Title]) VALUES (2, 4, CAST(N'2020-04-28T18:36:15.833' AS DateTime), N'mynewtesttoping')
GO
INSERT [dbo].[DbTopicInfo] ([Id], [AuthorUserId], [CreationStamp], [Title]) VALUES (4, 4, CAST(N'2020-05-04T20:51:50.773' AS DateTime), N'Unit testing')
GO
INSERT [dbo].[DbTopicInfo] ([Id], [AuthorUserId], [CreationStamp], [Title]) VALUES (7, 4, CAST(N'2020-05-05T23:06:10.097' AS DateTime), N'HTML')
GO
SET IDENTITY_INSERT [dbo].[DbTopicInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbUserInfo] ON 
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (1, N'testeruser', N'testeruser', CAST(N'2020-04-11T12:24:47.910' AS DateTime), CAST(N'2020-04-11T12:27:46.997' AS DateTime), N'Zob3HCEi6+nP+icJ8bBCAq2CN6EsU/RPdDb7HHtCSAw=', N'iM/8137XTutL4WpWeKzY3MT8iu3tL854B7TbHzmZ1QMUQpg9PmXmjHR4diuk1PYqwhx/eljXt6dL2ltKYlNMCg==', N'tester@test.com', 1, NULL, CAST(N'2020-04-11T12:24:47.943' AS DateTime), 0, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (2, N'tester', N'tester', CAST(N'2020-04-11T12:26:14.197' AS DateTime), CAST(N'2020-04-21T13:46:01.737' AS DateTime), N'LQEXgohydAFMGie8X4jKllVmevDtztb0RlXHsYV9F2o=', N'ZnP1teI6g7JAojyatlG3w2ZX5uxdxNrXCb9Tvst6C5fuP2VI623tjcLY6s2o2j+KEQxyk6RToGxzpvTugyU+qA==', N'tester@test.com', 1, NULL, CAST(N'2020-04-11T12:26:14.200' AS DateTime), 0, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (3, N'panama', N'panama', CAST(N'2020-04-23T16:42:58.877' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), N'DYggwfpeMnH/FZlemefVXxqMhV4buCtdksjuMA0zpsI=', N'JNmy8Z4sQH9dA7hg+E1PDgkugN2O9eoAVHFHrP4uzorb0eNWxFTsmvnIegkxXJ2LEno6O9fXxWqBIEkjQ//eJg==', N'k.k@l', 1, N'UuZ7kw91cuVZVe1itu7r5ZpoVpRHmMd8FZaciNCa0yWi86KDC0bLA9rflzmkCTuEZf8as6SyAdO8tBx25lXzg', CAST(N'2020-04-23T16:42:58.983' AS DateTime), 0, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (4, N'panama2', N'panama2', CAST(N'2020-04-23T16:43:31.137' AS DateTime), CAST(N'2020-05-06T13:56:59.380' AS DateTime), N'uEpnkVjkvE1L0bGJY4wXs06fuCJXeVY9aEwchi27LXE=', N'BllYPs3b6ALeFEsOhEB+eELoLqxbrqPAuNkfKRs58d87R4jryu2pa45CWlpVLfVrwHM87Rj981tPS993lEE8LQ==', N'tt@tt.tt', 1, N'4fOrl8kzv8633d1DeEE5Hayn085ogWmL6u1lvLgpGtJ5wIuqq84vwnbTfrxMhp1J6nJ6dI71WSlkeoOECXsXA', CAST(N'2020-04-23T16:43:31.147' AS DateTime), 0, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (5, N'tester2', N'tester2', CAST(N'2020-05-03T15:51:16.567' AS DateTime), CAST(N'2020-05-04T19:36:54.580' AS DateTime), N'nip8OAZ2O06En59jJ+hbmy+oXCqqo2nQ+fH1dnWr3cM=', N'MH5SEaFVs+NHgd8PRyuBQ7ItfCQPQL98wpbmAp3EAL/ztIjYujAfo/6+lJddMFZdN/k7oozrAoTKjyZbrPkSBw==', N'tt@tt.tt', 1, NULL, CAST(N'2020-05-04T19:36:38.030' AS DateTime), 1, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (6, N'tester3', N'tester3', CAST(N'2020-05-03T15:55:19.493' AS DateTime), CAST(N'2020-05-04T18:36:22.497' AS DateTime), N'CceuPYpTt1RaFaZp0PyxLN+AJyOVE+F3KNHC9ocnrpc=', N'Dtj5rv4Fg2iDZmYwkdyNGhq4P0xZ00jmnXabazwDMrBAPXMuNvabhn6OFwLg+FvPMdGRWstq4GbMhvPrylfwzQ==', N't@t.t', 1, NULL, CAST(N'2020-05-04T18:36:16.290' AS DateTime), 1, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (7, N'tester4', N'tester4', CAST(N'2020-05-04T19:43:33.073' AS DateTime), CAST(N'2020-05-04T19:43:47.167' AS DateTime), N'V4qn/LKE+mMIJ0aD1ufWdq9pwg9JUDul18H9vDG/TTw=', N'ep/KrchNoNQwOMd3hb5TVinvyqqvUrG6V9MsRC/AmVz1NbnbnPzU+S+fj5Puq/rpcooj2NZw0czQifDQRULlAw==', N't@t.t', 0, N'Q1VL92MsHys4ZDK7HvohIgjhgJM73j6szwmGeJbiQoSUtlpGFqzGX3Hl56k5v9U0sAGaOUnPhr9LiWswQag7g', CAST(N'2020-05-04T19:43:33.127' AS DateTime), 0, 0)
GO
SET IDENTITY_INSERT [dbo].[DbUserInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbUserSubscriptionContentPortionInfo] ON 
GO
INSERT [dbo].[DbUserSubscriptionContentPortionInfo] ([Id], [UserSubscriptionId], [ResourceContentPortionId], [IsLearned], [LearnedStamp], [IsDelivered], [DeliveredStamp], [IsMarkedToRepeat]) VALUES (6, 2, 2, 1, CAST(N'2020-05-03T19:38:21.590' AS DateTime), 1, CAST(N'2020-05-03T19:37:21.633' AS DateTime), 0)
GO
INSERT [dbo].[DbUserSubscriptionContentPortionInfo] ([Id], [UserSubscriptionId], [ResourceContentPortionId], [IsLearned], [LearnedStamp], [IsDelivered], [DeliveredStamp], [IsMarkedToRepeat]) VALUES (7, 2, 3, 1, CAST(N'2020-05-03T19:38:21.887' AS DateTime), 1, CAST(N'2020-05-03T19:38:21.587' AS DateTime), 0)
GO
INSERT [dbo].[DbUserSubscriptionContentPortionInfo] ([Id], [UserSubscriptionId], [ResourceContentPortionId], [IsLearned], [LearnedStamp], [IsDelivered], [DeliveredStamp], [IsMarkedToRepeat]) VALUES (8, 2, 4, 0, CAST(N'1753-01-01T00:00:00.000' AS DateTime), 1, CAST(N'2020-05-03T19:38:58.323' AS DateTime), 0)
GO
SET IDENTITY_INSERT [dbo].[DbUserSubscriptionContentPortionInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbUserSubscriptionInfo] ON 
GO
INSERT [dbo].[DbUserSubscriptionInfo] ([Id], [UserId], [TopicId], [IsActive], [Interval], [NextPortionTime]) VALUES (2, 4, 2, 1, 1602000000000, CAST(N'2020-05-06T09:01:49.610' AS DateTime))
GO
INSERT [dbo].[DbUserSubscriptionInfo] ([Id], [UserId], [TopicId], [IsActive], [Interval], [NextPortionTime]) VALUES (4, 4, 4, 0, 17280000000000, CAST(N'2020-05-06T09:00:01.023' AS DateTime))
GO
INSERT [dbo].[DbUserSubscriptionInfo] ([Id], [UserId], [TopicId], [IsActive], [Interval], [NextPortionTime]) VALUES (5, 4, 7, 1, 864000000000, CAST(N'2020-05-06T09:00:12.507' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[DbUserSubscriptionInfo] OFF
GO
USE [master]
GO
