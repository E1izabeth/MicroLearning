using MicroLearningSvc.Util;
using Moq;
using System;
using System.Timers;

namespace UnitTestProject.Util
{
    class TimerInTest
    {
        public ITimer Instance { get { return this.Mock.Object; } }

        public TimeSpan Interval { get; private set; }

        public Mock<ITimer> Mock { get; private set; }

        readonly ITestContext _ctx;


        public TimerInTest(ITestContext ctx, bool starts, bool stops)
        {
            _ctx = ctx;
            this.Mock = this.MakeMock(ctx, starts, stops);
        }

        private Mock<ITimer> MakeMock(ITestContext ctx, bool starts, bool stops)
        {
            var timer = ctx.Mocks.Create<ITimer>();

            if (starts)
            {
                timer.Setup(x => x.Start());

                timer.SetupSet(x => x.Interval = It.IsAny<TimeSpan>())
                     .Callback((TimeSpan interval) => this.Interval = interval);
            }

            if (stops)
            {
                timer.Setup(x => x.Stop());
            }

            return timer;
        }

        public void Raise()
        {
            this.Mock.Raise(x => x.Elapsed += null, EventArgs.Empty);
        }
    }

    static class TimerInTestContextExtensions
    {
        public static TimerInTest MakeTimerMock(this ITestContext ctx, bool starts, bool stops)
        {
            return new TimerInTest(ctx, starts, stops);
        }

        //public static bool IsDiffBetween(this TimeSpan dt, TimeSpan a, TimeSpan b)
        //{
        //    return Math.Abs(a.Ticks - b.Ticks) < Math.Abs(dt.Ticks);
        //}
    }
}
